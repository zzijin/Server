using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Server.NetworkModule.ConnService.ConnPool
{
    /// <summary>
    /// 有锁链表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class LockLinkedList<T> : ICollection<T>, IEnumerable<T>
    {
        LinkedList<T> _list;
        private readonly object _listLock = new object();

        public int Count
        {
            get
            {
                lock (_listLock)
                {
                    return _list.Count;
                }
            }
        }

        public bool IsReadOnly => false;

        public void Add(LinkedListNode<T> newNode)
        {
            lock (_listLock)
            {
                _list.AddLast(newNode);
            }
        }

        public void Add(T item)
        {
            lock (_listLock)
            {
                _list.AddLast(item);
            }
        }

        public LinkedListNode<T> AddLast(T value)
        {
            lock (_listLock)
            {
                return _list.AddLast(value);
            }
        }

        public LockLinkedList()
        {
            _list = new LinkedList<T>();
        }

        public void Clear()
        {
            lock (_listLock)
            {
                _list.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (_listLock)
            {
                return _list.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_listLock)
            {
                _list.CopyTo(array, arrayIndex);
            }
        }

        public LinkedList<T>.Enumerator GetEnumerator()
        {
            lock (_listLock)
            {
                return _list.GetEnumerator();
            }
        }

        public bool Remove(T item)
        {
            lock (_listLock)
            {
                return _list.Remove(item);
            }
        }

        public void Remove(LinkedListNode<T> node)
        {
            lock (_listLock)
            {
                _list.Remove(node);
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void LockList()
        {
            Monitor.Enter(_listLock);
        }

        public void UnlockList()
        {
            Monitor.Exit(_listLock);
        }

        public IEnumerable<LinkedListNode<T>> GetLinkedListNode()
        {
            LinkedListNode<T> node = _list.First;
            while (node != null)
            {
                yield return node;
                node = node.Next;
            }
        }
    }
}
