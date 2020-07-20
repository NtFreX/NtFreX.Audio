using System;

namespace NtFreX.Audio.AdapterInfrastructure
{
    public interface IPlaybackContext : IDisposable 
    {
        Observable<EventArgs> EndOfDataReached { get; }
        Observable<EventArgs> EndOfPositionReached { get; }
        Observable<EventArgs<double>> PositionChanged { get; }
    }
}
