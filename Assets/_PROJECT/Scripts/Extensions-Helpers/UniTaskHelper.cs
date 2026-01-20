using System.Threading;

namespace _PROJECT.Scripts.Helpers {
    public static class UniTaskHelper {
        public static CancellationToken CreateNewToken(ref CancellationTokenSource tokenSource) {
            tokenSource?.Cancel();
            tokenSource?.Dispose();
            tokenSource =  new CancellationTokenSource();
            return tokenSource.Token;
        }
    }
}
