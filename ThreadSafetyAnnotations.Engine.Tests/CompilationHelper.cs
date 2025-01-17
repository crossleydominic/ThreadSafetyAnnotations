﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers;
using ThreadSafetyAnnotations.Attributes;
using System.Reflection;

namespace ThreadSafetyAnnotations.Engine.Tests
{
    public static class CompilationHelper
    {
        private const string PROGRAM_BOILERPLATE_PROLOG =
@"using System;
using System.Collections.Generic;
using System.Text;
using ThreadSafetyAnnotations.Attributes;
 
namespace ThreadSafetyAnnotations.Engine.Tests.Boilerplate
{
";

        private const string PROGRAM_BOILERPLATE_EPILOG =
    @"
}";

        private static Compilation Create(string testClassString)
        {
            SyntaxTree tree = SyntaxTree.ParseText(GetProgramText(testClassString));

            return Compilation.Create("TestCompilation")
                .AddReferences(new MetadataReference[]
                {
                    new MetadataFileReference(typeof(GuardedByAttribute).Assembly.Location),
                    MetadataFileReference.CreateAssemblyReference("mscorlib")
                })
                .AddSyntaxTrees(tree);
        }
        
        public static AnalysisResult Analyze(string testClassString)
        {
            Compilation compilation = CompilationHelper.Create(testClassString);

            AnalysisEngine engine = new AnalysisEngine();

            return engine.Analyze(compilation.SyntaxTrees[0],compilation.GetSemanticModel(compilation.SyntaxTrees[0]));
        }

        private static string GetProgramText(string testClassString)
        {
            return PROGRAM_BOILERPLATE_PROLOG + testClassString + PROGRAM_BOILERPLATE_EPILOG;
        }
    }
}