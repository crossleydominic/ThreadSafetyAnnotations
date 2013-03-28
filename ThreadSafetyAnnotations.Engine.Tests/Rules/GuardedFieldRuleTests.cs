using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ThreadSafetyAnnotations.Engine.Tests.Rules
{
    [TestFixture]
    public class GuardedFieldRuleTests
    {
        [Test]
        public void GuardedFieldInNonThreadSafeClass_FailsAnalysis()
        {
            AnalysisResult result = CompilationHelper.Analyze(@"    
                public class ClassUnderTest
                {
                    [GuardedByAttribute(""_lock1"")]
                    public int _data1;
                }");

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Issues);
            Assert.GreaterOrEqual(result.Issues.Count, 1);
            Assert.IsTrue(result.Issues.Any(i => i.ErrorCode == ErrorCode.GUARDED_FIELD_IN_A_NON_THREAD_SAFE_CLASS));
        }
        
        [Test]
        public void PublicGuardedField_FailsAnalysis()
        {
            AnalysisResult result = CompilationHelper.Analyze(@"    
                [ThreadSafe]
                public class ClassUnderTest
                {
                    [GuardedByAttribute("""")]
                    public int _data1;
                }");

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Issues);
            Assert.GreaterOrEqual(result.Issues.Count, 1);
            Assert.IsTrue(result.Issues.Any(i => i.ErrorCode == ErrorCode.GUARDED_FIELD_IS_NOT_PRIVATE));
        }

        [Test]
        public void ProtectedGuardedField_FailsAnalysis()
        {
            AnalysisResult result = CompilationHelper.Analyze(@"    
                [ThreadSafe]
                public class ClassUnderTest
                {
                    //Lock is public, invalid
                    [GuardedByAttribute("""")]
                    protected int _data1;
                }");

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Issues);
            Assert.GreaterOrEqual(result.Issues.Count, 1);
            Assert.IsTrue(result.Issues.Any(i => i.ErrorCode == ErrorCode.GUARDED_FIELD_IS_NOT_PRIVATE));
        }
        
        [Test]
        public void GuardedFieldWithUnknownLockName_FailsAnalysis()
        {
            AnalysisResult result = CompilationHelper.Analyze(@"    
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

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Issues);
            Assert.GreaterOrEqual(result.Issues.Count, 1);
            Assert.IsTrue(result.Issues.Any(i => i.ErrorCode == ErrorCode.GUARDED_FIELD_REFERENCES_UNKNOWN_LOCK));
        }

        [Test]
        public void GuardedFieldWithEmptyLockName_FailsAnalysis()
        {
            AnalysisResult result = CompilationHelper.Analyze(@"    
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

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Issues);
            Assert.GreaterOrEqual(result.Issues.Count, 1);
            Assert.IsTrue(result.Issues.Any(i => i.ErrorCode == ErrorCode.GUARDED_FIELD_REFERENCES_UNKNOWN_LOCK));
        }

        [Test]
        public void GuardedFieldUsesSameLockMoreThanOnce_FailsAnalysis()
        {
            AnalysisResult result = CompilationHelper.Analyze(@"    
                [ThreadSafe]
                public class ClassUnderTest
                {
                    //Valid lock
                    [Lock]
                    private object _lock1;

                    //Unknown lock name.
                    [GuardedBy(""_lock1"", ""_lock1"")]
                    private int _data1;
                }");

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Issues);
            Assert.GreaterOrEqual(result.Issues.Count, 1);
            Assert.IsTrue(result.Issues.Any(i => i.ErrorCode == ErrorCode.GUARDED_FIELD_USES_SAME_LOCK_MORE_THAN_ONCE));
        }


        [Test]
        public void GuardedFieldCausesLockHierarchyConflict_FailsAnalysis()
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

                    [GuardedBy(""_lock1"", ""_lock3"", ""_lock2"")]
                    private int _data2;
                }");

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Issues);
            Assert.GreaterOrEqual(result.Issues.Count, 1);
            Assert.IsTrue(result.Issues.Any(i => i.ErrorCode == ErrorCode.GUARDED_FIELD_LOCK_HIERARCHY_DECLARATION_CONFLICT));
        }

        [Test]
        public void GuardedFieldWithLockHierarchySubset_PassesAnalysis()
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

                    [GuardedBy(""_lock1"", ""_lock3"")]
                    private int _data2;
                }");

            Assert.IsTrue(result.Success);
        }
    }
}
