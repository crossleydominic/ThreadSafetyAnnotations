using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadSafetyAnnotations.TestConsumer
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class GuardedByAttribute : Attribute
    {
        private string[] _lockNames;
        public GuardedByAttribute(params string[] lockNames)
        {
            _lockNames = lockNames;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class LockAttribute : Attribute
    {
    }


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ThreadSafeAttribute : Attribute
    {
    }
}
