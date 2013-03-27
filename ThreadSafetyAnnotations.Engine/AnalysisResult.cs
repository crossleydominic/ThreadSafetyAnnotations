using System;
using System.Collections.Generic;
using Shared.Utilities;

namespace ThreadSafetyAnnotations.Engine
{
    public class AnalysisResult
    {
        public static readonly AnalysisResult Succeeded = new AnalysisResult();

        private bool _success;
        private List<Issue> _issues;

        private AnalysisResult()
        {
            _success = true;
            _issues = null;
        }

        public AnalysisResult(Issue issue) : this(new Issue[] {issue}) {}

        public AnalysisResult(IEnumerable<Issue> issues)
        {
            #region Input validation

            Insist.IsNotNull(issues, "issues");
            Insist.AllItemsAreNotNull(issues, "issues");

            #endregion

            _success = false;
            _issues = new List<Issue>();
            _issues.AddRange(issues);
        }

        public bool Success { get { return _success; } }

        public List<Issue> Issues
        {
            get
            {
                if (_success)
                {
                    throw new InvalidOperationException("Cannot get issues for a successful result");
                }
                return _issues;
            }
        }
    }
}
