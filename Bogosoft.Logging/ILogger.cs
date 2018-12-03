using System.Threading;
using System.Threading.Tasks;

namespace Bogosoft.Logging
{
    /// <summary>
    /// Represents any type capable of logging a message.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Log a given message.
        /// </summary>
        /// <param name="message">A message to log.</param>
        void Log(IMessage message);

        /// <summary>
        /// Log a given message.
        /// </summary>
        /// <param name="message">A message to log.</param>
        /// <param name="token">A cancellation instruction.</param>
        /// <returns>A task representing a possibly asynchronous operation.</returns>
        Task LogAsync(IMessage message, CancellationToken token);
    }
}