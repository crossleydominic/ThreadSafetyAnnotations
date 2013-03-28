using Roslyn.Compilers.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadSafetyAnnotations.Engine.Info
{
    public class MemberInfo : BaseInfo<MemberDeclarationSyntax, Symbol>
    {
        private List<BlockSyntax> _blocks;
        private ClassInfo _parent;

        public MemberInfo(
            MemberDeclarationSyntax declaration,
            Symbol symbol,
            SemanticModel model, IEnumerable<BlockSyntax> blocks) :
                base(declaration, symbol, model)
        {
            _blocks= new List<BlockSyntax>();
            _blocks.AddRange(blocks);
        }

        public IReadOnlyList<BlockSyntax> Blocks{get { return _blocks; }}
        public ClassInfo Parent { get { return _parent; } set { _parent = value; } }
    }
}
