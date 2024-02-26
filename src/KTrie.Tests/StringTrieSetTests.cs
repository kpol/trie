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
        string[] input = { "abc", "abde" };
        var stringTrieSet = new StringTrieSet();
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
        string[] input = {"abc", "abde", "abx", "abxx"};
        var stringTrieSet = new StringTrieSet();
        stringTrieSet.AddRange(input);

        var result = stringTrieSet.GetByPrefix(prefix).OrderBy(s => s);

        Assert.Equal(found.OrderBy(s => s), result);
    }
}