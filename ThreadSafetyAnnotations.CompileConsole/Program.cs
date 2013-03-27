using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;
using Roslyn.Services.CSharp;

namespace ThreadSafetyAnnotations.CompileConsole
{
    class Program
    {
        private static void Main(string[] args)
        {
            //TODO: Accept command line params for paths
            string s = Environment.CurrentDirectory + "../../../../ThreadSafetyAnnotations.Consumer.LinkedListExample/ThreadSafetyAnnotations.Consumer.LinkedListExample.csproj";
            IWorkspace workspace = Workspace.LoadStandAloneProject(s);

            foreach (var project in workspace.CurrentSolution.Projects)
            {
                var compilation = project.GetCompilation();

                foreach (var document in project.Documents)
                {
                    //SemanticModel model = (SemanticModel)document.GetSemanticModel();

                    //var rewriter = new AutoGuardSyntaxRewriter(model);

                    ////Dont know of a nice way of getting the root node.
                    //var rewritten = rewriter.Visit(model.SyntaxTree.GetRoot().AncestorsAndSelf().First());

                    ////Hmm, rewrittern.SyntaxTree seems to be null, we'll have to convert to an intermediary string for now
                    //compilation = compilation.ReplaceSyntaxTree(document.GetSyntaxTree(), SyntaxTree.ParseText(rewritten.ToFullString()));
                }

                //CommonEmitResult result;
                //string outputFile = "CompiledTarget.exe";
                //using (var file = new FileStream(outputFile, FileMode.Create))
                //{
                //    //TODO: Add command line parameter for output location
                //    result = compilation.Emit(file);
                //}

                //if (result.Success)
                //{
                //    Console.WriteLine("Compilation succeeded");
                //    Console.WriteLine("Executing Target...");

                //    Process p = new Process();
                //    p.StartInfo = new ProcessStartInfo()
                //    {
                //        FileName = outputFile
                //    };
                //    p.Start();
                //    p.WaitForExit();

                //    Console.WriteLine("Target exited");
                //}
                //else
                //{
                //    Console.WriteLine("Compilation failed");

                //    foreach (Diagnostic diagnostic in result.Diagnostics)
                //    {
                //        Console.WriteLine(string.Empty);
                //        Console.WriteLine(diagnostic.Info);
                //        Console.WriteLine(diagnostic.Location);
                //    }
                //}

                //Console.ReadLine();
            }
        }
    }
}
