using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace KTrie.Tests;

public class TrieSetTests
{
    [Fact]
    public void AddNullKey()
    {
        Assert.Throws<ArgumentNullException>(() => new TrieSet<char> {null});
    }

    [Fact]
    public void AddRange()
    {
        const int count = 1000;

        var trie = new TrieSet<char>();

        trie.AddRange(Enumerable.Range(0, count).Select(i => i.ToString()));

        Assert.Equal(count, trie.Count);
    }

    [Fact]
    public void AddWithSameKey()
    {
        Assert.Throws<ArgumentException>(() => new TrieSet<char> {"a", "a"});
    }

    [Fact]
    public void Clear()
    {
        var trie = new TrieSet<char> { "ABC", "AB", "ADE", "ABCDE" };

        trie.Clear();

        Assert.Empty(trie);
    }

    [Fact]
    public void Contains()
    {
        var trie = new TrieSet<char> { "ABC", "AB", "ADE", "ABCDE" };

        Assert.Contains("ABC", trie);
        Assert.Contains("AB", trie);
        Assert.Contains("ADE", trie);
        Assert.Contains("ABCDE", trie);

        Assert.DoesNotContain("X", trie);
        Assert.DoesNotContain("ABCD", trie);
    }

    [Fact]
    public void ContainsKey()
    {
        var trie = new TrieSet<char> { "ABC", "AB", "ADE", "ABCDE" };

        Assert.Contains("ABC", trie);
        Assert.Contains("AB", trie);
        Assert.Contains("ADE", trie);
        Assert.Contains("ABCDE", trie);

        Assert.DoesNotContain("A", trie);
        Assert.DoesNotContain("AC", trie);
        Assert.DoesNotContain("ABCD", trie);
    }

    [Fact]
    public void ContainsKeyClear()
    {
        var trie = new TrieSet<char> { "ABC", "AB", "ADE", "ABCDE" };

        trie.Clear();

        Assert.DoesNotContain("ABC", trie);
        Assert.DoesNotContain("AB", trie);
        Assert.DoesNotContain("ADE", trie);
        Assert.DoesNotContain("ABCDE", trie);
    }

    [Fact]
    public void CopyTo()
    {
        var trie = new TrieSet<char> { "ABC", "AB", "ADE", "ABCDE" };

        var destinationArray = new IEnumerable<char>[6];

        trie.CopyTo(destinationArray, 1);

        var result = destinationArray.Where(i => i != null).ToArray();

        var expected = new[]
        {
            "AB",
            "ABC",
            "ABCDE",
            "ADE"
        };

        Assert.Null(destinationArray[0]);
        Assert.Null(destinationArray[^1]);
        Assert.True(expected.SequenceEqual(result));
    }

    [Fact]
    public void GetByPrefix()
    {
        var trie = new TrieSet<char> { "ABC", "AB", "ADE", "ABCDE" };

        var result = trie.GetByPrefix("ABC").Select(t => new string(t.ToArray())).OrderBy(t => t);

        string[] expectedResult = { "ABC", "ABCDE" };

        Assert.Equal(4, trie.Count);
        Assert.True(expectedResult.SequenceEqual(result));
    }

    [Fact]
    public void GetEnumerator()
    {
        var trie = new TrieSet<char> { "ABC", "AB", "ADE", "ABCDE" };

        var result = trie.Select(t => new string(t.ToArray())).OrderBy(w => w).ToArray();

        Assert.Equal(new[] { "AB", "ABC", "ABCDE", "ADE" }, result);
    }

    [Fact]
    public void IsReadOnly()
    {
        var trie = new TrieSet<char>();

        Assert.False(((ICollection<IEnumerable<char>>)trie).IsReadOnly);
    }

    [Fact]
    public void Remove()
    {
        var trie = new TrieSet<char> { "ABC", "AB", "ADE", "ABCDE", "X" };

        Assert.False(trie.Remove("XY"));
        Assert.False(trie.Remove("ABCD"));
        Assert.Equal(5, trie.Count);
        Assert.True(trie.Remove("ABCDE"));
        Assert.Equal(4, trie.Count);
        Assert.True(trie.Remove("X"));
        Assert.Equal(3, trie.Count);
        Assert.True(trie.Remove("ABC"));
        Assert.Equal(2, trie.Count);
        Assert.DoesNotContain("ABC", trie);
        Assert.Contains("AB", trie);
        Assert.Contains("ADE", trie);
    }

    [Fact]
    public void RemoveNotExists()
    {
        var trie = new TrieSet<char> { "ABC" };

        Assert.False(trie.Remove("A"));
        Assert.False(trie.Remove("X"));
    }

    [Fact]
    public void RemoveNullKey()
    {
        var trie = new TrieSet<char> { "ABC" };

        Assert.Throws<ArgumentNullException>(() => trie.Remove(null));
    }
}