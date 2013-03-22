using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;
using ThreadSafetyAnnotations.Engine;
using Roslyn.Compilers.Common;

namespace ThreadSafetyAnnotations.WorkspaceInspector
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                IWorkspace workspace = Workspace.LoadStandAloneProject(args[0]);

                foreach (IProject project in workspace.CurrentSolution.Projects)
                {
                    foreach (IDocument document in project.Documents)
                    {
                        //AnalysisEngine engine = new AnalysisEngine(document);
                        AnalysisEngine engine = new AnalysisEngine(
                            document.GetSyntaxTree(),
                            (SemanticModel)document.GetSemanticModel());

                        List<Issue> issues = engine.Analyze();

                        foreach (Issue issue in issues)
                        {
                            Console.WriteLine(issue.Description);
                            Console.WriteLine(issue.SourceFileName + " line: " + issue.SourceLineNumber.ToString());
                            Console.WriteLine(issue.SourceLineText);
                            Console.WriteLine();
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
        }
    }
}
