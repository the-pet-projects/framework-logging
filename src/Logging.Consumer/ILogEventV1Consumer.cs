namespace PetProjects.Framework.Logging.Consumer
{
    using System;

    public interface ILogEventV1Consumer : IDisposable
    {
        bool StartInBackground();
    }
}