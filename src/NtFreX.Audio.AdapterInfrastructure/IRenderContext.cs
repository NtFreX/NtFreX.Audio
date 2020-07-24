using System;

namespace NtFreX.Audio.AdapterInfrastructure
{
    public interface IRenderContext : IDisposable 
    {
        Observable<EventArgs> EndOfDataReached { get; }
        Observable<EventArgs> EndOfPositionReached { get; }
        Observable<EventArgs<double>> PositionChanged { get; }

#pragma warning disable CA1716 // Identifiers should not match keywords
        void Stop();
#pragma warning restore CA1716 // Identifiers should not match keywords
        void Start();

        TimeSpan GetPosition();
    }
}
