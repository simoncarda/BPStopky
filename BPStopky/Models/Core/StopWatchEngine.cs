using BPStopky.Services;

namespace BPStopky.Models.Core
{
    /// <summary>
    /// Základní logika stopek, která spravuje čas, stavy a kola.
    /// Je nezávislá na konkrétní implementaci časovače, což umožňuje 
    /// snadné testování a případné změny v budoucnu.
    /// </summary>
    internal class StopWatchEngine(IStopWatchService timerService)
    {
        public event Action? OnElapsedChanged;

        public TimeSpan Elapsed { get; private set; } = TimeSpan.Zero;
        private int _timerIntervalMs = 10;
        private readonly System.Diagnostics.Stopwatch _realStopwatch = new();

        public List<TimeSpan> Laps { get; private set; } = new();
        public StopWatchState CurrentState { get; private set; } = StopWatchState.Stopped;

        /// <summary>
        /// Logika, která bude volána periodicky časovačem. Aktualizuje uplynulý čas a 
        /// notifikace o změně stavu. Jelikož je tato metoda asynchronní, může být snadno 
        /// integrována s různými implementacemi časovače, které mohou mít různé 
        /// požadavky na asynchronní operace.
        /// </summary>
        private Task StopWatchTickAsync()
        {
            Elapsed = _realStopwatch.Elapsed;
            NotifyStateChanged();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Spustí vnitřní časomíru a začne periodicky aktualizovat uplynulý čas pomocí časovače.
        /// </summary>
        public void StartTimer()
        {
            if(CurrentState == StopWatchState.Running)
                return;

            _realStopwatch.Start();
            _ = timerService?.Start(StopWatchTickAsync, _timerIntervalMs);
            CurrentState = StopWatchState.Running;

            NotifyStateChanged();
        }

        /// <summary>
        /// Pozastaví vnitřní časomíru a zastaví aktualizace uplynulého času. 
        /// Umožňuje uživateli později pokračovat tam, kde přestal, bez ztráty již uplynulého času.
        /// </summary>
        public void PauseTimer()
        {
            if(CurrentState == StopWatchState.Paused)
                return;

            _realStopwatch.Stop();
            timerService?.Stop();
            CurrentState = StopWatchState.Paused;

            NotifyStateChanged();
        }

        /// <summary>
        /// Resetuje vnitřní časomíru a zastaví aktualizace uplynulého času. 
        /// Umožňuje uživateli začít znovu od nuly.
        /// </summary>
        public void ResetTimer()
        {
            if(CurrentState == StopWatchState.Stopped)
                return;

            _realStopwatch.Reset();
            timerService?.Stop();
            Elapsed = TimeSpan.Zero;
            CurrentState = StopWatchState.Stopped;
            Laps.Clear();

            NotifyStateChanged();
        }

        /// <summary>
        /// Uloží aktuální uplynulý čas do seznamu kol. 
        /// Umožňuje uživateli zaznamenat mezičasy.
        /// </summary>
        public void SetLap()
        {
            if(CurrentState != StopWatchState.Running)
                return;
            Laps.Add(Elapsed);
            NotifyStateChanged();
        }

        /// <summary>
        /// Notifikuje všechny posluchače, že se změnil uplynulý čas nebo stav.
        /// </summary>
        private void NotifyStateChanged() => OnElapsedChanged?.Invoke();
    }
}