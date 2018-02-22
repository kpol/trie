using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AlgoLib
{
    public class TrieSet<T> : ICollection<IEnumerable<T>>
    {
        private readonly IEqualityComparer<T> _comparer;

        private readonly TrieNode _root;

        public TrieSet() : this(EqualityComparer<T>.Default)
        {

        }

        public TrieSet(IEqualityComparer<T> comparer)
        {
            _comparer = comparer;
            _root = new TrieNode(default(T), comparer);
        }

        /// <inheritdoc />
        public int Count { get; private set; }

        /// <inheritdoc />
        bool ICollection<IEnumerable<T>>.IsReadOnly => false;

        /// <inheritdoc />
        public IEnumerator<IEnumerable<T>> GetEnumerator()
        {
            return GetAllNodes(_root).Select(GetFullKey).GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public void Add(IEnumerable<T> key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            var node = _root;

            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var item in key)
            {
                node = AddItem(node, item);
            }

            if (node.IsTerminal)
            {
                throw new ArgumentException($"An element with the same key already exists: '{key}'", nameof(key));
            }

            node.IsTerminal = true;
            // ReSharper disable once PossibleMultipleEnumeration
            node.Item = key;
            Count++;
        }


        public void AddRange(IEnumerable<IEnumerable<T>> collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            foreach (var item in collection)
            {
                Add(item);
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            _root.Children.Clear();
            Count = 0;
        }

        /// <inheritdoc />
        public bool Contains(IEnumerable<T> item)
        {
            var node = GetNode(item);

            return node != null && node.IsTerminal;
        }

        /// <inheritdoc />
        void ICollection<IEnumerable<T>>.CopyTo(IEnumerable<T>[] array, int arrayIndex)
        {
            Array.Copy(GetAllNodes(_root).Select(GetFullKey).ToArray(), 0, array, arrayIndex, Count);
        }

        /// <inheritdoc />
        public bool Remove(IEnumerable<T> key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            var node = GetNode(key);

            if (node == null)
            {
                return false;
            }

            if (!node.IsTerminal)
            {
                return false;
            }

            RemoveNode(node);

            return true;
        }

        /// <summary>
        /// Gets an item by key.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="item">Output item.</param>
        /// <returns>true if trie contains an element with the specified key; otherwise, false.</returns>
        public bool TryGetItem(IEnumerable<T> key, out IEnumerable<T> item)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            var node = GetNode(key);
            item = null;

            if (node == null)
            {
                return false;
            }

            if (!node.IsTerminal)
            {
                return false;
            }

            item = node.Item;

            return true;
        }

        internal bool TryGetNode(IEnumerable<T> key, out TrieNode node)
        {
            node = GetNode(key);

            if (node == null)
            {
                return false;
            }

            if (!node.IsTerminal)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets items by key prefix.
        /// </summary>
        /// <param name="prefix">Key prefix.</param>
        /// <returns>Collection of <see cref="T"/> items.</returns>
        public IEnumerable<IEnumerable<T>> GetByPrefix(IEnumerable<T> prefix)
        {
            if (prefix == null) throw new ArgumentNullException(nameof(prefix));

            var node = _root;

            foreach (var item in prefix)
            {
                if (!node.Children.TryGetValue(item, out node))
                {
                    return Enumerable.Empty<IEnumerable<T>>();
                }
            }

            return GetByPrefix(node);
        }

        private static IEnumerable<TrieNode> GetAllNodes(TrieNode node)
        {
            foreach (var child in node.Children)
            {
                if (child.Value.IsTerminal)
                {
                    yield return child.Value;
                }

                foreach (var item in GetAllNodes(child.Value))
                {
                    if (item.IsTerminal)
                    {
                        yield return item;
                    }
                }
            }
        }

        private static IEnumerable<IEnumerable<T>> GetByPrefix(TrieNode node)
        {
            var stack = new Stack<TrieNode>();
            var current = node;

            while (stack.Count > 0 || current != null)
            {
                if (current != null)
                {
                    if (current.IsTerminal)
                    {
                        yield return GetFullKey(current);
                    }

                    using (var enumrator = current.Children.GetEnumerator())
                    {
                        current = enumrator.MoveNext() ? enumrator.Current.Value : null;

                        while (enumrator.MoveNext())
                        {
                            stack.Push(enumrator.Current.Value);
                        }
                    }
                }
                else
                {
                    current = stack.Pop();
                }
            }
        }

        private static IEnumerable<T> GetFullKey(TrieNode node)
        {
            //var stack = new Stack<T>();
            //stack.Push(node.Key);

            //var n = node;

            //while ((n = n.Parent) != _root)
            //{
            //    stack.Push(n.Key);
            //}

            //return stack;

            return node.Item;
        }

        private TrieNode GetNode(IEnumerable<T> key)
        {
            var node = _root;

            foreach (var item in key)
            {
                if (!node.Children.TryGetValue(item, out node))
                {
                    return null;
                }
            }

            return node;
        }

        private void RemoveNode(TrieNode node)
        {
            Remove(node);
            Count--;
        }

        private TrieNode AddItem(TrieNode node, T key)
        {
            TrieNode child;

            if (!node.Children.TryGetValue(key, out child))
            {
                child = new TrieNode(key, _comparer)
                {
                    Parent = node
                };

                node.Children.Add(key, child);
            }

            return child;
        }

        private void Remove(TrieNode node, T key)
        {
            foreach (var trieNode in node.Children)
            {
                if (_comparer.Equals(key, trieNode.Key))
                {
                    node.Children.Remove(trieNode);

                    return;
                }
            }
        }

        private void Remove(TrieNode node)
        {
            while (true)
            {
                node.IsTerminal = false;

                if (node.Children.Count == 0 && node.Parent != null)
                {
                    Remove(node.Parent, node.Key);

                    if (!node.Parent.IsTerminal)
                    {
                        node = node.Parent;
                        continue;
                    }
                }

                break;
            }
        }

        internal class TrieNode
        {
            public TrieNode(T key, IEqualityComparer<T> comparer)
            {
                Key = key;
                Children = new Dictionary<T, TrieNode>(comparer);
            }

            public bool IsTerminal { get; set; }

            public T Key { get; }

            public IEnumerable<T> Item { get; set; }

            public IDictionary<T, TrieNode> Children { get; }

            public TrieNode Parent { get; set; }
        }
    }
}