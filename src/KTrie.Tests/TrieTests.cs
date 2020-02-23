using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace KTrie.Tests
{
    public class TrieTests
    {
        [Fact]
        public void AddWithSameKey()
        {
            Assert.Throws<ArgumentException>(() =>
                new Trie<char, bool> {{"a", false}, {"a", true}});
        }

        [Fact]
        public void Clear()
        {
            var trie = new Trie<char, bool> { { "ABC", false }, { "AB", false }, { "ADE", false }, { "ABCDE", false } };

            trie.Clear();

            Assert.Empty(trie);
        }

        [Fact]
        public void Contains()
        {
            var trie = new Trie<char, bool> { { "ABC", false }, { "AB", false }, { "ADE", true }, { "ABCDE", false } };

            var t = (IDictionary<IEnumerable<char>, bool>)trie;

            Assert.True(t.Contains(new KeyValuePair<IEnumerable<char>, bool>("ABC", false)));
            Assert.True(t.Contains(new KeyValuePair<IEnumerable<char>, bool>("AB", false)));
            Assert.True(t.Contains(new KeyValuePair<IEnumerable<char>, bool>("ADE", true)));
            Assert.True(t.Contains(new KeyValuePair<IEnumerable<char>, bool>("ABCDE", false)));

            Assert.False(t.Contains(new KeyValuePair<IEnumerable<char>, bool>("X", false)));
            Assert.False(t.Contains(new KeyValuePair<IEnumerable<char>, bool>("ADE", false)));
            Assert.False(t.Contains(new KeyValuePair<IEnumerable<char>, bool>("ABCD", false)));
        }

        [Fact]
        public void ContainsKey()
        {
            var trie = new Trie<char, bool> { { "ABC", false }, { "AB", false }, { "ADE", false }, { "ABCDE", false } };

            Assert.True(trie.ContainsKey("ABC"));
            Assert.True(trie.ContainsKey("AB"));
            Assert.True(trie.ContainsKey("ADE"));
            Assert.True(trie.ContainsKey("ABCDE"));

            Assert.False(trie.ContainsKey("A"));
            Assert.False(trie.ContainsKey("AC"));
            Assert.False(trie.ContainsKey("ABCD"));
        }

        [Fact]
        public void ContainsKeyClear()
        {
            var trie = new Trie<char, bool> { { "ABC", false }, { "AB", false }, { "ADE", false }, { "ABCDE", false } };

            trie.Clear();

            Assert.False(trie.ContainsKey("ABC"));
            Assert.False(trie.ContainsKey("AB"));
            Assert.False(trie.ContainsKey("ADE"));
            Assert.False(trie.ContainsKey("ABCDE"));
        }

        [Fact]
        public void CopyTo()
        {
            var trie = new StringTrie<bool> { { "ABC", true }, { "AB", false }, { "ADE", true }, { "ABCDE", false } };


            var destinationArray = new KeyValuePair<string, bool>[6];

            ((ICollection<KeyValuePair<string, bool>>)trie).CopyTo(destinationArray, 1);

            var result = destinationArray.Where(i => i.Key != null).OrderBy(i => i.Key).ToArray();

            var expected = new[]
                {
                    new KeyValuePair<string, bool>("AB", false),
                    new KeyValuePair<string, bool>("ABC", true),
                    new KeyValuePair<string, bool>("ABCDE", false),
                    new KeyValuePair<string, bool>("ADE", true)
                };

            Assert.Equal(new KeyValuePair<string, bool>(), destinationArray[0]);
            Assert.Equal(new KeyValuePair<string, bool>(), destinationArray[^1]);
            Assert.True(expected.SequenceEqual(result));
        }

        [Fact]
        public void GetByPrefix()
        {
            var trie = new Trie<char, int> { { "ABC", 1 }, { "AB", 2 }, { "ABCDE", 3 } };
            ((IDictionary<IEnumerable<char>, int>)trie).Add(new KeyValuePair<IEnumerable<char>, int>("ADE", 4));


            var result = trie.GetByPrefix("ABC").ToArray();
            var keys = result.Select(i => new string(i.Key.ToArray()));
            var values = result.Select(i => i.Value);

            string[] expectedResultKeys = { "ABC", "ABCDE" };
            int[] expectedResultValues = { 1, 3 };

            Assert.Equal(4, trie.Count);
            Assert.True(expectedResultKeys.SequenceEqual(keys));
            Assert.True(expectedResultValues.SequenceEqual(values));
        }

        [Fact]
        public void GetEnumerator()
        {
            var trie = new Trie<char, bool> { { "ABC", false }, { "AB", false }, { "ADE", false }, { "ABCDE", false } };


            var result = trie.Select(kvp => new string(kvp.Key.ToArray())).OrderBy(w => w).ToArray();
            var resultEnumerator =
                trie.OfType<KeyValuePair<IEnumerable<char>, bool>>().Select(k => new string(k.Key.ToArray())).OrderBy(w => w).ToArray();

            Assert.Equal(new[] { "AB", "ABC", "ABCDE", "ADE" }, result);
            Assert.Equal(new[] { "AB", "ABC", "ABCDE", "ADE" }, resultEnumerator);
        }

        [Fact]
        public void IsReadOnly()
        {
            var trie = new Trie<char, bool>();

            Assert.False(((IDictionary<IEnumerable<char>, bool>)trie).IsReadOnly);
        }

        [Fact]
        public void ItemsGet()
        {
            var trie = new Trie<char, bool> { { "ABC", false }, { "AB", false }, { "ADE", true }, { "ABCDE", false } };

            Assert.False(trie["ABC"]);
            Assert.False(trie["AB"]);
            Assert.True(trie["ADE"]);
            Assert.False(trie["ABCDE"]);
        }

        [Fact]
        public void ItemsGetException()
        {
            var trie = new Trie<char, bool> { { "ABC", false }, { "AB", false }, { "ADE", true }, { "ABCDE", false } };

            Assert.Throws<KeyNotFoundException>(() => trie["A"]);
        }

        [Fact]
        public void ItemsSet()
        {
            var trie = new Trie<char, bool> { ["ABC"] = true };

            Assert.True(trie["ABC"]);

            trie["AB"] = true;

            Assert.True(trie["AB"]);

            trie["AB"] = false;

            Assert.False(trie["AB"]);
        }

        [Fact]
        public void KeysValues()
        {
            var trie = new Trie<char, bool> { { "ABC", false }, { "AB", true }, { "ADE", false }, { "ABCDE", true } };

            Assert.True(new[] { "AB", "ABC", "ABCDE", "ADE" }.SequenceEqual(trie.Keys.Select(i => new string(i.ToArray())).OrderBy(s => s)));
            Assert.True(new[] { false, false, true, true }.SequenceEqual(trie.Values.OrderBy(s => s)));
        }

        [Fact]
        public void Remove()
        {
            const int initialCount = 5;

            var trie = new Trie<char, bool>
                {
                    { "ABC", false }, { "AB", false }, { "ADE", false }, { "ABCDE", false }, { "X", false }
                };

            Assert.False(((IDictionary<IEnumerable<char>, bool>)trie).Remove(new KeyValuePair<IEnumerable<char>, bool>("XY", true)));
            Assert.False(((IDictionary<IEnumerable<char>, bool>)trie).Remove(new KeyValuePair<IEnumerable<char>, bool>("ABCD", true)));
            Assert.False(((IDictionary<IEnumerable<char>, bool>)trie).Remove(new KeyValuePair<IEnumerable<char>, bool>("ABCDE", true)));
            Assert.Equal(initialCount, trie.Count);
            Assert.True(((IDictionary<IEnumerable<char>, bool>)trie).Remove(new KeyValuePair<IEnumerable<char>, bool>("ABCDE", false)));
            Assert.Equal(initialCount - 1, trie.Count);
            Assert.True(trie.Remove("X"));
            Assert.Equal(initialCount - 2, trie.Count);
            Assert.True(trie.Remove("ABC"));
            Assert.Equal(initialCount - 3, trie.Count);
            Assert.False(trie.ContainsKey("ABC"));
            Assert.True(trie.ContainsKey("AB"));
            Assert.True(trie.ContainsKey("ADE"));
        }

        [Fact]
        public void RemoveNotExists()
        {
            var trie = new Trie<char, bool> { { "ABC", false } };

            Assert.False(trie.Remove("A"));
            Assert.False(trie.Remove("X"));
        }

        [Fact]
        public void RemoveNullKey()
        {
            var trie = new Trie<char, bool> { { "ABC", false } };

            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => trie.Remove(null));
        }

        [Fact]
        public void TryGetValue()
        {
            const string expectedValue = "value";

            var trie = new Trie<char, string> { { "ABC", expectedValue } };

            Assert.True(trie.TryGetValue("ABC", out var value));
            Assert.Equal(expectedValue, value);
            Assert.False(trie.TryGetValue("A", out value));
            Assert.Null(value);
        }

        [Fact]
        public void TryGetValueKeyIsNull()
        {
            var trie = new Trie<char, bool> { { "ABC", false } };

            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once AssignNullToNotNullAttribute
                trie.TryGetValue(null, out _));
        }
    }
}