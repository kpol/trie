using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using AlgoLib;

namespace TestApp
{
    public class Program
    {
        private const string VocabularyPath = "vocabulary.txt";

        public static void Main(string[] args)
        {
            TrieSetWork();
            TrieWork();
            //TrieSetWork();
        }

        private static void TrieWork()
        {
            const int prefixLength = 1;

            var prefixes = GetAllMatches(Enumerable.Range(65, 26).Select(i => (char)i).ToArray(), prefixLength)
                .ToArray();

            var words = GetWords().ToArray();

            Console.WriteLine(
                "Words count: {0}. Prefixes count: {1}.", words.Length, prefixes.Length);

            var stopWatch = Stopwatch.StartNew();

            foreach (var prefix in prefixes)
            {
                var resultArray = words.Where(w => w.StartsWith(prefix)).ToArray();
            }

            stopWatch.Stop();

            Console.WriteLine("ToArray method: {0}", stopWatch.ElapsedMilliseconds);

            stopWatch.Restart();

            var trie = new Trie<bool>();
            trie.AddRange(words.Select(w => new StringEntry<bool>(w, false)));

            stopWatch.Stop();

            Console.WriteLine("Build tree: {0}", stopWatch.ElapsedMilliseconds);

            stopWatch.Restart();

            int prefixesCount = 0;
            foreach (var prefix in prefixes)
            {
                var resultTrie = trie.GetByPrefix(prefix).Select(w => w.Key).ToArray();
                prefixesCount = resultTrie.Length;
            }


            stopWatch.Stop();

            Console.WriteLine("Trie find prefixes: {0}", stopWatch.ElapsedMilliseconds);
            Console.WriteLine($"Prefixes found: {prefixesCount}");
        }

        private static void TrieSetWork()
        {
            const int prefixLength = 1;

            var prefixes = GetAllMatches(Enumerable.Range(65, 26).Select(i => (char)i).ToArray(), prefixLength)
                .ToArray();

            var words = GetWords().ToArray();

            Console.WriteLine(
                "Words count: {0}. Prefixes count: {1}.", words.Length, prefixes.Length);

            var stopWatch = Stopwatch.StartNew();

            foreach (var prefix in prefixes)
            {
                var resultArray = words.Where(w => w.StartsWith(prefix)).ToArray();
            }

            stopWatch.Stop();

            Console.WriteLine("ToArray method: {0}", stopWatch.ElapsedMilliseconds);

            stopWatch.Restart();

            var trie = new TrieSet<char>();
            trie.AddRange(words);

            stopWatch.Stop();

            Console.WriteLine("Build tree: {0}", stopWatch.ElapsedMilliseconds);

            stopWatch.Restart();

            int prefixesCount = 0;

            foreach (var prefix in prefixes)
            {
                var resultTrie = trie.GetByPrefix(prefix).Select(w => w).ToArray();
                prefixesCount = resultTrie.Length;
            }


            stopWatch.Stop();

            Console.WriteLine("Trie find prefixes: {0}", stopWatch.ElapsedMilliseconds);
            Console.WriteLine($"Prefixes found: {prefixesCount}");
        }

        private static IEnumerable<string> GetAllMatches(char[] chars, int length)
        {
            var indexes = new int[length];
            var current = new char[length];

            for (int i = 0; i < length; i++)
            {
                current[i] = chars[0];
            }

            do
            {
                yield return new string(current);
            }
            while (Increment(indexes, current, chars));
        }

        private static bool Increment(int[] indexes, char[] current, char[] chars)
        {
            int position = indexes.Length - 1;

            while (position >= 0)
            {
                indexes[position]++;
                if (indexes[position] < chars.Length)
                {
                    current[position] = chars[indexes[position]];
                    return true;
                }

                indexes[position] = 0;
                current[position] = chars[0];
                position--;
            }

            return false;
        }

        /// <summary>
        /// Returns distinct set of words. <remarks>This method returns 58110 English words.</remarks>
        /// </summary>
        /// <returns>Distinct set of words.</returns>
        private static IEnumerable<string> GetWords()
        {
            var path = Path.Combine(@"..\..\..\AlgoLib.Tests\cache", VocabularyPath);
            return File.ReadAllLines(path);
        }
    }
}