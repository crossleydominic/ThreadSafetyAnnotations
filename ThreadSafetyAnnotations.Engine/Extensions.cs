using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;

namespace ThreadSafetyAnnotations.Engine
{
    public static class Extensions
    {
        public static IEnumerable<SyntaxNode> OfType(this IEnumerable<SyntaxNode> nodes, Type ofType)
        {
            return nodes.Where(x => x.GetType() == ofType);
        }
    }

    public static class ISymbolExtensions
    {
        public static bool HasCustomAttribute<T>(this ISymbol symbol)
        {
            foreach (CommonAttributeData attribute in symbol.GetAttributes())
            {
                //TODO: Fix assembly reference bug.
                if (typeof(T).Name.StartsWith(attribute.AttributeClass.Name))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
