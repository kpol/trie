using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;

namespace KTrie.TestBenchmark;

[MemoryDiagnoser(false)]
public class StringTrieTest
{
    private string[] _words;
    private ILookup<char, string> _wordGroups;
    private Dictionary<string, List<string>> _dictWithAllPrefixes;
    private Trie _trie;

    private readonly string[] _prefixes =
    [
        "abc",
        "k",
        "hello",
        "world",
        "pr",
        "ab",
        "lo",
        "st",
        "tom",
        "tr",
        "mor",
        "c",
        "tre",
        "se",
        "go",
        "vi",
        "gre",
        "pol",
        "kir",
        "ve"
    ];

    [GlobalSetup]
    public void GlobalSetup()
    {
        _words = GetWords();
        _wordGroups = PreprocessWords();
        _dictWithAllPrefixes = LoadAllPrefixes();

        _trie = [.. _words];
    }

    [Benchmark]
    public void Load_Trie()
    {
        Trie _ = [.. _words];
    }

    [Benchmark]
    public void Load_DictionaryWithAllPrefixes()
    {
        var _ = LoadAllPrefixes();
    }

    [Benchmark]
    public void Load_DictionaryGroupedByFirstLetter()
    {
        var _ = PreprocessWords().ToList();
    }

    [Benchmark]
    public ICollection<string> EnumerateByPrefix_Trie()
    {
        HashSet<string> result = [];

        foreach (var prefix in _prefixes)
        {
            foreach (var res in _trie.EnumerateByPrefix(prefix))
            {
                result.Add(res);
            }
        }

        return result;
    }

    [Benchmark]
    public ICollection<string> StartsWith_Linq()
    {
        HashSet<string> result = [];

        foreach (var prefix in _prefixes)
        {
            foreach (var word in _words.Where(w => w.StartsWith(prefix)))
            {
                result.Add(word);
            }
        }

        return result;
    }

    [Benchmark]
    public ICollection<string> StartsWith_Linq_GroupedByFirstLetter()
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
    public ICollection<string> StartsWith_DictionaryWithAllPrefixes()
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
    public ICollection<string> Trie_EnumerateMatches() =>
        _trie.EnumerateMatches([Character.Any, 'c', Character.Any, Character.Any, 't']).ToHashSet();

    [Benchmark]
    public ICollection<string> Trie_Pattern_EnumerateByPrefix() =>
        _trie.EnumerateByPrefix([Character.Any, 'c', Character.Any, Character.Any, 't']).ToHashSet();

    [Benchmark]
    public ICollection<string> String_PatternMatching() =>
        _words.Where(w => w.Length == 5 && w[1] == 'c' && w[4] == 't').ToHashSet();

    [Benchmark]
    public ICollection<string> String_PrefixPatternMatching() =>
        _words.Where(w => w.Length >= 5 && w[1] == 'c' && w[4] == 't').ToHashSet();

    [Benchmark]
    public ICollection<string> Regex_PatternMatching() =>
        _words.Where(word => Regex.IsMatch(word, "^.c.{2}t$")).ToHashSet();

    [Benchmark]
    public ICollection<string> Regex_PrefixPatternMatching() =>
        _words.Where(word => Regex.IsMatch(word, "^.c.{2}t")).ToHashSet();

    private static string[] GetWords() => File.ReadAllLines("TestData/words_alpha.txt");

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