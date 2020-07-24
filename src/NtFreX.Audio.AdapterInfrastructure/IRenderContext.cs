using System;

namespace NtFreX.Audio.AdapterInfrastructure
{
    public interface IRenderContext : IDisposable 
    {
        Observable<EventArgs> EndOfDataReached { get; }
        Observable<EventArgs> EndOfPositionReached { get; }
        Observable<EventArgs<double>> PositionChanged { get; }

        void Stop();
        void Start();

        TimeSpan GetPosition();
    }
}
