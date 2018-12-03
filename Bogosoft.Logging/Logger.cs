using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bogosoft.Logging
{
    /// <summary>
    /// A set of static members for working with types that implement <see cref="ILogger"/>.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Get a console logger with default settings.
        /// </summary>
        public static ILogger Console => new ConsoleLogger();

        /// <summary>
        /// Correlate future messages logged by the current logger with a given correlation ID.
        /// </summary>
        /// <param name="logger">The current logger.</param>
        /// <param name="correlatedId">An ID to correlate with all future logged messages.</param>
        /// <returns>The current logger correlated with the given ID.</returns>
        public static ILogger Correlate(this ILogger logger, Guid correlatedId)
        {
            return new CorrelatedLogger(logger, m => correlatedId);
        }

        /// <summary>
        /// Correlate future messages logged by the current logger with a given message.
        /// </summary>
        /// <param name="logger">The current logger.</param>
        /// <param name="correlatedMessage">
        /// A message to correlate with all future messages logged by the current logger.
        /// </param>
        /// <returns>The current logger correlated with the given message.</returns>
        public static ILogger Correlate(this ILogger logger, IMessage correlatedMessage)
        {
            return new CorrelatedLogger(logger, m => correlatedMessage.Id);
        }

        /// <summary>
        /// Correlate future message logged by the current logger with an ID generated against each message.
        /// </summary>
        /// <param name="logger">The current logger.</param>
        /// <param name="correlatedIdGenerator">
        /// A strategy for generating correlation ID's against incoming messages.
        /// </param>
        /// <returns>The current logger correlated with the given correlation ID generation strategy.</returns>
        public static ILogger Correlate(this ILogger logger, Func<IMessage, Guid> correlatedIdGenerator)
        {
            return new CorrelatedLogger(logger, correlatedIdGenerator);
        }

        /// <summary>
        /// Log a debug-level message.
        /// </summary>
        /// <param name="logger">The current logger.</param>
        /// <param name="format">A message format.</param>
        /// <param name="values">An array of format-substitutable values.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current logger or given format is null.
        /// </exception>
        public static void Debug(this ILogger logger, string format, params object[] values)
        {
            logger.Log(MessageSeverity.Debug, format, values);
        }

        /// <summary>
        /// Asynchronously log a debug-level message.
        /// </summary>
        /// <param name="logger">The current logger.</param>
        /// <param name="format">A message format.</param>
        /// <param name="values">
        /// An array of format-substitutable values. If the last value is a <see cref="CancellationToken"/>,
        /// it will be used as such and removed from the given array of values.
        /// </param>
        /// <returns>A task representing an asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current logger or given format is null.
        /// </exception>
        public static Task DebugAsync(this ILogger logger, string format, params object[] values)
        {
            return logger.LogAsync(MessageSeverity.Debug, format, values);
        }

        /// <summary>
        /// Log an exception as an error.
        /// </summary>
        /// <param name="logger">The current logger.</param>
        /// <param name="exception">An exception to log as an error.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current logger or given exception is null.
        /// </exception>
        public static void Error(this ILogger logger, Exception exception)
        {
            if (exception is null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            string text;

            do
            {
                text = exception.Message;

                if (!string.IsNullOrEmpty(exception.StackTrace))
                {
                    text += Environment.NewLine + exception.StackTrace;
                }

                logger.Error(text);

            } while (null != (exception = exception.InnerException));
        }

        /// <summary>
        /// Log an error message.
        /// </summary>
        /// <param name="logger">The current logger.</param>
        /// <param name="format">A message format.</param>
        /// <param name="values">An array of format-substitutable values.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current logger or given format is null.
        /// </exception>
        public static void Error(this ILogger logger, string format, params object[] values)
        {
            logger.Log(new Message(MessageSeverity.Error, format, values));
        }

        /// <summary>
        /// Asynchronously log an exception as an error.
        /// </summary>
        /// <param name="logger">The current logger.</param>
        /// <param name="exception">An exception to log as an error.</param>
        /// <param name="token">A cancellation instruction.</param>
        /// <returns>A task representing an asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current logger or given exception is null.
        /// </exception>
        public static async Task ErrorAsync(
            this ILogger logger,
            Exception exception,
            CancellationToken token = default(CancellationToken)
            )
        {
            if (exception is null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            string text;

            do
            {
                text = exception.Message;

                if (!string.IsNullOrEmpty(exception.StackTrace))
                {
                    text += Environment.NewLine + exception.StackTrace;
                }

                await logger.ErrorAsync(text, token).ConfigureAwait(false);

            } while (null != (exception = exception.InnerException));
        }

        /// <summary>
        /// Asynchronously log an error message.
        /// </summary>
        /// <param name="logger">The current logger.</param>
        /// <param name="format">A message format.</param>
        /// <param name="values">
        /// An array of format-substitutable values. If the last value is a <see cref="CancellationToken"/>,
        /// it will be used as such and removed from the given array of values.
        /// </param>
        /// <returns>A task representing an asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current logger or given format is null.
        /// </exception>
        public static Task ErrorAsync(this ILogger logger, string format, params object[] values)
        {
            return logger.LogAsync(MessageSeverity.Error, format, values);
        }

        /// <summary>
        /// Log a message.
        /// </summary>
        /// <param name="logger">The current logger.</param>
        /// <param name="severity">The severity of the message that will be logged.</param>
        /// <param name="format">A message format.</param>
        /// <param name="values">An array of format-substitutable values.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current logger or given format is null.
        /// </exception>
        public static void Log(this ILogger logger, MessageSeverity severity, string format, params object[] values)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (format is null)
            {
                throw new ArgumentNullException(nameof(format));
            }

            logger.Log(new Message(severity, format, values));
        }

        /// <summary>
        /// Asynchronously log a message.
        /// </summary>
        /// <param name="logger">The current logger.</param>
        /// <param name="severity">The severity of the message that will be logged.</param>
        /// <param name="format">A message format.</param>
        /// <param name="values">
        /// An array of format-substitutable values. If the last value is a <see cref="CancellationToken"/>,
        /// it will be used as such and removed from the given array of values.
        /// </param>
        /// <returns>A task representing an asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current logger or given format is null.
        /// </exception>
        public static Task LogAsync(this ILogger logger, MessageSeverity severity, string format, params object[] values)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (format is null)
            {
                throw new ArgumentNullException(nameof(format));
            }

            CancellationToken token;

            var ubound = values.Length - 1;

            if (values.Length > 0 && values[ubound] is CancellationToken)
            {
                token = (CancellationToken)values[ubound];

                values = values.Take(ubound).ToArray();
            }
            else
            {
                token = CancellationToken.None;
            }

            var message = new Message(severity, format, values);

            return logger.LogAsync(message, token);
        }

        /// <summary>
        /// Log a general, informational message.
        /// </summary>
        /// <param name="logger">The current logger.</param>
        /// <param name="format">A message format.</param>
        /// <param name="values">An array of format-substitutable values.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current logger or given format is null.
        /// </exception>
        public static void Info(this ILogger logger, string format, params object[] values)
        {
            logger.Log(new Message(MessageSeverity.Informational, format, values));
        }

        /// <summary>
        /// Asynchronously log a general, informational message.
        /// </summary>
        /// <param name="logger">The current logger.</param>
        /// <param name="format">A message format.</param>
        /// <param name="values">
        /// An array of format-substitutable values. If the last value is a <see cref="CancellationToken"/>,
        /// it will be used as such and removed from the given array of values.
        /// </param>
        /// <returns>A task representing an asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current logger or given format is null.
        /// </exception>
        public static Task InfoAsync(this ILogger logger, string format, params object[] values)
        {
            return logger.LogAsync(MessageSeverity.Informational, format, values);
        }

        /// <summary>
        /// Log a warning.
        /// </summary>
        /// <param name="logger">The current logger.</param>
        /// <param name="format">A message format.</param>
        /// <param name="values">An array of format-substitutable values.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current logger or given format is null.
        /// </exception>
        public static void Warn(this ILogger logger, string format, params object[] values)
        {
            logger.Log(new Message(MessageSeverity.Warning, format, values));
        }

        /// <summary>
        /// Asynchronously log a warning.
        /// </summary>
        /// <param name="logger">The current logger.</param>
        /// <param name="format">A message format.</param>
        /// <param name="values">
        /// An array of format-substitutable values. If the last value is a <see cref="CancellationToken"/>,
        /// it will be used as such and removed from the given array of values.
        /// </param>
        /// <returns>A task representing an asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current logger or given format is null.
        /// </exception>
        public static Task WarnAsync(this ILogger logger, string format, params object[] values)
        {
            return logger.LogAsync(MessageSeverity.Warning, format, values);
        }

        /// <summary>
        /// Constrain the current logger by only logging messages that match a given condition.
        /// </summary>
        /// <param name="logger">The current logger.</param>
        /// <param name="condition">A condition that a message must satisfy in order to be logged.</param>
        /// <returns>The current logger constrained by the given condition.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current logger or given condition is null.
        /// </exception>
        public static ILogger When(this ILogger logger, Func<IMessage, bool> condition)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (condition is null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            return new FilteredLogger(logger, condition);
        }
    }
}