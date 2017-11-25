namespace PetProjects.Framework.Logging.Samples.WebApi
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using PetProjects.Framework.Logging.Producer;
    using Serilog.Events;

    public class Program
    {
        public static void Main(string[] args)
        {
            Program.BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddPetProjectLogging(hostingContext.Configuration.GetSection("Logging").GetValue<LogEventLevel>("LogLevel"), new PeriodicSinkConfiguration { BatchSizeLimit = 100, Period = TimeSpan.FromMilliseconds(10) }, new KafkaConfiguration { Brokers = new List<string> { "marx-petprojects.westeurope.cloudapp.azure.com:9092" }, Topic = "testlogs" }, "TestSampleApp", true);
                })
                .UseStartup<Startup>()
                .Build();
    }
}