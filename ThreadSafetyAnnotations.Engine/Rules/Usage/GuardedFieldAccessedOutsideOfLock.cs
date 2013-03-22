using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;
using ThreadSafetyAnnotations.Engine.Info;

namespace ThreadSafetyAnnotations.Engine.Rules.Usage
{
    public class GuardedFieldAccessedOutsideOfLock : IAnalysisRule
    {
        public Issue AnalyzeEx(CommonSyntaxTree tree, SemanticModel model, ClassInfo classInfo)
        {
            if (classInfo.HasThreadSafeAttribute == false ||
                classInfo.GuardedFields.Count == 0 ||
                classInfo.Locks.Count == 0)
            {
                return null;
            }

            foreach (MethodDeclarationSyntax methodDeclaration in classInfo.Declaration.DescendantNodes().OfType<MethodDeclarationSyntax>())
            {
                DataFlowAnalysis dataFlow = model.AnalyzeDataFlow(methodDeclaration.Body);
                
                Issue issue = AnalyzeMethod(methodDeclaration, model, dataFlow, classInfo);

                if (issue != null)
                {
                    return issue;
                }
            }

            return null;
        }

        private Issue AnalyzeMethod(MethodDeclarationSyntax methodDeclaration, SemanticModel model, DataFlowAnalysis dataFlow, ClassInfo classInfo)
        {
            var identifiers = methodDeclaration.DescendantNodes().OfType<IdentifierNameSyntax>().ToList();

            foreach (IdentifierNameSyntax identifierName in identifiers)
            {
                SymbolInfo identifierSymbol = model.GetSymbolInfo(identifierName);

                //Does this symbol refer to a GuardedField?
                GuardedFieldInfo foundGuardedField = classInfo.GuardedFields.FirstOrDefault(field => field.Symbol == identifierSymbol.Symbol);

                if (foundGuardedField != null)
                {
                    //We must be inside a lock statement
                    LockHierarchy controlFlowHierarchy = LockHierarchy.FromIdentifierName(identifierName);

                    bool lockHierarchySatisfied = LockHierarchy.IsSatisfiedBy(foundGuardedField.DeclaredLockHierarchy, controlFlowHierarchy);

                    if (!lockHierarchySatisfied)
                    {
                        return new Issue(
                            ErrorCode.GUARDED_FIELD_ACCESSED_OUTSIDE_OF_LOCK, 
                            foundGuardedField.Declaration,
                            foundGuardedField.Symbol);
                    }
                }
            }

            return null;
        }
    }
}
