using Bogosoft.Cloning;
using System;

namespace Bogosoft.Logging
{
    /// <summary>
    /// Represents any type that acts like a message.
    /// </summary>
    public interface IMessage : ICloneable<IMessage>, IEquatable<IMessage>
    {
        /// <summary>
        /// Get or set a message ID to correlate to the current message.
        /// </summary>
        Guid CorrelatedId { get; set; }

        /// <summary>
        /// Get the date of the current message.
        /// </summary>
        DateTimeOffset Date { get; }

        /// <summary>
        /// Get the text format of the current message.
        /// </summary>
        string Format { get; }

        /// <summary>
        /// Get the ID assigned to the current message.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Get the severity of the current message.
        /// </summary>
        MessageSeverity Severity { get; }

        /// <summary>
        /// Get the format-substitutable values of the current message.
        /// </summary>
        object[] Values { get; }
    }
}