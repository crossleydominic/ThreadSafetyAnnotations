using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadSafetyAnnotations.Engine.Rules
{
    internal interface IAnalysisRule
    {
        List<Issue> Analyze(IBaseInfo target);
        Type TargetType { get; }
    }
}
