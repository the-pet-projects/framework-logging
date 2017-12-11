namespace PetProjects.Framework.Logging.Consumer.ElasticSearch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Elasticsearch.Net;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Console;

    public static class PetProjectServiceCollectionExtensions
    {
        public static void AddPetProjectElasticLogConsumer(this IServiceCollection collection, KafkaConfiguration kafkaConfig, ElasticClientConfiguration clientConfig)
        {
            collection.TryAddSingleton<ElasticClientConfiguration>(clientConfig);

            var topics = kafkaConfig.Topic.Split(',');
            var indices = topics.Select(topic => $"logs-{ topic }-{ DateTime.UtcNow.ToString("dd-MM-yyyy") }").ToArray();

            collection.TryAddTransient<IElasticLowLevelClientFactory, ElasticLowLevelClientFactory>();
            collection.TryAddSingleton<IElasticLowLevelClient>(serviceProvider => serviceProvider.GetRequiredService<IElasticLowLevelClientFactory>().BuildAsync(serviceProvider.GetRequiredService<ElasticClientConfiguration>(), indices).Result);

            collection.AddLogging(cfg => cfg.AddConsole());
            collection.TryAddSingleton<IPetProjectLogConsumerLogger>(sp =>
            {
                var providers = sp.GetServices<ILoggerProvider>();
                return new PetProjectLogConsumerLogger(providers.First(provider => provider is ConsoleLoggerProvider));
            });

            for (var i = 0; i < topics.Length; i++)
            {
                var topic = topics[i];
                var index = indices[i];

                // this will make the provider own the consumer instance, i.e., the consumer will be disposed automatically by the DI container
                // if we added the singleton like this: collection.AddSingleton(new PetProjectLogConsumer()); then it wouldn't be disposed
                // automatically because the DI didn't actually create the instance so it doesn't own it
                collection.AddSingleton<PetProjectLogConsumer>(sp =>
                    new PetProjectLogConsumer(
                        kafkaConfig.Brokers,
                        kafkaConfig.ConsumerGroupId,
                        topic,
                        new ElasticLogEventV1Store(sp.GetRequiredService<IElasticLowLevelClient>(), index),
                        sp.GetRequiredService<IPetProjectLogConsumerLogger>()));
            }
        }

        public static IEnumerable<Task> StartPetProjectElasticLogConsumerAsync(this IServiceProvider provider)
        {
            var consumers = provider.GetServices<PetProjectLogConsumer>();

            // ToList is important otherwise enumerable is not iterated through and tasks are not started
            return consumers.Select(c => c.StartInBackgroundAsync()).ToList();
        }
    }
}