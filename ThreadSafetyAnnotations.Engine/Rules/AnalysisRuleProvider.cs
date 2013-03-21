using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ThreadSafetyAnnotations.Engine.Rules
{
    public class AnalysisRuleProvider : IAnalysisRuleProvider
    {
        Lazy<List<IAnalysisRule>> _rules = new Lazy<List<IAnalysisRule>>(() =>
        {   
            TypeFilter typeFilter = new TypeFilter((t, o) => t == typeof(IAnalysisRule));
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.FindInterfaces(typeFilter, null).Any() && t.IsAbstract == false)
                .Select(t => (IAnalysisRule)Activator.CreateInstance(t))
                .ToList();

        });

        public List<IAnalysisRule> Rules
        {
            get { return _rules.Value; }
        }
    }
}
