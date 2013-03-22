using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.Common;
using Shared.Utilities.Enums;

namespace ThreadSafetyAnnotations.Engine
{
    public class Issue
    {
        private string _description;
        private ErrorCode _errorCode;
        private CommonSyntaxNode _syntaxNode;
        private ISymbol _symbol;
        private string _sourceLineText;
        private int _sourceLineNumber;
        private string _sourceFileName;
        private CommonLocation _location;

        public Issue(
            ErrorCode errorCode,
            CommonSyntaxNode syntaxNode, 
            ISymbol symbol)
        {
            #region Input validtion

            if (!Enum.IsDefined(typeof(ErrorCode), errorCode))
            {
                throw new ArgumentException("errorCode is not defined.", "errorCode");
            }

            if (syntaxNode == null)
            {
                throw new ArgumentNullException("syntaxNode");
            }

            if (symbol == null)
            {
                throw new ArgumentNullException("symbol");
            }

            #endregion

            _errorCode = errorCode;
            _description = DescriptionResolver.GetDescription(errorCode);
            _syntaxNode = syntaxNode;
            _symbol = symbol;
            _location = _symbol.Locations.First();

            _sourceLineNumber = GetSourceLineNumber();
            _sourceLineText = GetSourceLineText(_sourceLineNumber);
            _sourceFileName = GetSourceFileName();
        }

        private string GetSourceLineText(int lineNumber)
        {
            return _location.SourceTree.GetText().GetLineFromLineNumber(lineNumber).Extent.ToString().Trim();
        }

        private int GetSourceLineNumber()
        {
            return _location.SourceTree.GetLineSpan(_location.SourceSpan, false).StartLinePosition.Line;
        }

        private string GetSourceFileName()
        {
            return _location.SourceTree.FilePath;
        }

        public string Description { get { return _description; } }
        public ErrorCode ErrorCode { get { return _errorCode; } }
        public CommonSyntaxNode SyntaxNode { get { return _syntaxNode; } }
        public string SourceFileName { get { return _sourceFileName; } }
        public string SourceLineText { get { return _sourceLineText; } }
        public int SourceLineNumber { get { return _sourceLineNumber; } }
    }
}
