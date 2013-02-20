using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Services;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;
using Roslyn.Compilers;
using ThreadSafetyAnnotations.Engine.Rules.ClassRules;
using ThreadSafetyAnnotations.Engine.Rules;
using System.Reflection;
using ThreadSafetyAnnotations.Attributes;

namespace ThreadSafetyAnnotations.Engine
{
    public class AnalysisEngine
    {
        private CommonSyntaxTree _syntaxTree;
        private ISemanticModel _semanticModel;

        private static readonly List<IAnalysisRule> _analysisRules;

        static AnalysisEngine()
        {
            _analysisRules = new List<IAnalysisRule>();
            TypeFilter typeFilter = new TypeFilter((t, o) => t == typeof(IAnalysisRule));
            foreach (Type type in Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.FindInterfaces(typeFilter, null).Any() && t.IsAbstract == false))
            {
                _analysisRules.Add((IAnalysisRule)Activator.CreateInstance(type));
            }
        }

        public AnalysisEngine(CommonSyntaxTree syntaxTree, ISemanticModel semanticModel)
        {
            #region Input validation

            if (syntaxTree == null)
            {
                throw new ArgumentNullException("syntaxTree");
            }

            if (semanticModel == null)
            {
                throw new ArgumentNullException("semanticModel");
            }

            #endregion

            _syntaxTree = syntaxTree;
            _semanticModel = semanticModel;
        }

        public List<Issue> Analzye()
        {
            List<Issue> issues = new List<Issue>();

            List<ClassInfo> classInfos = DiscoverInformation();

            foreach (ClassInfo classInfo in classInfos)
            {
                foreach (IAnalysisRule rule in _analysisRules.Where(r=>r.TargetType == typeof(ClassInfo)))
                {
                    issues.AddRange(rule.Analyze(classInfo));
                }

                foreach (LockInfo lockInfo in classInfo.Locks)
                {
                    foreach (IAnalysisRule rule in _analysisRules.Where(r => r.TargetType == typeof(LockInfo)))
                    {
                        issues.AddRange(rule.Analyze(lockInfo));
                    }
                }

                foreach (GuardedMemberInfo memberInfo in classInfo.GuardedMembers)
                {
                    foreach (IAnalysisRule rule in _analysisRules.Where(r => r.TargetType == typeof(GuardedMemberInfo)))
                    {
                        issues.AddRange(rule.Analyze(memberInfo));
                    }
                }
            }

            return issues;
            
        }

        private List<ClassInfo> DiscoverInformation()
        {
            List<ClassInfo> classes = new List<ClassInfo>();

            foreach (ClassDeclarationSyntax classDeclaration in _syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                ISymbol sym = _semanticModel.GetDeclaredSymbol(classDeclaration);
                Assembly assembly = Assembly.GetAssembly(typeof(ThreadSafeAttribute));
                if (assembly.GetTypes().Any(t => t.Name == sym.Name))
                {
                    continue;
                }
                

                classes.Add(new ClassInfo(classDeclaration, _semanticModel));
            }

            return classes;
        }
    }
}
