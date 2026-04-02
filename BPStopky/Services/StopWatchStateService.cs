using BPStopky.Models;

namespace BPStopky.Services
{
    internal class StopWatchStateService
    {
        public StopWatchState CurrentState { get; private set; } = StopWatchState.Stopped;
        public void SetStopped() 
        {
            CurrentState = StopWatchState.Stopped;
        }
        public void SetRunning() 
        {
            CurrentState = StopWatchState.Running;
        }
        public void SetPaused() 
        {
            CurrentState = StopWatchState.Paused;
        }
    }
}
