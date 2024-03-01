using System.Collections.Generic;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace KTrie.TestBenchmark;

[MemoryDiagnoser(false)]
public class StringTrieTest
{
    private readonly string[] _words;
    private readonly ILookup<char, string> _wordGroups;
    private readonly Dictionary<string, List<string>> _dictWithAllPrefixes;
    private readonly Trie _trie;
    private readonly TrieDictionary<int> _trieDictionary;


    private readonly string[] _prefixes =
    [
        "ABC",
        "K",
        "HELLO",
        "WORLD",
        "PR",
        "AB",
        "LO",
        "ST",
        "TOM",
        "TR",
        "MOR",
        "X",
        "TRE",
        "SE",
        "GO",
        "VI",
        "GRE",
        "POL",
        "KIR",
        "VE"
    ];

    public StringTrieTest()
    {
        _words = GetWords();
        _wordGroups = PreprocessWords();
        _dictWithAllPrefixes = LoadAllPrefixes();

        _trie = [.. _words];
        _trieDictionary = [];

        for (int i = 0; i < _words.Length; i++)
        {
            _trieDictionary.Add(_words[i], i);
        }
    }

    [Benchmark]
    public ICollection<string> Trie_GetByPrefix()
    {
        HashSet<string> result = [];

        foreach (var prefix in _prefixes)
        {
            foreach (var res in _trie.GetByPrefix(prefix))
            {
                result.Add(res);
            }
        }

        return result;
    }

    [Benchmark]
    public ICollection<string> Linq_StartsWith()
    {
        HashSet<string> result = [];

        foreach (var prefix in _prefixes)
        {
            var firstLetter = prefix[0];
            foreach (var word in _wordGroups[firstLetter].Where(w => w.StartsWith(prefix)))
            {
                result.Add(word);
            }
        }

        return result;
    }

    [Benchmark]
    public ICollection<string> Linq_DictionaryWithAllPrefixes()
    {
        HashSet<string> result = [];

        foreach (var prefix in _prefixes)
        {
            if (_dictWithAllPrefixes.TryGetValue(prefix, out var words))
            {
                foreach (var word in words)
                {
                    result.Add(word);
                }
            }
        }

        return result;
    }

    [Benchmark]
    public ICollection<(string, int)> TrieDictionary_GetByPrefix()
    {
        HashSet<(string, int)> result = [];

        foreach (var prefix in _prefixes)
        {
            foreach (var res in _trieDictionary.GetByPrefix(prefix))
            {
                result.Add((res.Key, res.Value));
            }
        }

        return result;
    }

    private static string[] GetWords() => File.ReadAllLines("TestData/vocabulary.txt");

    private ILookup<char, string> PreprocessWords()
    {
        var words = _words
            .ToLookup(w => w[0]);

        return words;
    }

    private Dictionary<string, List<string>> LoadAllPrefixes()
    {
        Dictionary<string, List<string>> d = [];

        foreach (var word in _words)
        {
            for (int i = 1; i <= word.Length; i++)
            {
                var prefix = word[..i];

                if (d.TryGetValue(prefix, out var value))
                {
                    value.Add(word);
                }
                else
                {
                    d[prefix] = [word];
                }
            }
        }

        return d;
    }
}