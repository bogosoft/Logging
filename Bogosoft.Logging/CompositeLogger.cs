using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bogosoft.Logging
{
    /// <summary>
    /// An implementation of the <see cref="ILogger"/> contract that treats a collection of loggers
    /// as if they were a single logger. This class cannot be inherited.
    /// </summary>
    public sealed class CompositeLogger : ILogger
    {
        readonly ILogger[] loggers;

        /// <summary>
        /// Create a new instance of the <see cref="CompositeLogger"/> class.
        /// </summary>
        /// <param name="loggers">A collection of loggers to become the composite.</param>
        public CompositeLogger(IEnumerable<ILogger> loggers)
        {
            this.loggers = loggers.ToArray();
        }

        /// <summary>
        /// Create a new instance of the <see cref="CompositeLogger"/> class.
        /// </summary>
        /// <param name="loggers">A collection of loggers to become the composite.</param>
        public CompositeLogger(params ILogger[] loggers)
        {
            this.loggers = loggers;
        }

        /// <summary>
        /// Log a given message.
        /// </summary>
        /// <param name="message">A message to log.</param>
        public void Log(IMessage message)
        {
            foreach (var logger in loggers)
            {
                logger.Log(message);
            }
        }

        /// <summary>
        /// Log a given message.
        /// </summary>
        /// <param name="message">A message to log.</param>
        /// <param name="token">A cancellation instruction.</param>
        /// <returns>A task representing a possibly asynchronous operation.</returns>
        public Task LogAsync(IMessage message, CancellationToken token)
        {
            return Task.WhenAll(loggers.Select(x => x.LogAsync(message, token)));
        }
    }
}