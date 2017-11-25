namespace PetProjects.Framework.Logging.Consumer
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using PetProjects.Framework.Logging.Contracts;

    public class PetProjectLogConsumer : ILogEventV1Consumer
    {
        private const string GroupId = "PetProjectLogConsumers";
        private const string ClientIdPrefix = "PetProjectLogConsumer";

        private static readonly TimeSpan PollTimeout = TimeSpan.FromSeconds(2);

        private readonly ILogEventV1Store store;
        private readonly KafkaConfiguration kafkaConfig;
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();
        private readonly object lockObj = new object();

        private Task task;

        public PetProjectLogConsumer(KafkaConfiguration kafkaConfig, ILogEventV1Store store)
        {
            this.store = store;
            this.kafkaConfig = kafkaConfig;
        }

        public bool StartInBackground()
        {
            lock (this.lockObj)
            {
                if (this.task != null)
                {
                    return false;
                }

                // because Poll is synchronous it will block the thread that runs this task
                // so it's better to create a task with LongRunning flag to let the task scheduler know
                // that it can create a new dedicated thread for this task instead of allocating one from the default threadpool
                this.task = Task.Factory.StartNew(
                    () =>
                    {
                        var config = new Dictionary<string, object>
                        {
                            { "group.id", PetProjectLogConsumer.GroupId },
                            { "bootstrap.servers", string.Join(",", this.kafkaConfig.Brokers) },
                            { "client.id", $"{ PetProjectLogConsumer.ClientIdPrefix }-{ Guid.NewGuid() }" }
                        };

                        using (var consumer = new Consumer<Null, List<LogEventV1>>(config, null, new JsonDeserializer<List<LogEventV1>>()))
                        {
                            consumer.OnMessage += this.HandleMessage;
                            while (this.tokenSource != null && !this.tokenSource.IsCancellationRequested)
                            {
                                consumer.Poll(PetProjectLogConsumer.PollTimeout);
                            }
                        }
                    }, 
                    TaskCreationOptions.LongRunning);

                return true;
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

        private void HandleMessage(object sender, Message<Null, List<LogEventV1>> message)
        {
            this.store.Store(message.Value);
        }
    }
}