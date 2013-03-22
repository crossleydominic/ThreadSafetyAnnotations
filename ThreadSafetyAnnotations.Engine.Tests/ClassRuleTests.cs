using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;
using NUnit.Framework;

namespace ThreadSafetyAnnotations.Engine.Tests
{
    [TestFixture]
    public class ClassRuleTests
    {
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

                    [GuardedByAttribute(""_lock1"")]
                    private int _data1;
                }");

            Assert.IsNotNull(issues);
            Assert.AreEqual(0, issues.Count);
        }

    }
}
