using KTrie.TrieNodes;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace KTrie;

internal static class TrieEnumerables
{
    public static DescendantTerminalNodeEnumerable GetDescendantTerminalNodes(CharTrieNode root)
        => new(root);

    public readonly struct DescendantTerminalNodeEnumerable(CharTrieNode root) : IEnumerable<TerminalCharTrieNode>
    {
        private readonly CharTrieNode _root = root;

        public readonly Enumerator GetEnumerator() => new(_root);

        IEnumerator<TerminalCharTrieNode> IEnumerable<TerminalCharTrieNode>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<TerminalCharTrieNode>
        {
            private Queue<CharTrieNode> _queue;
            private TerminalCharTrieNode? _current;

            public Enumerator(CharTrieNode root)
            {
                _queue = QueuePool<CharTrieNode>.Rent();
                _current = null;

                foreach (var child in root.Children)
                {
                    _queue.Enqueue(child);
                }
            }

            public readonly TerminalCharTrieNode Current => _current!;
            readonly object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                while (_queue.Count > 0)
                {
                    var node = _queue.Dequeue();

                    foreach (var child in node.Children)
                    {
                        _queue.Enqueue(child);
                    }

                    if (node.IsTerminal)
                    {
                        _current = (TerminalCharTrieNode)node;

                        return true;
                    }
                }

                _current = null;

                return false;
            }

            public void Reset() => throw new NotSupportedException();

            public readonly void Dispose() => QueuePool<CharTrieNode>.Return(_queue);
        }
    }

    internal static class QueuePool<T>
    {
        private static readonly ConcurrentQueue<Queue<T>> _pool = new();

        public static Queue<T> Rent()
        {
            if (_pool.TryDequeue(out var queue))
            {
                return queue;
            }

            return new Queue<T>();
        }

        public static void Return(Queue<T> queue)
        {
            queue.Clear();
            _pool.Enqueue(queue);
        }
    }
}
