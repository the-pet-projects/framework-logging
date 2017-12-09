namespace PetProjects.Framework.Logging.Consumer
{
    using System;
    using System.Threading.Tasks;

    public interface ILogEventV1Consumer : IDisposable
    {
        Task StartInBackgroundAsync();
    }
}