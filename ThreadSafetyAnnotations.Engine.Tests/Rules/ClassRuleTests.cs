using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ThreadSafetyAnnotations.Engine.Tests.Rules
{
    [TestFixture]
    public class ClassRuleTests
    {
        [Test]
        public void ClassWithThreadSafeAttributeButNoLocksOrGuardedMembers_FailsAnalysis()
        {
            AnalysisResult result = CompilationHelper.Analyze(@" 
                [ThreadSafe]   
                public class ClassUnderTest
                {
                }");

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Issues);
            Assert.AreEqual(1, result.Issues.Count);
            Assert.IsTrue(result.Issues.Any(i => i.ErrorCode == ErrorCode.CLASS_MUST_HAVE_LOCKS_OR_GUARDED_FIELDS));
        }

        [Test]
        public void ClassNotUsingThreadSafety_PassesAnalysis()
        {
            AnalysisResult result = CompilationHelper.Analyze(@"    
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

            Assert.IsTrue(result.Success);
        }

        [Test]
        public void AbstractClass_FailsAnalysis()
        {
            AnalysisResult result = CompilationHelper.Analyze(@"    
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

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Issues);
            Assert.GreaterOrEqual(result.Issues.Count, 1);
            Assert.IsTrue(result.Issues.Any(i => i.ErrorCode == ErrorCode.CLASS_CANNOT_BE_ABSTRACT));
        }

        [Test]
        public void StaticClass_FailsAnalysis()
        {
            AnalysisResult result = CompilationHelper.Analyze(@"    
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

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Issues);
            Assert.GreaterOrEqual(result.Issues.Count, 1);
            Assert.IsTrue(result.Issues.Any(i => i.ErrorCode == ErrorCode.CLASS_CANNOT_BE_STATIC));
        }

        [Test]
        public void PartialClass_FailsAnalysis()
        {
            AnalysisResult result = CompilationHelper.Analyze(@"    
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

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Issues);
            Assert.GreaterOrEqual(result.Issues.Count, 1);
            Assert.IsTrue(result.Issues.Any(i => i.ErrorCode == ErrorCode.CLASS_CANNOT_BE_PARTIAL));
        }
    }
}
