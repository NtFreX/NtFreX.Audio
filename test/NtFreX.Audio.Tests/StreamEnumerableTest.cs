using NtFreX.Audio.Infrastructure.Threading.Extensions;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NtFreX.Audio.Tests
{

    [TestFixture]
    public class StreamEnumerableTest
    {
        [Test]
        public async Task CanEnumerateMemoryStreamTwiceAsync()
        {
            var collection = new[] { 1, 2, 3, 4, 5 };
            using var stream = new MemoryStream(collection.Select(Convert.ToByte).ToArray());
            var streamEnumerable = stream.ToEnumerable();

            var firstCount = 0;
            await streamEnumerable
                .ForEachAsync((index, value) => firstCount++)
                .ToArrayAsync()
                .ConfigureAwait(false);

            var secondCount = 0;
            await streamEnumerable
                .ForEachAsync((index, value) => secondCount++)
                .ToArrayAsync()
                .ConfigureAwait(false);

            Assert.AreEqual(firstCount, secondCount);
        }
    }
}
