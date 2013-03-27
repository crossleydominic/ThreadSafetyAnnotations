using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;
using Roslyn.Services;
using Roslyn.Services.CSharp;
using ThreadSafetyAnnotations.Engine;
using ThreadSafetyAnnotations.WorkspaceInspector;

namespace ThreadSafetyAnnotations.InspectionConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO: Accept command line params for paths
            string projectPath = Environment.CurrentDirectory + "../../../../ThreadSafetyAnnotations.Consumer.LinkedListExample/ThreadSafetyAnnotations.Consumer.LinkedListExample.csproj";

            ProjectInspector inspector = new ProjectInspector();
            InspectionResult result = inspector.Inspect(projectPath);

            if (result.InspectionSuccessed)
            {
                Console.WriteLine("Inspection succeeded");
            }
            else
            {
                Console.WriteLine("Inspection failed");

                foreach (AnalysisResult analysisResult in result.FailingAnalysisResults)
                {
                    foreach (Issue issue in analysisResult.Issues)
                    {
                        Console.WriteLine(issue.Description);
                        Console.WriteLine(issue.SourceFileName + " line: " + issue.SourceLineNumber.ToString());
                        Console.WriteLine(issue.SourceLineText);
                        Console.WriteLine();
                    }
                }
            }

            Console.ReadLine();

        }
    }
}
