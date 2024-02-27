using System.Linq;
using Xunit;

namespace KTrie.Tests;

public class StringTrieSetTests
{
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
        string[] input = ["abc", "abde"];
        var stringTrieSet = new StringTrieSet2();
        stringTrieSet.AddRange(input);

        Assert.Equal(foundExpected, stringTrieSet.Contains(lookup));
    }

    [Theory]
    [InlineData("a", new[]{"abc", "abde", "abx", "abxx"})]
    [InlineData("x", new string[0])]
    [InlineData("ab", new[] { "abc", "abde", "abx", "abxx" })]
    [InlineData("abc", new[] { "abc"})]
    [InlineData("abcc", new string[0])]
    [InlineData("abx", new[] { "abx", "abxx" })]
    public void GetByPrefix(string prefix, string[] found)
    {
        string[] input = ["abc", "abde", "abx", "abxx"];
        var stringTrieSet = new StringTrieSet2();
        stringTrieSet.AddRange(input);

        var result = stringTrieSet.GetByPrefix(prefix).OrderBy(s => s);

        Assert.Equal(found.OrderBy(s => s), result);
    }

    [Fact]
    public void Remove()
    {
        StringTrieSet2 trie = ["a", "ab", "abc"];

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
        StringTrieSet2 trie = ["a", "ab", "abc", "abd"];

        Assert.Equal(4, trie.Count);

        Assert.True(trie.Remove("ab"));
        Assert.False(trie.Remove("ab"));

        Assert.Equal(3, trie.Count);
        Assert.Contains("a", trie);
        Assert.Contains("abc", trie);
        Assert.Contains("abd", trie);
        Assert.DoesNotContain("ab", trie);
    }
}