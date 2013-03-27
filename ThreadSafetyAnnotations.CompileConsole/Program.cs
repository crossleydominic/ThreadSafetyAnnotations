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

namespace ThreadSafetyAnnotations.CompileConsole
{
    class Program
    {
        private static void Main(string[] args)
        {
            //TODO: Accept command line params for paths
            string projectPath = Environment.CurrentDirectory + "../../../../ThreadSafetyAnnotations.Consumer.ConcurrentListExample/ThreadSafetyAnnotations.Consumer.ConcurrentListExample.csproj";

            ProjectInspector inspector = new ProjectInspector();
            InspectionResult result = inspector.Inspect(projectPath);

            if (result.InspectionSuccessed)
            {
                var compilation = result.Project.GetCompilation();

                CommonEmitResult emitResult;
                string outputFile = result.Project.AssemblyName;

                using (var file = new FileStream(outputFile, FileMode.Create))
                {
                    //TODO: Add command line parameter for output location
                    emitResult = compilation.Emit(file);
                }

                if (emitResult.Success)
                {
                    Console.WriteLine("Compilation succeeded");
                }
                else
                {
                    Console.WriteLine("Compilation failed");

                    foreach (Diagnostic diagnostic in emitResult.Diagnostics)
                    {
                        Console.WriteLine(string.Empty);
                        Console.WriteLine(diagnostic.Info);
                        Console.WriteLine(diagnostic.Location);
                    }
                }
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
