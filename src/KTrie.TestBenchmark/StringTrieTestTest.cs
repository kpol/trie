using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace KTrie.TestBenchmark;

[MemoryDiagnoser(false)]
public class StringTrieTest
{
    private readonly string[] _words;
    private readonly StringTrieSet _stringTrie;
    private readonly ILookup<char, string> _wordGroups;
    private readonly StringTrieSet2 _trie2;

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

        _stringTrie = [.. _words];
        _trie2 = [.. _words];
        _wordGroups = PreprocessWords();
    }

    [Benchmark]
    public ICollection<string> Trie_GetByPrefix()
    {
        var result = new HashSet<string>();

        foreach (var prefix in _prefixes)
        {
            foreach (var res in _stringTrie.GetByPrefix(prefix))
            {
                result.Add(res);
            }
        }

        return result;
    }

    [Benchmark]
    public ICollection<string> Trie_GetByPrefix2()
    {
        var result = new HashSet<string>();

        foreach (var prefix in _prefixes)
        {
            foreach (var res in _trie2.GetByPrefix(prefix))
            {
                result.Add(res);
            }
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
