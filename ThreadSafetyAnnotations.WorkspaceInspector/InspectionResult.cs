using Roslyn.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Utilities;
using ThreadSafetyAnnotations.Engine;

namespace ThreadSafetyAnnotations.WorkspaceInspector
{
    public class InspectionResult
    {
        private IProject _project;
        private List<AnalysisResult> _results;

        public InspectionResult(IProject project, List<AnalysisResult> results)
        {
            #region Input validation

            Insist.IsNotNull(project, "project");
            Insist.IsNotNull(results, "results");

            #endregion

            _project = project;
            _results = results;
        }

        public IProject Project { get { return _project; } }
        public IReadOnlyList<AnalysisResult> AllResults { get { return _results; } }

        public bool InspectionSuccessed
        {
            get { return _results.All(r => r.Success); }
        }

        public IEnumerable<AnalysisResult> FailingAnalysisResults
        {
            get { return _results.Where(r => r.Success == false); }
        }
    }
}
