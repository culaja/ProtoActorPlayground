using System.Threading;
using System.Threading.Tasks;

namespace Framework
{
    public static class CancellationTokenExtensions
    {
        public static async Task WaitAsync(this CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000, cancellationToken);
            }
        }
    }
}