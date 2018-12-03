using System;

namespace Bogosoft.Logging
{
    /// <summary>
    /// An enumeration of message severity levels.
    /// </summary>
    [Flags]
    public enum MessageSeverity
    {
        /// <summary>
        /// The corresponding message has no defined severity.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// The corresponding message contains debug-level information suitable for technical consumers.
        /// </summary>
        Debug = 0x01,

        /// <summary>
        /// The corresponding message is an error.
        /// </summary>
        Error = 0x02,

        /// <summary>
        /// The corresponding message contains information of a general nature.
        /// </summary>
        Informational = 0x04,

        /// <summary>
        /// The corresponding message is a warning.
        /// </summary>
        Warning = 0x08
    }
}