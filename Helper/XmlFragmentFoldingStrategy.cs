
using ICSharpCode.AvalonEdit.Document;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace ICSharpCode.AvalonEdit.Folding
{
    /// <summary>
    ///     Determines folds for an xml string in the editor.
    /// </summary>
    public class XmlFragmentFoldingStrategy : XmlFoldingStrategy
    {
        #region Public Methods

        /// <summary>
        ///     Create <see cref="NewFolding" /> s for the specified document.
        /// </summary>
        public new IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
        {
            try
            {
                var readerSettings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment };

                using var reader = XmlReader.Create(document.CreateReader(), readerSettings);
                return CreateNewFoldings(document, reader, out firstErrorOffset);
            }
            catch (XmlException)
            {
                firstErrorOffset = 0;
                return Enumerable.Empty<NewFolding>();
            }
        }

        /// <summary>
        ///     Create <see cref="NewFolding" /> s for the specified document and updates the folding
        ///     manager with them.
        /// </summary>
        public new void UpdateFoldings(FoldingManager manager, TextDocument document)
        {
            var foldings = CreateNewFoldings(document, out var firstErrorOffset);
            manager.UpdateFoldings(foldings, firstErrorOffset);
        }

        #endregion Public Methods
    }
}