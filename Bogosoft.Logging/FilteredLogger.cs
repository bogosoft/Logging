using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bogosoft.Logging
{
    class FilteredLogger : ILogger
    {
        readonly Func<IMessage, bool> condition;
        readonly ILogger logger;

        internal FilteredLogger(ILogger logger, Func<IMessage, bool> condition)
        {
            this.condition = condition;
            this.logger = logger;
        }

        public void Log(IMessage message)
        {
            if (condition.Invoke(message))
            {
                logger.Log(message);
            }
        }

        public Task LogAsync(IMessage message, CancellationToken token)
        {
            if (condition.Invoke(message))
            {
                return logger.LogAsync(message, token);
            }
            else
            {
#if NET45
                return Task.FromResult(0);
#else
                return Task.CompletedTask;
#endif
            }
        }
    }
}