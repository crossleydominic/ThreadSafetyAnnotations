using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;

namespace ThreadSafetyAnnotations.Engine.Rules
{
    public interface IAnalysisRule
    {
        List<Issue> Analyze(IBaseInfo target);
        Type TargetType { get; }

        Type TargetTypeEx { get; }

        List<Issue> AnalyzeEx(CommonSyntaxTree tree, SemanticModel model, ClassInfoEx classInfo);
    }
}
