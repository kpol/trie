using System.Collections.Generic;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace KTrie.TestBenchmark
{
    public class StringTrieTest
    {
        private readonly string[] _words;
        private readonly StringTrieSet _stringTrie;

        private readonly string[] _prefixes =
        {
            "ABC",
            "K",
            "HELLO",
            "WORLD",
            "PR",
            "AB",
            "LO",
            "ST",
            "TOM",
            "TR"
        };

        public StringTrieTest()
        {
            _words = GetWords();
            _stringTrie = new StringTrieSet();
            _stringTrie.AddRange(_words);
        }

        [Benchmark]
        public ICollection<string> Trie_GetByPrefix()
        {
            var result = new List<string>();

            foreach (var prefix in _prefixes)
            {
                result.AddRange(_stringTrie.GetByPrefix(prefix));
            }

            return result;
        }

        [Benchmark]
        public ICollection<string> Linq_StartsWith()
        {
            var result = new List<string>();

            foreach (var prefix in _prefixes)
            {
                result.AddRange(_words.Where(w => w.StartsWith(prefix)));
            }

            return result;
        }

        private static string[] GetWords() => File.ReadAllLines("TestData/vocabulary.txt");
    }
}