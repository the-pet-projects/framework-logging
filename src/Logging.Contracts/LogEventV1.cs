namespace PetProjects.Framework.Logging.Contracts
{
    using System;
    using System.Collections.Generic;

    public class LogEventV1
    {
        /// <summary>
        /// Properties associated with the event, including those presented in <see cref="MessageTemplate" />.
        /// </summary>
        public Dictionary<string, string> Properties { get; set; }

        /// <summary>The time at which the event occurred.</summary>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>The level of the event.</summary>
        public LogLevel Level { get; set; }

        /// <summary>The message template describing the event.</summary>
        public string MessageTemplate { get; set; }

        /// <summary>The message template describing the event.</summary>
        public string RenderedMessage { get; set; }

        /// <summary>An exception associated with the event, or null.</summary>
        public Exception Exception { get; set; }

        /// <summary>Type property associated with this log. This will most likely be an identifier of the log producer.</summary>
        public string Type { get; set; }

        /// <summary>Log Event batch id.</summary>
        public string BatchId { get; set; }
    }
}
