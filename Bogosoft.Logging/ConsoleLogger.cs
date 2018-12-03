using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Bogosoft.Logging
{
    /// <summary>
    /// An implementation of the <see cref="ILogger"/> that uses the console
    /// to output written logs. This class cannot be inherited.
    /// </summary>
    public sealed class ConsoleLogger : ILogger
    {
        /// <summary>
        /// Get the default date format to be used if no other format is specified.
        /// </summary>
        public const string DefaultDateFormat = "yyyy-MM-dd HH:mm:ss.fffK";

        static string ToString(MessageSeverity severity)
        {
            switch (severity)
            {
                case MessageSeverity.Informational:
                    return "INFO";
                default:
                    return severity.ToString().ToUpper();
            }
        }

        readonly string dateFormat;
        readonly TextWriter output;

        /// <summary>
        /// Get or set the console color to be used when printing a timestamp.
        /// </summary>
        public ConsoleColor DateColor = ConsoleColor.Cyan;

        /// <summary>
        /// Get a dictionary of console colors by severity to be used when printing message text.
        /// </summary>
        public readonly Dictionary<MessageSeverity, ConsoleColor> MessageColors = new Dictionary<MessageSeverity, ConsoleColor>
        {
            { MessageSeverity.Debug, ConsoleColor.DarkGray },
            { MessageSeverity.Error, ConsoleColor.DarkRed },
            { MessageSeverity.Informational, ConsoleColor.Gray },
            { MessageSeverity.Warning, ConsoleColor.DarkYellow }
        };

        /// <summary>
        /// Get a dictionary of console colors by severity to be used when printing message severity labels.
        /// </summary>
        public readonly Dictionary<MessageSeverity, ConsoleColor> MessageSeverityLabelColors = new Dictionary<MessageSeverity, ConsoleColor>
        {
            { MessageSeverity.Debug, ConsoleColor.Gray },
            { MessageSeverity.Error, ConsoleColor.Red },
            { MessageSeverity.Informational, ConsoleColor.White },
            { MessageSeverity.Warning, ConsoleColor.Yellow }
        };

        /// <summary>
        /// Create a new instance of the <see cref="ConsoleLogger"/> class that targets STDOUT.
        /// <see cref="DefaultDateFormat"/> will be used when printing timestamps.
        /// </summary>
        public ConsoleLogger()
            : this(DefaultDateFormat, true)
        {
        }

        /// <summary>
        /// Create a new instance of the <see cref="ConsoleLogger"/> class that targets STDOUT.
        /// </summary>
        /// <param name="dateFormat">A format to be used when printing timestamps.</param>
        public ConsoleLogger(string dateFormat)
            : this(dateFormat, true)
        {
        }

        /// <summary>
        /// Create a new instance of the <see cref="ConsoleLogger"/> class. <see cref="DefaultDateFormat"/>
        /// will be used when printing timestamps.
        /// </summary>
        /// <param name="useStdOut">True to target STDOUT; false to target STDERR.</param>
        public ConsoleLogger(bool useStdOut)
            : this(DefaultDateFormat, useStdOut)
        {
        }

        /// <summary>
        /// Create a new instance of the <see cref="ConsoleLogger"/> class.
        /// </summary>
        /// <param name="dateFormat">A format to be used when printing timestamps.</param>
        /// <param name="useStdOut">True to target STDOUT; false to target STDERR.</param>
        public ConsoleLogger(string dateFormat, bool useStdOut)
        {
            this.dateFormat = dateFormat;

            output = useStdOut ? Console.Out : Console.Error;
        }

        /// <summary>
        /// Log a given message.
        /// </summary>
        /// <param name="message">A message to log.</param>
        public void Log(IMessage message)
        {
            Console.ForegroundColor = DateColor;

            output.Write(message.Date.ToString(dateFormat));

            Console.ResetColor();

            output.Write(' ');

            Console.ForegroundColor = MessageSeverityLabelColors[message.Severity];

            output.Write(ToString(message.Severity));

            Console.ResetColor();

            output.Write(' ');

            Console.ForegroundColor = MessageColors[message.Severity];

            output.WriteLine(message.ToString());

            Console.ResetColor();
        }

        /// <summary>
        /// Log a given message.
        /// </summary>
        /// <param name="message">A message to log.</param>
        /// <param name="token">A cancellation instruction.</param>
        /// <returns>A task representing a possibly asynchronous operation.</returns>
        public async Task LogAsync(IMessage message, CancellationToken token)
        {
            Console.ForegroundColor = DateColor;

            await output.WriteAsync(message.Date.ToString(dateFormat), token);

            Console.ResetColor();

            await output.WriteAsync(' ', token);

            Console.ForegroundColor = MessageSeverityLabelColors[message.Severity];

            await output.WriteAsync(ToString(message.Severity), token);

            Console.ResetColor();

            await output.WriteAsync(' ', token);

            Console.ForegroundColor = MessageColors[message.Severity];

            await output.WriteLineAsync(message.ToString(), token);

            Console.ResetColor();
        }
    }
}