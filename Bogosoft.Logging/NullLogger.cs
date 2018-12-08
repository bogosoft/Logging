using System.Threading;
using System.Threading.Tasks;

namespace Bogosoft.Logging
{
    class NullLogger : ILogger
    {
        public void Log(IMessage message)
        {
        }

        public Task LogAsync(IMessage message, CancellationToken token)
        {
#if NET45
            return Task.FromResult(0);
#else
            return Task.CompletedTask;
#endif
        }
    }
}