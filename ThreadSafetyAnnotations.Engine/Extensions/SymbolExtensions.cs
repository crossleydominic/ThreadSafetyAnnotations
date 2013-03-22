using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;

namespace ThreadSafetyAnnotations.Engine.Extensions
{
    public static class SymbolExtensions
    {
        public static bool HasCustomAttribute<T>(this Symbol symbol) where T : Attribute
        {
            foreach (CommonAttributeData attribute in symbol.GetAttributes())
            {
                if (attribute.IsInstanceOfAttributeType<T>())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
