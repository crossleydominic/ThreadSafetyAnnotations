using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ThreadSafetyAnnotations.Engine.Tests.Rules
{
    [TestFixture]
    public class ClassRuleTests
    {
        [Test]
        public void ClassWithThreadSafeAttributeButNoLocksOrGuardedMembers()
        {
            List<Issue> issues = CompilationHelper.Analyze(@" 
                [ThreadSafe]   
                public class ClassUnderTest
                {
                }");

            Assert.IsNotNull(issues);
            Assert.AreEqual(1, issues.Count); 
            Assert.IsTrue(issues.Any(i => i.ErrorCode == ErrorCode.CLASS_MUST_HAVE_LOCKS_OR_GUARDED_FIELDS));
        }

        [Test]
        public void ClassNotUsingThreadSafety()
        {
            List<Issue> issues = CompilationHelper.Analyze(@"    
                public class ClassUnderTest
                {
                    public object _someObj;

                    public int _data1;
                    public int _data2;

                    public int AddData()
                    {
                        return _data1 + _data2;
                    }

                    public int Data1{ get { return _data1; } }
                    public int Data2{ get { return _data2; } }
                }");

            Assert.IsNotNull(issues);
            Assert.AreEqual(0, issues.Count);
        }

        [Test]
        public void AbstractClass_CausesIssue()
        {
            List<Issue> issues = CompilationHelper.Analyze(@"    
                [ThreadSafe]
                public abstract class ClassUnderTest
                {
                    public object _someObj;

                    public int _data1;
                    public int _data2;

                    public int AddData()
                    {
                        return _data1 + _data2;
                    }

                    public int Data1{ get { return _data1; } }
                    public int Data2{ get { return _data2; } }
                }");

            Assert.IsNotNull(issues);
            Assert.GreaterOrEqual(issues.Count, 1);
            Assert.IsTrue(issues.Any(i => i.ErrorCode == ErrorCode.CLASS_CANNOT_BE_ABSTRACT));
        }

        [Test]
        public void StaticClass_CausesIssue()
        {
            List<Issue> issues = CompilationHelper.Analyze(@"    
                [ThreadSafe]
                public static class ClassUnderTest
                {
                    [Lock]
                    public static object _someObj;

                    [GuardedBy(""_lock1"")]
                    public static int _data1;

                    [GuardedBy(""_lock1"")]
                    public static int _data2;
                }");

            Assert.IsNotNull(issues);
            Assert.GreaterOrEqual(issues.Count, 1);
            Assert.IsTrue(issues.Any(i => i.ErrorCode == ErrorCode.CLASS_CANNOT_BE_STATIC));
        }

        [Test]
        public void PartialClass_CausesIssue()
        {
            List<Issue> issues = CompilationHelper.Analyze(@"    
                [ThreadSafe]
                public partial class ClassUnderTest
                {
                    [Lock]
                    public static object _someObj;

                    [GuardedBy(""_lock1"")]
                    public static int _data1;

                    [GuardedBy(""_lock1"")]
                    public static int _data2;
                }");

            Assert.IsNotNull(issues);
            Assert.GreaterOrEqual(issues.Count, 1);
            Assert.IsTrue(issues.Any(i => i.ErrorCode == ErrorCode.CLASS_CANNOT_BE_PARTIAL));
        }

        [Test]
        public void ClassWithCorrectUsage()
        {
            List<Issue> issues = CompilationHelper.Analyze(@"    
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

                    public int Data1{ get { return _data1; } }
                    public int Data2{ get { return _data2; } }
                }");

            Assert.IsNotNull(issues);
            Assert.AreEqual(0, issues.Count);
        }

    }
}
