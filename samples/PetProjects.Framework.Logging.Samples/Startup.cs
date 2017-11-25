namespace PetProjects.Framework.Logging.Samples.WebApi
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using PetProjects.Framework.Logging.Consumer;
    using PetProjects.Framework.Logging.Consumer.ElasticSearch;
    using Swashbuckle.AspNetCore.Swagger;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddPetProjectElasticLogConsumer(
                new KafkaConfiguration
                {
                    Brokers = new List<string> { "marx-petprojects.westeurope.cloudapp.azure.com:9092" },
                    Topic = "testlogs"
                },
                new ElasticClientConfiguration
                {
                    Address = "http://bubaloo-petproject.westeurope.cloudapp.azure.com:9200",
                    AppLogsIndex = "logs-testsampleindex"
                });

            services.AddLogging();
            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Values API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ApplicationServices.StartPetProjectElasticLogConsumer();

            app.UseMvc();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Values API v1");
            });
        }
    }
}