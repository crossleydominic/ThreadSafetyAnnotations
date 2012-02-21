using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadSafetyAnnotations.Attributes
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
}
