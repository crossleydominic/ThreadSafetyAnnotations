Thread Safety Annotations for C#
================================

This is an experimental library to allow C# classes to be marked up with a set of custom attributes that express **very** basic locking policies.  These locking policies can be statically checked at compile time to ensure that a classes implementation is thread safe.  The library was built in order to learn about the Roslyn API and the type of added functionality that it will allow.

This library is build on top of the C# compiler-as-a-service ([Roslyn](http://msdn.microsoft.com/en-gb/roslyn)).

The original inspiration for this came from the implementation of Thread Safety Annotations in CLANG.  More can be read about this here: http://clang.llvm.org/docs/LanguageExtensions.html#thread-safety-annotation-checking

How it works
============

Custom attributes can be used to make up a class and it's fields to define a simple locking policy.
- ThreadSafeAttribute - Should be added to the definition of a class.  This attribute is used as a marker to indicate that the class will be analysed.
- LockAttribute - Used to indicate that the object will be used as the synchronization root as the target of a lock(){...} statement
- GuardedByAttribute - Should be added to any of a classes fields that will be accessed from multiple threads. The attribute allows for the name of a lock (or locks) to be associated with the field.  The analysis engine will disallow any accesses of the field unless its corresponding lock is taken.

Once a class has been marked up with the custom attributes which define the basic locking policies then it becomes possible to check all of the classes methods and properties to ensure that the locking policy is being followed. For instance, if we wanted to ensure that a field is only accessed when it's lock it's taken then we could declare the fields like this

    public class SomeClass
    {
        [Lock]               
        private object _lock;
        
        [GuardedBy("_lock")] 
        private int _data;
        
        //Methods using _data here...
    }

Now that the association between data and lock have been made we can check the implementation of methods and properties to ensure that any accesses to _data only occur withing a lock statement which uses _lock as the target.

This will compile successfully

    public void AddToData(int value)
    {
        lock(_lock)
        {
            /* Lock taken, read and write to _data ok */
            _data = _data + value;
        }
    }
    
But if I'd written some code that uses _data and I'd forgottent to take the lock then compilcation will fail

    public void AddToData(int value)
    {
        /* Compiler error here, no lock taken on read of _data */
        _data = _data + value;
    }

The GuardedByAttribute can also be used to define simple lock hierarchies.

    [GuardedBy("_lock1", "_lock2")]
    private SomeObject _data;
    
Now it's not possible to access _data without taking both _lock1 and _lock2 and the locks must be taken in that order.  

Declaration of fields that lead to conflicting lock hierarchies will also cause a compilation error

    [GuardedBy("_lock1", "_lock2")]
    private SomeObject _data;
    
    /* Compilation failed, declaration of hierarchy will conflict with previous declaration */
    [GuardedBy("_lock2", "_lock1")]
    private SomeObject _data;
    
Limitations    
===========

The static analyser only works in very simple scenarios.  
- Classes can't be static/partial/abstract.
- Locks can only be private and of type System.Object.
- Data fields can only be private.
- The only type of synchronization allowed is using the lock(){...} statements so no Reader/Writer locks or anything clever.
- Locks must be taken in the same method/property body in which the guarded field is accessed.  It's not possible to enter the lock in method A, call method B (whilst still inside the lock statement) and access the guarded field within method B.
- It won't stop you copying a reference inside a lock and then modifying that object via the copied reference outside the lock.

Example
=======

A very trivial exmaple showing complete usage follows.

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