using Aml.Engine.CAEX;

namespace Aml.Editor.Plugin.BaseX.ViewModel
{
    /// <summary>
    /// This class represents a CAXDocument which was selected to
    /// be edited in the AutomationML editor
    /// </summary>
    internal class AMLDocumentViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AMLDocumentViewModel"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="isPartialLoad">if set to <c>true</c> [is partial load].</param>
        internal AMLDocumentViewModel (CAEXDocument document, bool isPartialLoad)
        {
            Document = document;
            IsPartialLoad = isPartialLoad;
        }

        /// <summary>
        /// Edits the document.
        /// </summary>
        internal void EditDocument ()
        {
            Aml.Editor.API.AMLEditor.AMLApplication.OpenAMLDocument ("");
        }

        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <value>
        /// The document.
        /// </value>
        public CAEXDocument Document { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is partially loaded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is partial load; otherwise it is completly loaded.
        /// </value>
        public bool IsPartialLoad { get; }
    }
}
