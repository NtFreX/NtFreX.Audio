using NtFreX.Audio.Infrastructure.Threading.Extensions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace NtFreX.Audio.Tests
{

    [TestFixture]
    public class SeekableAsyncEnumerableTest
    {
        [Test]
        public void ForEachDoesNotEnumerateTheCollection()
        {
            var array = new[] { 1, 2, 3, 4, 5 };
            var collection = array.ToSeekableAsyncEnumerable();

            var count = 0;
            collection.ForEachAsync((index, value) => count++);

            Assert.AreEqual(0, count);
        }

        [Test]
        public async Task ToArrayAsyncDoesEnumerateTheCollection()
        {
            var array = new[] { 1, 2, 3, 4, 5 };
            var collection = array.ToSeekableAsyncEnumerable();

            var count = 0;
            await collection
                .ForEachAsync((index, value) => count++)
                .ToArrayAsync()
                .ConfigureAwait(false);

            Assert.AreEqual(array.Length, count);
        }

        [Test]
        public async Task CanEnumerateTwiceAsync()
        {
            var array = new[] { 1, 2, 3, 4, 5 };
            var collection = array.ToSeekableAsyncEnumerable();

            var firstCount = 0;
            await collection
                .ForEachAsync((index, value) => firstCount++)
                .ToArrayAsync()
                .ConfigureAwait(false);

            var secondCount = 0;
            await collection
                .ForEachAsync((index, value) => secondCount++)
                .ToArrayAsync()
                .ConfigureAwait(false);

            Assert.AreEqual(firstCount, secondCount);
        }
    }
}
