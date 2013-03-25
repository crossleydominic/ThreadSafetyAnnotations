using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ThreadSafetyAnnotations.Engine.Tests.Rules
{
    [TestFixture]
    public class LockRuleTests
    {
        [Test]
        public void ClassWithPublicLock_CausesIssue()
        {
            List<Issue> issues = CompilationHelper.Analyze(@"    
                [ThreadSafe]
                public class ClassUnderTest
                {
                    [Lock]
                    public object _lock1;
                }");

            Assert.IsNotNull(issues);
            Assert.GreaterOrEqual(issues.Count, 1);
            Assert.IsTrue(issues.Any(i => i.ErrorCode == ErrorCode.LOCK_IS_NOT_PRIVATE));
        }

        [Test]
        public void ClassWithLockProtectingNothing_CausesIssue()
        {
            List<Issue> issues = CompilationHelper.Analyze(@"    
                [ThreadSafe]
                public class ClassUnderTest
                {
                    [Lock]
                    public object _lock1;
                }");

            Assert.IsNotNull(issues);
            Assert.GreaterOrEqual(issues.Count, 1);
            Assert.IsTrue(issues.Any(i => i.ErrorCode == ErrorCode.LOCK_PROTECTS_NOTHING));
        }

        [Test]
        public void ClassWithProtectedLock_CausesIssue()
        {
            List<Issue> issues = CompilationHelper.Analyze(@"    
                [ThreadSafe]
                public class ClassUnderTest
                {
                    [Lock]
                    protected object _lock1;
                }");

            Assert.IsNotNull(issues);
            Assert.GreaterOrEqual(issues.Count, 1);
            Assert.IsTrue(issues.Any(i => i.ErrorCode == ErrorCode.LOCK_IS_NOT_PRIVATE));
        }

        [Test]
        public void ClassWithNonObjectLock_CausesIssue()
        {
            List<Issue> issues = CompilationHelper.Analyze(@"   
                [ThreadSafe]
                public class ClassUnderTest
                {
                    [Lock]
                    public SomeLockType _lock1;
                }

                public class SomeLockType : Object { }");

            Assert.IsNotNull(issues);
            Assert.GreaterOrEqual(issues.Count, 1);
            Assert.IsTrue(issues.Any(i => i.ErrorCode == ErrorCode.LOCK_MUST_BE_SYSTEM_OBJECT));
        }

        [Test]
        public void LockInNonThreadSafeClass_CausesIssue()
        {
            List<Issue> issues = CompilationHelper.Analyze(@"   

                public class SomeLockType{}
                 
                public class ClassUnderTest
                {
                    [Lock]
                    public SomeLockType _lock1;
                }");

            Assert.IsNotNull(issues);
            Assert.GreaterOrEqual(issues.Count, 1);
            Assert.IsTrue(issues.Any(i => i.ErrorCode == ErrorCode.LOCK_IN_A_NON_THREAD_SAFE_CLASS));
        }
    }
}
