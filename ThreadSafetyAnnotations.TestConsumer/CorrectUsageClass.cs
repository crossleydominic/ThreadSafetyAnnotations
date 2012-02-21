using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThreadSafetyAnnotations.Attributes;
using System.Threading;

namespace ThreadSafetyAnnotations.TestConsumer
{
    [ThreadSafe]
    public class CorrectUsageClass
    {
        //Single lock in a declaration, declared private
        [Lock]
        private object _lock1;

        //Multiple locks in a declaration.
        [Lock]
        private object _lock2, _lock3;

        //Multiple data members in a declaration, protected by a single lock.
        [GuardedBy("_lock1")]
        private int _intType1, _intType2;

        //Reference type data member
        [GuardedBy("_lock2")]
        private RefTypeValue _refType;

        //Value type data member, protected by multiple locks.
        [GuardedBy("_lock1", "_lock2", "_lock3")]
        private ValueType _valType;

        public void AcquireLockWithLockKeyword()
        {
            lock (_lock1)
            {
                _intType1 += _intType1;
            }
        }

        public void AcquireLockWithMonitorKeyword()
        {
            Monitor.Enter(_lock1);
            try
            {
                _intType1 += _intType1;
            }
            finally
            {
                Monitor.Exit(_lock1);
            }
        }

        public int AllowGetAndSet
        {
            get
            {
                int copy = 0;
                lock (_lock1)
                {
                    copy = _intType1;
                }
                return copy;
            }
            set
            {
                lock (_lock1)
                {
                    _intType1 = value;
                }
            }
        }
    }

    public class RefTypeValue
    {
        public int Data { get; set; }

        public void DoNothing() { }
    }

    public struct ValueTypeValue
    {
        public int Data { get; set; }
    }
}
