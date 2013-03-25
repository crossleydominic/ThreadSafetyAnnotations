Thread Safety Annotations for C#
================================

This is an experimental library to allow C# classes to be marked up with a set of custom attributes that express basic locking policies.  These locking policies can be statically checked at compile time to ensure that a classes implementation is thread safe.

This library is build on top of the C# compiler-as-a-service ([Roslyn](http://msdn.microsoft.com/en-gb/roslyn)).

The original inspiration for this came from the implementation of Thread Safety Annotations in CLANG.  More can be read about this here: http://clang.llvm.org/docs/LanguageExtensions.html#thread-safety-annotation-checking

Example
=======

A very trivial exmaple showing usage follows.

    //ThreadSafe attributes marks this class as a candidate for static analysis
    [ThreadSafe]
    public class ClassUnderTest
    {
        //The Lock attribute indicates that this field will be used
        //to lock onto as part of a lock(){...} statement
        [Lock]
        private object _lock1;

        [Lock]
        private object _lock2;

        //The GuardedBy attribute indicates which locks will be protecting the field.
        //The locks are ordered and must be obtained in the specified order.
        [GuardedBy("_lock1", "_lock2")]
        private int _data1;

        [GuardedBy("_lock1")]
        private int _data2;

        //This method will pass static analysis, all of the required locks are taken in 
        //the correct order before the fields are accessed.
        public int AddData_Safe()
        {
            lock(_lock1)
            {
                lock(_lock2)
                {
                    return _data1 + _data2;
                }
            }
        }
        
        //This method will fail static analysis, _lock2 is required to be taken before
        //accessing the _data1 field.
        public int AddData_Unsafe()
        {
            lock(_lock1)
            {
                return _data1 + _data2;
            }
        }
        
        public void Increment1()
        {
            lock(_lock1)
            {            
                lock(_lock2)
                {
                    _data1 += 1;
                }
            }
        }
        
        public void Increment2()
        {
            lock(_lock1)
            {
                _data2 += 1;
            }
        }
    }