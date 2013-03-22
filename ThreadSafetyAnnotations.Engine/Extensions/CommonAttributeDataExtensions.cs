using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roslyn.Compilers.Common;

namespace ThreadSafetyAnnotations.Engine.Extensions
{
    public static class CommonAttributeDataExtensions
    {
        public static bool IsInstanceOfAttributeType<TAttributeType>(this CommonAttributeData attributeSymbol)
        {
            Type attributeType = typeof(TAttributeType);

            //TODO: Don't like this, think of a nicer way to do this in the future.
            if (string.Equals(attributeSymbol.AttributeClass.ContainingAssembly.Name, attributeType.Assembly.GetName().Name, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(attributeSymbol.AttributeClass.Name, attributeType.Name, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
    }
}
