using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadSafetyAnnotations.Attributes;

namespace ThreadSafetyAnnotations.Consumer.LinkedListExample
{
    //Try commenting out some of the locks below and running either the InspectionConsole or CompileConsole

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
    }
}
