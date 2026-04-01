

namespace BPStopky.Services
{
    internal partial class StopWatchTimerService : IStopWatchService
    {
        private PeriodicTimer? _timer;
        private CancellationTokenSource? _cts;

        public async Task Start(Func<Task> onTickAsync, int intervalMs)
        {
            Stop();

            ArgumentNullException.ThrowIfNull(onTickAsync);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(intervalMs);

            var cts = new CancellationTokenSource();
            var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(intervalMs));
            _cts = cts;
            _timer = timer;

            try {
                while (await timer.WaitForNextTickAsync(cts.Token)) {
                    await onTickAsync();
                }
            }
            catch (OperationCanceledException) {  }
            finally {
                if (ReferenceEquals(_timer, timer)) _timer = null;
                if (ReferenceEquals(_cts, cts)) _cts = null;

                timer.Dispose();
                cts.Dispose();
            }
        }
        public void Stop()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            _timer?.Dispose();
            _timer = null;
        }
        public void Dispose()
        {
            Stop();
        }
    }
}
