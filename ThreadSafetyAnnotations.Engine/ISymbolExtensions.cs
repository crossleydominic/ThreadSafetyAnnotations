using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.Common;

namespace ThreadSafetyAnnotations.Engine
{
    internal static class ISymbolExtensions
    {
        public static bool HasCustomAttribute<T>(this ISymbol symbol)
        {
            foreach (IAttributeData attribute in symbol.GetAttributes())
            {
                //TODO: Fix assembly reference bug.
                if (typeof(T).Name.StartsWith(attribute.AttributeClass.Name))
                {
                    return true;
                }
            }

            return false;
        }

        public static List<string> GetGuardedByLockNames(this ISymbol symbol)
        {
            List<string> lockNames = new List<string>();

            //TODO: support [GuardedBy(new string[] { "_lock2", "_lock3" })] syntax

            foreach (IAttributeData attribute in symbol.GetAttributes())
            {
                foreach (ITypedConstant constant in attribute.PositionalArguments)
                {
                    lockNames.Add(constant.Value.ToString());
                }
            }

            return lockNames;
        }
    }
}
