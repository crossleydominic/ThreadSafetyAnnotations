using System;
using System.Collections.Generic;
using System.Linq;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;

namespace ThreadSafetyAnnotations.Engine.Rules.LockRules
{
    internal class LockProtectsGuardedMember : IAnalysisRule
    {
        public Issue AnalyzeEx(CommonSyntaxTree tree, SemanticModel model, ClassInfo classInfo)
        {
            //Get the list of all lock names that are declared on guarded fields
            IEnumerable<string> requiredLockNames = classInfo.GuardedFields.SelectMany(field => field.Attribute.ConstructorArguments.SelectMany(arg => arg.Values.Select(argVal=>argVal.Value.ToString()))).ToList();

            //Check that all locks actually protect something
            foreach (LockInfo lockInfo in classInfo.Locks)
            {
                //see if the current lock is used by any guarded members
                if (requiredLockNames.Any(rln => rln == lockInfo.Symbol.Name) == false)
                {
                    return new Issue(
                        ErrorCode.LOCK_PROTECTS_NOTHING, 
                        lockInfo.Declaration,
                        lockInfo.Symbol);
                }
            }

            return null;
        }
    }
}
