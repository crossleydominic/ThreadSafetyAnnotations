﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadSafetyAnnotations.Attributes;

namespace ThreadSafetyAnnotations.Consumer.LinkedListExample
{
    [ThreadSafe]
    public class ConcurrentList<T>
    {
        [Lock]
        private object _lock;

        [GuardedBy("_lock")]
        private List<T> _internalList;

        public ConcurrentList()
        {
            _internalList = new List<T>();
        }

        public void Add(T item)
        {
            lock (_lock)
            {
                _internalList.Add(item);
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _internalList.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (_lock)
            {
                return _internalList.Contains(item);
            }
        }

        public bool Remove(T item)
        {
            lock (_lock)
            {
                return _internalList.Remove(item);
            }
        }

        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _internalList.Count;
                }
            }
        }

        public int IndexOf(T item)
        {
            lock (_lock)
            {
                return _internalList.IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            lock (_lock)
            {
                _internalList.Insert(index, item);
            }
        }

        public void RemoveAt(int index)
        {
            lock (_lock)
            {
                _internalList.RemoveAt(index);
            }
        }

        public T this[int index]
        {
            get
            {
                lock (_lock)
                {
                    return _internalList[index];
                }
            }
            set
            {
                lock (_lock)
                {
                    _internalList[index] = value;
                }
            }
        }
    }
}
