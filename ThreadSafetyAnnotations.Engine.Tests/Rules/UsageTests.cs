using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ThreadSafetyAnnotations.Engine.Tests.Rules
{
    [TestFixture]
    public class UsageTests
    {
        [Test]
        public void EnsureAnalysisPerformedOnIndexerSet()
        {
            AnalysisResult result = CompilationHelper.Analyze(@"    
                [ThreadSafe]
                public class ClassUnderTest
                {
                    [Lock]
                    private object _lock1;

                    [GuardedBy(""_lock1"")]
                    private int _data1;

                    public int this[int index]
                    {
                        set
                        {
                            _data1 = value;
                        }
                    }
                }");

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Issues);
            Assert.GreaterOrEqual(result.Issues.Count, 1);
            Assert.IsTrue(result.Issues.Any(i => i.ErrorCode == ErrorCode.GUARDED_FIELD_ACCESSED_OUTSIDE_OF_LOCK));
        }

        [Test]
        public void EnsureAnalysisPerformedOnIndexerGet()
        {
            AnalysisResult result = CompilationHelper.Analyze(@"    
                [ThreadSafe]
                public class ClassUnderTest
                {
                    [Lock]
                    private object _lock1;

                    [GuardedBy(""_lock1"")]
                    private int _data1;

                    public int this[int index]
                    {
                        get
                        {
                            return _data1;
                        }
                    }
                }");

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Issues);
            Assert.GreaterOrEqual(result.Issues.Count, 1);
            Assert.IsTrue(result.Issues.Any(i => i.ErrorCode == ErrorCode.GUARDED_FIELD_ACCESSED_OUTSIDE_OF_LOCK));
        }

        [Test]
        public void EnsureAnalysisPerformedOnMethod()
        {
            AnalysisResult result = CompilationHelper.Analyze(@"    
                [ThreadSafe]
                public class ClassUnderTest
                {
                    [Lock]
                    private object _lock1;

                    [GuardedBy(""_lock1"")]
                    private int _data1;

                    public int Data()
                    {
                        return _data1;
                    }
                }");

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Issues);
            Assert.GreaterOrEqual(result.Issues.Count, 1);
            Assert.IsTrue(result.Issues.Any(i => i.ErrorCode == ErrorCode.GUARDED_FIELD_ACCESSED_OUTSIDE_OF_LOCK));
        }

        [Test]
        public void EnsureAnalysisPerformedOnPropertySet()
        {
            AnalysisResult result = CompilationHelper.Analyze(@"    
                [ThreadSafe]
                public class ClassUnderTest
                {
                    [Lock]
                    private object _lock1;

                    [GuardedBy(""_lock1"")]
                    private int _data1;

                    public int Data
                    {
                        set
                        {
                            _data1 = value;
                        }
                    }
                }");

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Issues);
            Assert.GreaterOrEqual(result.Issues.Count, 1);
            Assert.IsTrue(result.Issues.Any(i => i.ErrorCode == ErrorCode.GUARDED_FIELD_ACCESSED_OUTSIDE_OF_LOCK));
        }

        [Test]
        public void EnsureAnalysisPerformedOnPropertyGet()
        {
            AnalysisResult result = CompilationHelper.Analyze(@"    
                [ThreadSafe]
                public class ClassUnderTest
                {
                    [Lock]
                    private object _lock1;

                    [GuardedBy(""_lock1"")]
                    private int _data1;

                    public int Data
                    {
                        get
                        {
                            return _data1;
                        }
                    }
                }");

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Issues);
            Assert.GreaterOrEqual(result.Issues.Count, 1);
            Assert.IsTrue(result.Issues.Any(i => i.ErrorCode == ErrorCode.GUARDED_FIELD_ACCESSED_OUTSIDE_OF_LOCK));
        }

        [Test]
        public void NoLocksTaken_FailsAnalysis()
        {
            AnalysisResult result = CompilationHelper.Analyze(@"    
                [ThreadSafe]
                public class ClassUnderTest
                {
                    [Lock]
                    private object _lock1;

                    [Lock]
                    private object _lock2;

                    [GuardedBy(""_lock1"", ""_lock2"")]
                    private int _data1;

                    public int AddData()
                    {
                        return _data1;
                    }
                }");

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Issues);
            Assert.GreaterOrEqual(result.Issues.Count, 1);
            Assert.IsTrue(result.Issues.Any(i => i.ErrorCode == ErrorCode.GUARDED_FIELD_ACCESSED_OUTSIDE_OF_LOCK));
        }

        [Test]
        public void LocksTakenInWrongOrder_FailsAnalysis()
        {
            AnalysisResult result = CompilationHelper.Analyze(@"    
                [ThreadSafe]
                public class ClassUnderTest
                {
                    [Lock]
                    private object _lock1;

                    [Lock]
                    private object _lock2;

                    [GuardedBy(""_lock1"", ""_lock2"")]
                    private int _data1;

                    public int AddData()
                    {
                        lock(_lock2)
                        {
                            lock(_lock1)
                            {
                                    return _data1;
                            }
                        }
                    }
                }");

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Issues);
            Assert.GreaterOrEqual(result.Issues.Count, 1);
            Assert.IsTrue(result.Issues.Any(i => i.ErrorCode == ErrorCode.GUARDED_FIELD_ACCESSED_OUTSIDE_OF_LOCK));
        }

        [Test]
        public void InsufficientLocksTaken_FailsAnalysis()
        {
            AnalysisResult result = CompilationHelper.Analyze(@"    
                [ThreadSafe]
                public class ClassUnderTest
                {
                    [Lock]
                    private object _lock1;

                    [Lock]
                    private object _lock2;

                    [GuardedBy(""_lock1"", ""_lock2"")]
                    private int _data1;

                    public int AddData()
                    {
                        lock(_lock1)
                        {
                            return _data1;
                        }
                    }
                }");

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Issues);
            Assert.GreaterOrEqual(result.Issues.Count, 1);
            Assert.IsTrue(result.Issues.Any(i => i.ErrorCode == ErrorCode.GUARDED_FIELD_ACCESSED_OUTSIDE_OF_LOCK));
        }

        [Test]
        public void ClassWithCorrectUsage_PassesAnalysis()
        {
            AnalysisResult result = CompilationHelper.Analyze(@"    
                [ThreadSafe]
                public class ClassUnderTest
                {
                    [Lock]
                    private object _lock1;

                    [Lock]
                    private object _lock2;

                    [Lock]
                    private object _lock3;

                    [GuardedBy(""_lock1"", ""_lock2"", ""_lock3"")]
                    private int _data1;

                    [GuardedBy(""_lock1"")]
                    private int _data2;

                    public int AddData()
                    {
                        lock(_lock1)
                        {
                            lock(_lock2)
                            {
                                lock(_lock3)    
                                {
                                    return _data1 + _data2;
                                }
                            }
                        }
                    }

                    public int Data1    
                    { 
                        get 
                        { 
                            lock(_lock1)
                            {
                                lock(_lock2)
                                {
                                    lock(_lock3)    
                                    {
                                        return _data1;
                                    }
                                }
                            }
                        }
                    }

                    public int Data2
                    { 
                        get 
                        { 
                            lock(_lock1)
                            {
                                return _data2;  
                            }
                        } 
                    }
                }");

            Assert.IsTrue(result.Success);
        }
    }
}
