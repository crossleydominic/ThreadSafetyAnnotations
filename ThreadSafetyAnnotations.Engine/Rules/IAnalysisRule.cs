﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;
using ThreadSafetyAnnotations.Engine.Info;

namespace ThreadSafetyAnnotations.Engine.Rules
{
    public interface IAnalysisRule
    {
        AnalysisResult AnalyzeEx(CommonSyntaxTree tree, SemanticModel model, ClassInfo classInfo);
    }
}
