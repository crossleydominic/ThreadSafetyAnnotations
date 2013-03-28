using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ThreadSafetyAnnotations.Engine.Tests.Rules
{
    [TestFixture]
    public class LockRuleTests
    {
        [Test]
        public void ClassWithPublicLock_FailsAnalysis()
        {
            AnalysisResult result = CompilationHelper.Analyze(@"    
                [ThreadSafe]
                public class ClassUnderTest
                {
                    [Lock]
                    public object _lock1;
                }");

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Issues);
            Assert.GreaterOrEqual(result.Issues.Count, 1);
            Assert.IsTrue(result.Issues.Any(i => i.ErrorCode == ErrorCode.LOCK_IS_NOT_PRIVATE));
        }

        [Test]
        public void ClassWithLockProtectingNothing_FailsAnalysis()
        {
            AnalysisResult result = CompilationHelper.Analyze(@"    
                [ThreadSafe]
                public class ClassUnderTest
                {
                    [Lock]
                    public object _lock1;
                }");

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Issues);
            Assert.GreaterOrEqual(result.Issues.Count, 1);
            Assert.IsTrue(result.Issues.Any(i => i.ErrorCode == ErrorCode.LOCK_PROTECTS_NOTHING));
        }

        [Test]
        public void ClassWithProtectedLock_FailsAnalysis()
        {
            AnalysisResult result = CompilationHelper.Analyze(@"    
                [ThreadSafe]
                public class ClassUnderTest
                {
                    [Lock]
                    protected object _lock1;
                }");
            
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Issues);
            Assert.GreaterOrEqual(result.Issues.Count, 1);
            Assert.IsTrue(result.Issues.Any(i => i.ErrorCode == ErrorCode.LOCK_IS_NOT_PRIVATE));
        }

        [Test]
        public void ClassWithNonObjectLock_FailsAnalysis()
        {
            AnalysisResult result = CompilationHelper.Analyze(@"   
                [ThreadSafe]
                public class ClassUnderTest
                {
                    [Lock]
                    public SomeLockType _lock1;
                }

                public class SomeLockType : Object { }");

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Issues);
            Assert.GreaterOrEqual(result.Issues.Count, 1);
            Assert.IsTrue(result.Issues.Any(i => i.ErrorCode == ErrorCode.LOCK_MUST_BE_SYSTEM_OBJECT));
        }

        [Test]
        public void LockInNonThreadSafeClass_FailsAnalysis()
        {
            AnalysisResult result = CompilationHelper.Analyze(@"   

                public class SomeLockType{}
                 
                public class ClassUnderTest
                {
                    [Lock]
                    public SomeLockType _lock1;
                }");

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Issues);
            Assert.GreaterOrEqual(result.Issues.Count, 1);
            Assert.IsTrue(result.Issues.Any(i => i.ErrorCode == ErrorCode.LOCK_IN_A_NON_THREAD_SAFE_CLASS));
        }
    }
}
