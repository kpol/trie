using System.Collections.Generic;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace KTrie.TestBenchmark;

public class StringTrieTest
{
    private readonly string[] _words;
    private readonly StringTrieSet _stringTrie;
    private readonly ILookup<char, string> _wordGroups;

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
    };

    public StringTrieTest()
    {
        _words = GetWords();

        _stringTrie = new StringTrieSet();
        _stringTrie.AddRange(_words);

        _wordGroups = PreprocessWords();
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
        var result = new HashSet<string>();

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

    private static string[] GetWords() => File.ReadAllLines("TestData/vocabulary.txt");

    private ILookup<char, string> PreprocessWords()
    {
        var words = _words
            .ToLookup(w => w[0]);

        return words;
    }
}