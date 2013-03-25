using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;
using ThreadSafetyAnnotations.Engine.Info;

namespace ThreadSafetyAnnotations.Engine.Rules.GuardedFieldRules
{
    internal class GuardedFieldReferenceUnknownLock : IAnalysisRule
    {
        public AnalysisResult Analyze(CommonSyntaxTree tree, SemanticModel model, ClassInfo classInfo)
        {
            foreach (var guardedField in classInfo.GuardedFields)
            {
                //Check to see if all of the locks are actually declared
                foreach (string lockName in guardedField.DeclaredLockHierarchy)
                {
                    if (classInfo.Locks.Any(@lock => @lock.Name == lockName) == false)
                    {
                        return new AnalysisResult(new Issue(
                            ErrorCode.GUARDED_FIELD_REFERENCES_UNKNOWN_LOCK,
                            guardedField.Declaration,
                            guardedField.Symbol));
                    }
                }
            }

            return AnalysisResult.Succeeded;
        }
    }
}
