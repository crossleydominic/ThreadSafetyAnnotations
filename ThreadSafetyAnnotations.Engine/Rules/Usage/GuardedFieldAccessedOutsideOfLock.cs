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
        public AnalysisResult Analyze(CommonSyntaxTree tree, SemanticModel model, ClassInfo classInfo)
        {
            if (classInfo.HasThreadSafeAttribute == false ||
                classInfo.GuardedFields.Count == 0 ||
                classInfo.Locks.Count == 0)
            {
                return AnalysisResult.Succeeded;
            }

            foreach (MethodDeclarationSyntax methodDeclaration in classInfo.Declaration.DescendantNodes().OfType<MethodDeclarationSyntax>())
            {
                Issue issue = AnalyzeMethod(methodDeclaration, model, classInfo);

                if (issue != null)
                {
                    return new AnalysisResult(issue);
                }
            }

            return AnalysisResult.Succeeded;
        }

        private Issue AnalyzeMethod(MethodDeclarationSyntax methodDeclaration, SemanticModel model, ClassInfo classInfo)
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
                    LockHierarchy controlFlowHierarchy = CreateLockHiearchyFromIdentifier(identifierName);

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

        private LockHierarchy CreateLockHiearchyFromIdentifier(IdentifierNameSyntax identifier)
        {
            //Go up the syntax tree looking at locks and record them in 
            //LAST TO FIRST order 
            List<string> lastToFirstLockList = new List<string>();
            
            //Assuming this traverses the tree upwards and reports nodes in order
            var lockStatements = identifier.Ancestors().OfType<LockStatementSyntax>();

            foreach (LockStatementSyntax lockStatement in lockStatements)
            {
                string lockName = lockStatement.DescendantNodes()
                    .OfType<IdentifierNameSyntax>()
                    .First().Identifier.ValueText;

                lastToFirstLockList.Add(lockName);
            }

            //Reverse list to put it in locks taken FIRST-TO-LAST order
            lastToFirstLockList.Reverse();

            return LockHierarchy.FromStringList(lastToFirstLockList);
        }
    }
}
