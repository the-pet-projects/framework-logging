namespace PetProjects.Framework.Logging.Consumer.ElasticSearch
{
    using System;
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
            collection.TryAddSingleton<KafkaConfiguration>(kafkaConfig);
            collection.TryAddSingleton<ElasticClientConfiguration>(clientConfig);
            collection.TryAddSingleton<ElasticStoreConfiguration>(clientConfig);

            collection.TryAddTransient<IElasticLowLevelClientFactory, ElasticLowLevelClientFactory>();
            collection.TryAddSingleton<IElasticLowLevelClient>(serviceProvider => serviceProvider.GetRequiredService<IElasticLowLevelClientFactory>().BuildAsync(serviceProvider.GetRequiredService<ElasticClientConfiguration>()).Result);
            collection.TryAddTransient<ILogEventV1Store, ElasticLogEventV1Store>();

            collection.AddLogging(cfg => cfg.AddConsole());
            collection.TryAddSingleton<IPetProjectLogConsumerLogger>(sp =>
            {
                var providers = sp.GetServices<ILoggerProvider>();
                return new PetProjectLogConsumerLogger(providers.First(provider => provider is ConsoleLoggerProvider));
            });

            // this will make the provider own the consumer instance, i.e., the consumer will be disposed automatically by the DI container
            // if we added the singleton like this: collection.AddSingleton(new PetProjectLogConsumer()); then it wouldn't be disposed
            // automatically because the DI didn't actually create the instance so it doesn't own it
            collection.TryAddSingleton<PetProjectLogConsumer>();
        }

        public static Task StartPetProjectElasticLogConsumerAsync(this IServiceProvider provider)
        {
            var consumer = provider.GetRequiredService<PetProjectLogConsumer>();
            return consumer.StartInBackgroundAsync();
        }
    }
}