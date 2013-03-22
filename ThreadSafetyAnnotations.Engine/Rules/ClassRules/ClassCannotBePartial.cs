using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;

namespace ThreadSafetyAnnotations.Engine.Rules.ClassRules
{
    internal class ClassCannotBePartial : IAnalysisRule
    {
        public Issue AnalyzeEx(CommonSyntaxTree tree, SemanticModel model, ClassInfo classInfo)
        {
            if (classInfo.HasThreadSafeAttribute &&
                classInfo.Declaration.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                return new Issue(
                    ErrorCode.CLASS_CANNOT_BE_PARTIAL,
                    classInfo.Declaration,
                    classInfo.Symbol);
            }

            return null;
        }
    }
}
