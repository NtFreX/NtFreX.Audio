using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Sampler.Console
{
    internal interface ISample
    {
        string Name { get; }
        string Description { get; }
        Task RunAsync(CancellationToken cancellationToken);
    }
}
