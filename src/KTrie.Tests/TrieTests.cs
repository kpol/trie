using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace KTrie.Tests;

public class TrieTests
{
    private static readonly string[] Words = GetWords();
    
    [Theory]
    [InlineData("a", false)]
    [InlineData("ab", false)]
    [InlineData("abc", true)]
    [InlineData("abcd", false)]
    [InlineData("abde", true)]
    [InlineData("abce", false)]
    [InlineData("x", false)]
    public void Contains(string lookup, bool foundExpected)
    {
        Trie trie = ["abc", "abde"];

        Assert.Equal(foundExpected, trie.Contains(lookup));
    }

    [Theory]
    [InlineData("a", new[] { "abc", "abde", "abx", "abxx" })]
    [InlineData("x", new string[0])]
    [InlineData("ab", new[] { "abc", "abde", "abx", "abxx" })]
    [InlineData("abc", new[] { "abc" })]
    [InlineData("abcc", new string[0])]
    [InlineData("abx", new[] { "abx", "abxx" })]
    public void GetByPrefix(string prefix, string[] found)
    {
        Trie trie = ["abc", "abde", "abx", "abxx"];

        var result = trie.StartsWith(prefix).OrderBy(s => s);

        Assert.Equal(found.OrderBy(s => s), result);
    }

    [Fact]
    public void Remove()
    {
        Trie trie = ["a", "ab", "abc"];

        Assert.Equal(3, trie.Count);

        Assert.True(trie.Remove("a"));
        Assert.False(trie.Remove("a"));

        Assert.Equal(2, trie.Count);
        Assert.Contains("ab", trie);
        Assert.Contains("abc", trie);
        Assert.DoesNotContain("a", trie);
    }

    [Fact]
    public void Remove2()
    {
        Trie trie = ["a", "ab", "abc", "abd"];

        Assert.Equal(4, trie.Count);

        Assert.True(trie.Remove("ab"));
        Assert.False(trie.Remove("ab"));

        Assert.Equal(3, trie.Count);
        Assert.Contains("a", trie);
        Assert.Contains("abc", trie);
        Assert.Contains("abd", trie);
        Assert.DoesNotContain("ab", trie);
    }

    [Fact]
    public void Remove3()
    {
        Trie trie = ["abc"];

        Assert.Single(trie);

        Assert.True(trie.Remove("abc"));
        Assert.False(trie.Remove("abc"));

        Assert.Empty(trie);
        Assert.DoesNotContain("abc", trie);

        trie.Add("abc");

        Assert.Single(trie);
        Assert.Equal(1, trie.Count);
        Assert.Contains("abc", trie);
    }

    [Fact]
    public void Remove4()
    {
        Trie trie = ["abc", "abcd"];

        Assert.Equal(2, trie.Count);

        Assert.True(trie.Remove("abc"));
        Assert.Contains("abcd", trie);
        Assert.DoesNotContain("abc", trie);

        Assert.Single(trie);
        Assert.Equal(1, trie.Count);
    }

    [Fact]
    public void Remove5()
    {
        const int initialCount = 5;

        Trie trie = ["abc", "ab", "ade", "abcde", "x"];

        Assert.False(trie.Remove("xy"));
        Assert.False(trie.Remove("abcd"));

        Assert.Equal(initialCount, trie.Count);

        Assert.True(trie.Remove("abcde"));
        Assert.Equal(initialCount - 1, trie.Count);
        Assert.True(trie.Remove("x"));
        Assert.Equal(initialCount - 2, trie.Count);
        Assert.True(trie.Remove("abc"));
        Assert.Equal(initialCount - 3, trie.Count);
        Assert.DoesNotContain("abc", trie);
        Assert.Contains("ab", trie);
        Assert.Contains("ade", trie);
    }

    [Fact]
    public void Add()
    {
        Trie trie = ["abc", "abc"];

        Assert.Single(trie);
        Assert.Equal(1, trie.Count);
    }

    [Fact]
    public void Add2()
    {
        Trie trie = ["abcd"];

        Assert.Single(trie);
        Assert.Equal(1, trie.Count);

        trie.Add("abc");
        Assert.Equal(2, trie.Count);

        Assert.Contains("abcd", trie);
        Assert.Contains("abc", trie);

        Assert.True(trie.Remove("abc"));

        Assert.Single(trie);
        Assert.Equal(1, trie.Count);
        Assert.Contains("abcd", trie);
    }

    [Fact]
    public void GetByPrefix2()
    {
        Trie trie = ["abc", "abcd"];

        var result = trie.StartsWith("abc").Order().ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal(["abc", "abcd"], result);
    }

    [Fact]
    public void GetByPrefix_Vocabulary()
    {
        Trie trie = [.. Words];

        var result = trie.StartsWith("sc").ToHashSet();
        var startsWithResult = Words.Where(w => w.StartsWith("sc")).ToHashSet();

        Assert.True(startsWithResult.SetEquals(result));
    }

    [Fact]
    public void Pattern_Vocabulary()
    {
        Trie trie = [..Words];

        var result = trie.Matches([Character.Any, 'c', Character.Any, Character.Any, 't']).ToHashSet();
        var regexResult = Words.Where(w => Regex.IsMatch(w, "^.c.{2}t$")).ToHashSet();

        Assert.True(regexResult.SetEquals(result));
    }

    [Fact]
    public void PrefixPattern_Vocabulary()
    {
        Trie trie = [.. Words];

        var result = trie.StartsWith([Character.Any, 'c', Character.Any, Character.Any, 't']).ToHashSet();
        var regexResult = Words.Where(w => Regex.IsMatch(w, "^.c.{2}t")).ToHashSet();

        Assert.True(regexResult.SetEquals(result));
    }

    [Fact]
    public void Clear()
    {
        Trie trie = ["abc", "abcd"];

        trie.Clear();

        Assert.Equal(0, trie.Count);
        Assert.Empty(trie);
    }

    private static string[] GetWords() => File.ReadAllLines("TestData/vocabulary.txt");
}