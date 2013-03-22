using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ThreadSafetyAnnotations.Engine.Tests
{
    [TestFixture]
    public class GuardedFieldRuleTests
    {
        [Test]
        public void GuardedMemberInNonThreadSafeClass_CausesIssue()
        {
            List<Issue> issues = CompilationHelper.Analyze(@"    
                public class ClassUnderTest
                {
                    [GuardedByAttribute(""_lock1"")]
                    public int _data1;
                }");

            Assert.IsNotNull(issues);
            Assert.GreaterOrEqual(issues.Count, 1);
            Assert.IsTrue(issues.Any(i => i.ErrorCode == ErrorCode.GUARDED_MEMBER_IN_A_NON_THREAD_SAFE_CLASS));
        }
        
        [Test]
        public void PublicGuardedField_CausesIssue()
        {
            List<Issue> issues = CompilationHelper.Analyze(@"    
                [ThreadSafe]
                public class ClassUnderTest
                {
                    [GuardedByAttribute("""")]
                    public int _data1;
                }");

            Assert.IsNotNull(issues);
            Assert.GreaterOrEqual(issues.Count, 1);
            Assert.IsTrue(issues.Any(i => i.ErrorCode == ErrorCode.GUARDED_MEMBER_IS_NOT_PRIVATE));
        }

        [Test]
        public void ProtectedGuardedField_CausesIssue()
        {
            List<Issue> issues = CompilationHelper.Analyze(@"    
                [ThreadSafe]
                public class ClassUnderTest
                {
                    //Lock is public, invalid
                    [GuardedByAttribute("""")]
                    protected int _data1;
                }");

            Assert.IsNotNull(issues);
            Assert.GreaterOrEqual(issues.Count, 1);
            Assert.IsTrue(issues.Any(i => i.ErrorCode == ErrorCode.GUARDED_MEMBER_IS_NOT_PRIVATE));
        }
        
        [Test]
        public void GuardedFieldWithUnknownLockName_CausesIssue()
        {
            List<Issue> issues = CompilationHelper.Analyze(@"    
                [ThreadSafe]
                public class ClassUnderTest
                {
                    //Valid lock
                    [Lock]
                    private object _lock1;

                    //Unknown lock name.
                    [GuardedBy(""_lock2"")]
                    private int _data1;
                }");

            Assert.IsNotNull(issues);
            Assert.GreaterOrEqual(issues.Count, 1);
            Assert.IsTrue(issues.Any(i => i.ErrorCode == ErrorCode.GUARDED_MEMBER_REFERENCES_UNKNOWN_LOCK));
        }

        [Test]
        public void GuardedFieldWithEmptyLockName_CausesIssue()
        {
            List<Issue> issues = CompilationHelper.Analyze(@"    
                [ThreadSafe]
                public class ClassUnderTest
                {
                    //Valid lock
                    [Lock]
                    private object _lock1;

                    //Unknown lock name.
                    [GuardedBy("""")]
                    private int _data1;
                }");

            Assert.IsNotNull(issues);
            Assert.GreaterOrEqual(issues.Count, 1);
            Assert.IsTrue(issues.Any(i => i.ErrorCode == ErrorCode.GUARDED_MEMBER_REFERENCES_UNKNOWN_LOCK));
        }
    }
}
