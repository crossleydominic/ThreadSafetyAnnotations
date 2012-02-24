using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThreadSafetyAnnotations.Attributes;

namespace ThreadSafetyAnnotations.TestConsumer
{
    [ThreadSafe]
    public partial class WithPartial
    {
        [Lock]
        private object _lock1;

        [GuardedBy("_lock1")]
        private int _data1;
    }

    [ThreadSafe]
    public class WithPublicLock
    {
        //Lock is public, invalid
        [Lock]
        public object _lock1;
    }

    [ThreadSafe]
    public class WithProtectedLock
    {
        //Lock is protected, invalid
        [Lock]
        protected object _lock1;
    }

    [ThreadSafe]
    public class WithNonObjectLock
    {
        //Lock is not System.Object, invalid
        [Lock]
        public SomeLockType _lock1;
    }

    public class WithoutThreadSafeAttribute
    {
        //Lock is not System.Object, invalid
        [Lock]
        public SomeLockType _lock1;
    }

    [ThreadSafe]
    public class WithPublicDataMember
    {
        //Lock is public, invalid
        [GuardedByAttribute("")]
        public int _data1;
    }

    [ThreadSafe]
    public class WithUnknownLockName
    {
        //Valid lock
        [Lock]
        private object _lock1;

        //Unknown lock name.
        [GuardedBy("_lock2")]
        private int _data1;
    }

    public class SomeLockType : Object { }
}
