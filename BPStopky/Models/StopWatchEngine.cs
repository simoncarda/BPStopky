using BPStopky.Services;

namespace BPStopky.Models
{
    internal class StopWatchEngine(IStopWatchService timerService, StopWatchStateService stopWatchStateService)
    {
        public event Func<Task>? OnElapsedChangedAsync;

        public TimeSpan Elapsed { get; private set; } = TimeSpan.Zero;
        private int _timerIntervalMs = 10;
        private readonly System.Diagnostics.Stopwatch _realStopwatch = new();

        private Task StopWatchTickAsync()
        {
            Elapsed = _realStopwatch.Elapsed;
            NotifyStateChanged();
            return Task.CompletedTask;
        }

        public void StartTimer()
        {
            _realStopwatch.Start();
            _ = timerService?.Start(StopWatchTickAsync, _timerIntervalMs);
            stopWatchStateService.SetRunning();

            NotifyStateChanged();
        }

        public void PauseTimer()
        {
            _realStopwatch.Stop();
            timerService?.Stop();
            stopWatchStateService.SetPaused();

            NotifyStateChanged();
        }

        public void StopTimer()
        {
            _realStopwatch.Reset(); // Vynuluje reálné měření
            timerService?.Stop();
            Elapsed = TimeSpan.Zero;
            stopWatchStateService.SetStopped();

            NotifyStateChanged();
        }

        public void SetLap()
        {

        }

        private void NotifyStateChanged() => _ = OnElapsedChangedAsync?.Invoke();
    }
}