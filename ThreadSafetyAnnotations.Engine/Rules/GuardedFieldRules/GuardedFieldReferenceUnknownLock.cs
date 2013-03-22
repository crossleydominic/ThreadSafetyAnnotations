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
        public Issue AnalyzeEx(CommonSyntaxTree tree, SemanticModel model, ClassInfo classInfo)
        {
            foreach (var guardedField in classInfo.GuardedFields)
            {
                //Get list of all lock names that are protecting this field
                List<string> lockNames = guardedField.Attribute.ConstructorArguments.SelectMany(arg => arg.Values.Select(argVal=>argVal.Value.ToString())).ToList();

                //Check to see if all of the locks are actually declared
                foreach (string lockName in lockNames)
                {
                    if (classInfo.Locks.Any(@lock => @lock.Symbol.Name == lockName) == false)
                    {
                        return new Issue(
                            ErrorCode.GUARDED_MEMBER_REFERENCES_UNKNOWN_LOCK,
                            guardedField.Declaration,
                            guardedField.Symbol);
                    }
                }
            }

            return null;
        }
    }
}
