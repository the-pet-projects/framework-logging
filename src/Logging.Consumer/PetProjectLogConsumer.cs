namespace PetProjects.Framework.Logging.Consumer
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using PetProjects.Framework.Logging.Contracts;
    using LogLevel = PetProjects.Framework.Logging.Contracts.LogLevel;

    public class PetProjectLogConsumer : ILogEventV1Consumer
    {
        private const string ClientIdPrefix = "PetProjectLogConsumer";

        private static readonly TimeSpan PollTimeout = TimeSpan.FromSeconds(2);

        private readonly ILogEventV1Store store;
        private readonly KafkaConfiguration kafkaConfig;
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();
        private readonly object lockObj = new object();
        private readonly IPetProjectLogConsumerLogger logger;
        private Consumer<Null, List<LogEventV1>> consumer;

        private Task task;

        public PetProjectLogConsumer(KafkaConfiguration kafkaConfig, ILogEventV1Store store, IPetProjectLogConsumerLogger logger)
        {
            this.store = store;
            this.kafkaConfig = kafkaConfig;
            this.logger = logger;
        }

        public Task StartInBackgroundAsync()
        {
            lock (this.lockObj)
            {
                if (this.task != null)
                {
                    return this.task;
                }

                // because Poll is synchronous it will block the thread that runs this task
                // so it's better to create a task with LongRunning flag to let the task scheduler know
                // that it can create a new dedicated thread for this task instead of allocating one from the default threadpool
                this.task = Task.Factory.StartNew(
                    () =>
                    {
                        this.WrapTryCatchWithLogger(() =>
                        {
                            var config = new Dictionary<string, object>
                            {
                                { "group.id", this.kafkaConfig.ConsumerGroupId },
                                { "bootstrap.servers", string.Join(",", this.kafkaConfig.Brokers) },
                                { "client.id", $"{ ClientIdPrefix }-{ Guid.NewGuid() }" }
                            };

                            this.consumer = new Consumer<Null, List<LogEventV1>>(config, null, new JsonDeserializer<List<LogEventV1>>());
                            this.consumer.Subscribe(this.kafkaConfig.Topic);
                            this.consumer.OnMessage += this.HandleMessage;
                            this.consumer.OnConsumeError += this.HandleConsumeError;
                            this.consumer.OnError += this.HandleError;
                            this.consumer.OnStatistics += this.HandleStatistics;
                            this.consumer.OnLog += this.HandleLog;

                            while (this.tokenSource != null && !this.tokenSource.IsCancellationRequested)
                            {
                                this.consumer.Poll(PollTimeout);
                            }
                        });
                    },
                    TaskCreationOptions.LongRunning);

                return this.task;
            }
        }

        public void Dispose()
        {
            lock (this.lockObj)
            {
                if (this.task == null)
                {
                    this.task = Task.CompletedTask;
                }
            }

            this.tokenSource.Cancel();
            this.task.Wait();
            this.tokenSource?.Dispose();
        }

        private void HandleLog(object sender, LogMessage e)
        {
            this.StoreLog(this.BuildLogEvent(LogLevel.Information, "HandleLog called", null, null, new Dictionary<string, string> { { "LogMessage", JsonConvert.SerializeObject(e) } }));
        }

        private void HandleStatistics(object sender, string e)
        {
            this.StoreLog(this.BuildLogEvent(LogLevel.Information, "HandleStatistics called", null, null, new Dictionary<string, string> { { "StatisticValue", e } }));
        }

        private void HandleError(object sender, Error e)
        {
            this.StoreLog(this.BuildLogEvent(LogLevel.Error, "HandleError called", null, null, new Dictionary<string, string> { { "Error", JsonConvert.SerializeObject(e) } }));
        }

        private void HandleConsumeError(object sender, Message e)
        {
            this.StoreLog(this.BuildLogEvent(LogLevel.Error, "HandleConsumeError called", null, null, new Dictionary<string, string> { { "Message", JsonConvert.SerializeObject(e) } }));
        }

        private void HandleMessage(object sender, Message<Null, List<LogEventV1>> message)
        {
            this.StoreLog(message.Value);
            this.consumer.CommitAsync(message).Wait();
        }

        private void StoreLog(List<LogEventV1> messages)
        {
            try
            {
                this.store.Store(messages);
            }
            catch (Exception ex)
            {
                this.logger.LogCritical(ex, "Exception occured while storing the following logs in elasticsearch: {logMessages}", messages);
            }
        }

        private void WrapTryCatchWithLogger(Action act)
        {
            try
            {
                act();
            }
            catch (Exception ex)
            {
                this.logger.LogCritical(ex, "Exception occured in log aggregator.");
            }
        }

        private List<LogEventV1> BuildLogEvent(LogLevel level, string message, Exception ex = null, object[] args = null, Dictionary<string, string> properties = null)
        {
            return new List<LogEventV1>
            {
                new LogEventV1
                {
                    BatchId = Guid.NewGuid().ToString(),
                    Type = "LogAggregatorConsumer",
                    Exception = ex,
                    Level = level,
                    MessageTemplate = message,
                    Timestamp = DateTimeOffset.UtcNow,
                    RenderedMessage = (args != null && args.Length > 0) ? string.Format(message, args) : message,
                    Properties = properties ?? new Dictionary<string, string>()
                }
            };
        }
    }
}