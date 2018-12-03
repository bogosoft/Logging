using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Bogosoft.Logging
{
    static class TextWriterExtensions
    {
        internal static Task WriteAsync(this TextWriter writer, char value, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            return writer.WriteAsync(value);
        }

        internal static Task WriteAsync(this TextWriter writer, string value, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            return writer.WriteAsync(value);
        }

        internal static Task WriteLineAsync(this TextWriter writer, string value, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            return writer.WriteLineAsync(value);
        }
    }
}