using System.Runtime.CompilerServices;

namespace Nerdshoe.Services
{
    public interface ILogService
    {
        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <returns>The logger.</returns>
        /// <param name="logName">Log name.</param>
        ILogger GetLogger([CallerFilePath]string logName = null);
    }
}
