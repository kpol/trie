using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlgoLib.Tests
{
    [TestClass]
    public class TrieTestsBenchmark
    {
        private const string VocabularyPath = "vocabulary.txt";

        private static readonly string[] Prefixes =
            {
                "AB", "DE", "CX", "MR", "MA", "RE", "ART", "ZO", "P", "RAND", 
                "PO", "ENT", "LA", "AR", "QU", "TU", "OP", "S", "NI", "SKU", 
                "AN", "RE", "LI", "SORR", "CERT", "CAT", "DOG", "ANIMA", "TE", "ZA"
            };

        public TestContext TestContext { get; set; }

        [TestMethod]
        [Ignore]
        public void BenchmarkTest()
        {
            const int Count = 1;

            var words = GetWords();

            TestContext.WriteLine(
                "Words count: {0}. Iterations count: {1}. Prefixes count: {2}.", words.Count(), Count, Prefixes.Length);

            var stopWatch = Stopwatch.StartNew();

            for (int i = 0; i < Count; i++)
            {
                foreach (var prefix in Prefixes)
                {
                    var resultArray = words.Where(w => w.StartsWith(prefix)).ToArray();
                }
            }

            stopWatch.Stop();

            TestContext.WriteLine("ToArray method: {0}", stopWatch.ElapsedMilliseconds);

            stopWatch.Restart();

            var trie = new Trie<bool>();
            trie.AddRange(words.Select(w => new StringEntry<bool>(w, false)));

            stopWatch.Stop();

            TestContext.WriteLine("Build tree: {0}", stopWatch.ElapsedMilliseconds);

            stopWatch.Restart();

            for (int i = 0; i < Count; i++)
            {
                foreach (var prefix in Prefixes)
                {
                    var resultTrie = trie.GetByPrefix(prefix).Select(w => w.Key).ToArray();
                }
            }

            stopWatch.Stop();

            TestContext.WriteLine("Trie find prefixes: {0}", stopWatch.ElapsedMilliseconds);
        }

        [TestMethod]
        [Ignore]
        public void CheckTest()
        {
            var words = GetWords();

            var trie = new Trie<bool>();
            trie.AddRange(words.Select(w => new StringEntry<bool>(w, false)));

            foreach (var prefix in Prefixes)
            {
                var result1 = words.Where(w => w.StartsWith(prefix));
                var result2 = trie.GetByPrefix(prefix).Select(t => t.Key).OrderBy(w => w);

                Assert.IsTrue(result1.SequenceEqual(result2));
            }
        }

        /// <summary>
        /// Finds <see cref="string"/> items by its prefix.
        /// </summary>
        /// <param name="sortedSet"><see cref="SortedSet{T}"/> of items.</param>
        /// <param name="prefix">Prefix.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input <see cref="SortedSet{T}"/> that start with <paramref name="prefix"/>.</returns>
        private static IEnumerable<string> FindPrefix(SortedSet<string> sortedSet, string prefix)
        {
            foreach (var word in sortedSet.GetViewBetween(prefix, sortedSet.Max))
            {
                if (word.StartsWith(prefix))
                {
                    yield return word;
                }
            }
        }

        /// <summary>
        /// Returns distinct set of words. <remarks>This method returns 58110 English words.</remarks>
        /// </summary>
        /// <returns>Distinct set of words.</returns>
        private IEnumerable<string> GetWords()
        {
            var path = Path.Combine(TestContext.DeploymentDirectory, VocabularyPath);
            return File.ReadAllLines(path);
        }
    }
}
