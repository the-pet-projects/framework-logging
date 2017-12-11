namespace PetProjects.Framework.Logging.Consumer
{
    using System.Collections.Generic;
    using PetProjects.Framework.Logging.Contracts;

    public interface ILogEventV1Store
    {
        void Store(string index, List<LogEventV1> logs);
    }
}