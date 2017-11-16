namespace PetProjects.Framework.Logging.Producer
{
    using System;

    public class PeriodicSinkConfiguration
    {
        public int BatchSizeLimit { get; set; }

        public TimeSpan Period { get; set; }
    }
}