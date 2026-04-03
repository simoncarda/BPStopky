using BPStopky.Services;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace BPStopky.Models
{
    internal class StopWatchEngine(IStopWatchService timerService)
    {
        public event Func<Task>? OnElapsedChangedAsync;

        public TimeSpan Elapsed { get; private set; } = TimeSpan.Zero;
        private int _timerIntervalMs = 10;
        private readonly System.Diagnostics.Stopwatch _realStopwatch = new();

        public List<TimeSpan> Laps { get; private set; } = new();
        public StopWatchState CurrentState { get; private set; } = StopWatchState.Stopped;

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
            CurrentState = StopWatchState.Running;

            NotifyStateChanged();
        }

        public void PauseTimer()
        {
            _realStopwatch.Stop();
            timerService?.Stop();
            CurrentState = StopWatchState.Paused;

            NotifyStateChanged();
        }

        public void ResetTimer()
        {
            _realStopwatch.Reset();
            timerService?.Stop();
            Elapsed = TimeSpan.Zero;
            CurrentState = StopWatchState.Stopped;
            Laps.Clear();

            NotifyStateChanged();
        }

        public void SetLap()
        {
            Laps.Add(Elapsed);
        }

        private void NotifyStateChanged() => _ = OnElapsedChangedAsync?.Invoke();
    }
}