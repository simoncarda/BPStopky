using BPStopky.Services;

namespace BPStopky.Models
{
    internal class StopWatchEngine(IStopWatchService timerService)
    {
        public TimeOnly Elapsed { get; private set; }
        public event Func<Task>? OnStateChangedAsync;

        private int _timerIntervalMs = 10;
        private readonly System.Diagnostics.Stopwatch _realStopwatch = new();

        private Task StopWatchTickAsync()
        {
            Elapsed = TimeOnly.FromTimeSpan(_realStopwatch.Elapsed);
            NotifyStateChanged();
            return Task.CompletedTask;
        }

        public void StartTimer()
        {
            _realStopwatch.Start();

            _ = timerService?.Start(StopWatchTickAsync, _timerIntervalMs);
            NotifyStateChanged();
        }

        public void PauseTimer()
        {
            _realStopwatch.Stop();
            timerService?.Stop();
            NotifyStateChanged();
        }

        public void StopTimer()
        {
            _realStopwatch.Reset(); // Vynuluje reálné měření
            timerService?.Stop();
            Elapsed = TimeOnly.FromTimeSpan(TimeSpan.Zero);
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => _ = OnStateChangedAsync?.Invoke();
    }
}