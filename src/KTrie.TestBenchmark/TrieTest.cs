using System.Collections.Generic;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace KTrie.TestBenchmark
{
    public class TrieTest
    {
        private readonly string[] _words;
        private readonly Trie<bool> _trie;

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

        public TrieTest()
        {
            _words = GetWords();
            _trie = new Trie<bool>();
            _trie.AddRange(_words.Select(w => new StringEntry<bool>(w, false)));
        }

        [Benchmark]
        public ICollection<string> Trie()
        {
            var result = new List<string>();

            foreach (var prefix in _prefixes)
            {
                result.AddRange(_trie.GetByPrefix(prefix).Select(s => s.Key));
            }

            return result;
        }

        [Benchmark]
        public ICollection<string> LinqStartsWith()
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