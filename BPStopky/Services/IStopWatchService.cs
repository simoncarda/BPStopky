using System;
using System.Collections.Generic;
using System.Text;

namespace BPStopky.Services
{
    public interface IStopWatchService : IDisposable
    {
        Task Start(Func<Task> onTickAsync, int intervalMs);
        public void Stop();
    }
}
