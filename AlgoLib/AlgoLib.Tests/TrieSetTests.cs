using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlgoLib.Tests
{
    [TestClass]
    public class TrieSetTests
    {
        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void AddNullKey()
        {
            // ReSharper disable once UnusedVariable
            var trie = new TrieSet<char> { null };
        }

        [TestMethod]
        public void AddRange()
        {
            const int count = 1000;

            var trie = new TrieSet<char>();

            trie.AddRange(Enumerable.Range(0, count).Select(i => i.ToString()));

            Assert.AreEqual(count, trie.Count);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void AddWithSameKey()
        {
            // ReSharper disable once UnusedVariable
            var trie = new TrieSet<char> { "a", "a" };
        }

        [TestMethod]
        public void Clear()
        {
            var trie = new TrieSet<char> { "ABC", "AB", "ADE", "ABCDE" };

            trie.Clear();

            Assert.AreEqual(0, trie.Count);
        }

        [TestMethod]
        public void Contains()
        {
            var trie = new TrieSet<char> { "ABC", "AB", "ADE", "ABCDE" };

            Assert.IsTrue(trie.Contains("ABC"));
            Assert.IsTrue(trie.Contains("AB"));
            Assert.IsTrue(trie.Contains("ADE"));
            Assert.IsTrue(trie.Contains("ABCDE"));

            Assert.IsFalse(trie.Contains("X"));
            Assert.IsFalse(trie.Contains("ABCD"));
        }

        [TestMethod]
        public void ContainsKey()
        {
            var trie = new TrieSet<char> { "ABC", "AB", "ADE", "ABCDE" };

            Assert.IsTrue(trie.Contains("ABC"));
            Assert.IsTrue(trie.Contains("AB"));
            Assert.IsTrue(trie.Contains("ADE"));
            Assert.IsTrue(trie.Contains("ABCDE"));

            Assert.IsFalse(trie.Contains("A"));
            Assert.IsFalse(trie.Contains("AC"));
            Assert.IsFalse(trie.Contains("ABCD"));
        }

        [TestMethod]
        public void ContainsKeyClear()
        {
            var trie = new TrieSet<char> { "ABC", "AB", "ADE", "ABCDE" };

            trie.Clear();

            Assert.IsFalse(trie.Contains("ABC"));
            Assert.IsFalse(trie.Contains("AB"));
            Assert.IsFalse(trie.Contains("ADE"));
            Assert.IsFalse(trie.Contains("ABCDE"));
        }

        [TestMethod]
        public void CopyTo()
        {
            var trie = new TrieSet<char> { "ABC", "AB", "ADE", "ABCDE" };

            var destinationArray = new IEnumerable<char>[6];

            ((ICollection<IEnumerable<char>>)trie).CopyTo(destinationArray, 1);

            var result = destinationArray.Where(i => i != null).ToArray();

            var expected = new[]
                {
                    "AB",
                    "ABC",
                    "ABCDE",
                    "ADE"
                };

            Assert.AreEqual(null, destinationArray[0]);
            Assert.AreEqual(null, destinationArray[destinationArray.Length - 1]);
            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestMethod]
        public void GetByPrefix()
        {
            var trie = new TrieSet<char> { "ABC", "AB", "ADE", "ABCDE" };

            var result = trie.GetByPrefix("ABC").Select(t => new string(t.ToArray())).OrderBy(t => t);

            string[] expectedResult = { "ABC", "ABCDE" };

            Assert.AreEqual(4, trie.Count);
            Assert.IsTrue(expectedResult.SequenceEqual(result));
        }

        [TestMethod]
        public void GetEnumerator()
        {
            var trie = new TrieSet<char> { "ABC", "AB", "ADE", "ABCDE" };

            var result = trie.Select(t => new string(t.ToArray())).OrderBy(w => w).ToArray();

            CollectionAssert.AreEqual(new[] { "AB", "ABC", "ABCDE", "ADE" }, result);
        }

        [TestMethod]
        public void IsReadOnly()
        {
            var trie = new TrieSet<char>();

            Assert.IsFalse(((ICollection<IEnumerable<char>>)trie).IsReadOnly);
        }

        [TestMethod]
        public void Remove()
        {
            var trie = new TrieSet<char> { "ABC", "AB", "ADE", "ABCDE", "X" };

            Assert.IsFalse(trie.Remove("XY"));
            Assert.IsFalse(trie.Remove("ABCD"));
            Assert.AreEqual(5, trie.Count);
            Assert.IsTrue(trie.Remove("ABCDE"));
            Assert.AreEqual(4, trie.Count);
            Assert.IsTrue(trie.Remove("X"));
            Assert.AreEqual(3, trie.Count);
            Assert.IsTrue(trie.Remove("ABC"));
            Assert.AreEqual(2, trie.Count);
            Assert.IsFalse(trie.Contains("ABC"));
            Assert.IsTrue(trie.Contains("AB"));
            Assert.IsTrue(trie.Contains("ADE"));
        }

        [TestMethod]
        public void RemoveNotExists()
        {
            // ReSharper disable once CollectionNeverQueried.Local
            var trie = new TrieSet<char> { "ABC" };

            Assert.IsFalse(trie.Remove("A"));
            Assert.IsFalse(trie.Remove("X"));
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void RemoveNullKey()
        {
            // ReSharper disable once CollectionNeverQueried.Local
            var trie = new TrieSet<char> { "ABC" };

            trie.Remove(null);
        }
    }
}