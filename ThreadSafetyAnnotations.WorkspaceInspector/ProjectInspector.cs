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
    public class ProjectInspector
    {
        public InspectionResult Inspect(string absoluteProjectPath)
        {
            IWorkspace workspace = Workspace.LoadStandAloneProject(Environment.CurrentDirectory + "../../../../ThreadSafetyAnnotations.Consumer.ConcurrentListExample/ThreadSafetyAnnotations.Consumer.ConcurrentListExample.csproj");

            IProject project = workspace.CurrentSolution.Projects.First();

            List<AnalysisResult> analysisResults = new List<AnalysisResult>();

            foreach (IDocument document in project.Documents)
            {
                AnalysisEngine engine = new AnalysisEngine(
                    document.GetSyntaxTree(),
                    (SemanticModel) document.GetSemanticModel());

                AnalysisResult analysisResult = engine.Analyze();

                analysisResults.Add(analysisResult);
            }

            return new InspectionResult(project, analysisResults);
        }
    }
}
