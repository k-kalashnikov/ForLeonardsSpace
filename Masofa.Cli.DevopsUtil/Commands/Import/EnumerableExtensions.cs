using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.Cli.DevopsUtil.Commands.Import
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<List<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
        {
            if (chunkSize <= 0)
                throw new ArgumentException("Chunk size must be positive.", nameof(chunkSize));

            using var enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var chunk = new List<T>(chunkSize);
                do
                {
                    chunk.Add(enumerator.Current);
                } while (chunk.Count < chunkSize && enumerator.MoveNext());

                yield return chunk;
            }
        }
    }
}
