using Bogosoft.Cloning;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bogosoft.Logging
{
    class CorrelatedLogger : ILogger
    {
        readonly Func<IMessage, Guid> correlatedIdGenerator;
        readonly ILogger source;

        internal CorrelatedLogger(ILogger source, Func<IMessage, Guid> correlatedIdGenerator)
        {
            this.correlatedIdGenerator = correlatedIdGenerator;
            this.source = source;
        }

        public void Log(IMessage message)
        {
            var id = correlatedIdGenerator.Invoke(message);

            message = message.CloneAnd(m => m.CorrelatedId = id);

            source.Log(message);
        }

        public Task LogAsync(IMessage message, CancellationToken token)
        {
            var id = correlatedIdGenerator.Invoke(message);

            message = message.CloneAnd(m => m.CorrelatedId = id);

            return source.LogAsync(message, token);
        }
    }
}