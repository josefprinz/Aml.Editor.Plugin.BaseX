using Aml.Editor.API;
using Aml.Editor.MVVMBase;
using Aml.Editor.Plugin.Contracts;
using Aml.Engine.CAEX;
using Aml.Engine.Services.BaseX;
using Aml.Engine.Services.BaseX.Model;
using System.Collections.Generic;
using System.Linq;

namespace Aml.Editor.Plugin.BaseX.ViewModel
{
    internal class PluginViewModel : ViewModelBase
    {

        #region Fields

        /// <summary>
        ///  <see cref="EditSelectedDocumentCommand"/>
        /// </summary>    
        private RelayCommand<object>? _editSelectedDocumentCommand;

        /// <summary>
        ///  <see cref="Error"/>
        /// </summary>    
        private string? _error = null;

        /// <summary>
        ///  <see cref="LoadDocumentsCommand"/>
        /// </summary>
        private RelayCommand<object>? _loadDocumentsCommand;

        /// <summary>
        ///  <see cref="SelectedDocument"/>
        /// </summary>
        private DocumentInfo? _selectedDocument;

        /// <summary>
        /// <see cref="ZoomFactor"/>
        /// </summary>
        private double _zoomFactor;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginViewModel"/> class.
        /// </summary>
        internal PluginViewModel()
        {
            QueryViewModel = new(this);
            SettingsViewModel = new(this);
            Service = AMLDatabaseService.Register();
            Documents = new();
        }

        #endregion Constructors

        #region Properties

        public List<DocumentInfo> Documents { get; private set; }

        /// <summary>
        ///  Gets and sets the DocumentsBase
        /// </summary>
        public string? DocumentsBase =>
            SettingsViewModel.IsConnected ?
                SettingsViewModel.Database : "no database";

        /// <summary>
        ///  The EditSelectedDocumentCommand - Command
        /// </summary>
        public System.Windows.Input.ICommand EditSelectedDocumentCommand => this._editSelectedDocumentCommand ??= new RelayCommand<object>(this.EditSelectedDocumentCommandExecute, this.EditSelectedDocumentCommandCanExecute);

        /// <summary>
        ///  Gets and sets the Error
        /// </summary>
        public string? Error
        {
            get => _error;
            set => Set(ref _error, value);
        }

        /// <summary>
        ///  The LoadDocuments - Command
        /// </summary>
        public System.Windows.Input.ICommand LoadDocumentsCommand => this._loadDocumentsCommand ??= new RelayCommand<object>(this.LoadDocumentsCommandExecute, this.LoadDocumentsCommandCanExecute);

        /// <summary>
        ///  Gets and sets the SelectedDocument
        /// </summary>
        public DocumentInfo? SelectedDocument
        {
            get => _selectedDocument;
            set {
                Set(ref _selectedDocument, value);
                _editSelectedDocumentCommand?.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets the database service.
        /// </summary>
        /// <value>
        /// The service.
        /// </value>
        public AMLDatabaseService Service { get; }


        public XQueryViewModel QueryViewModel { get; }

        /// <summary>
        /// Gets the settings view model.
        /// </summary>
        /// <value>
        /// The settings view model.
        /// </value>
        public SettingsViewModel SettingsViewModel
        {
            get;
        }

        /// <summary>
        /// Gets or sets the UI zoom factor.
        /// </summary>
        /// <value>
        /// The zoom factor.
        /// </value>
        public double ZoomFactor
        {
            get => _zoomFactor;
            set => Set(ref _zoomFactor, value);
        }

        #endregion Properties

        #region Methods

        /// <summary>
        ///  Test, if the <see cref="EditSelectedDocumentCommand"/> can execute.
        /// </summary>
        /// <param name="parameter">
        ///  TODO The parameter. 
        /// </param>  
        /// <returns>
        ///  true, if command can execute
        /// </returns> 
        private bool EditSelectedDocumentCommandCanExecute(object parameter)
        {
            return SettingsViewModel!=null && !string.IsNullOrEmpty(SettingsViewModel.Database) && SelectedDocument != null;
        }

        /// <summary>
        ///  The <see cref="EditSelectedDocumentCommand"/> Execution Action.
        /// </summary>
        /// <param name="parameter">
        ///  TODO The parameter. 
        /// </param>        
        private async void EditSelectedDocumentCommandExecute(object parameter)
        {
            if (SettingsViewModel == null || 
                SelectedDocument == null ||
                string.IsNullOrEmpty(SettingsViewModel.Database) ||
                parameter is not string type)
            {
                return;
            }

            Error = string.Empty;
            CAEXDocument? document;
            if (type=="Partial")
            {
                document = await Service.LoadCAEXFileHeaderAsCAEXDocumentAsync (SettingsViewModel.Database, SelectedDocument.Name);
                if (document != null && document.CAEXFile.Exists)
                {
                    Service.Elements(document.CAEXFile.Node, CAEX_CLASSModel_TagNames.ROLECLASSLIB_STRING, true).ToList();
                    Service.Elements(document.CAEXFile.Node, CAEX_CLASSModel_TagNames.SYSTEMUNITCLASSLIB_STRING, true).ToList();
                    Service.Elements(document.CAEXFile.Node, CAEX_CLASSModel_TagNames.INTERFACECLASSLIB_STRING, true).ToList();
                    Service.Elements(document.CAEXFile.Node, CAEX_CLASSModel_TagNames.ATTRIBUTETYPELIB_STRING, true).ToList();
                    Service.Elements(document.CAEXFile.Node, CAEX_CLASSModel_TagNames.INSTANCEHIERARCHY_STRING, true).ToList();
                }
            }
            else
            {
                document = await Service.LoadCAEXDocumentAsync(SettingsViewModel.Database, SelectedDocument.Name);
            }

            if (document == null)
            {
                Error = $"Service error: {Service.ErrorMessage}. Cannot load {SelectedDocument.Name} from {SettingsViewModel.Database}";
            }
            else if (!document.CAEXFile.Exists)
            {
                Error = $"Document error: CAEXFile content not loaded. Cannot load {SelectedDocument.Name} from {SettingsViewModel.Database}";
            }
            AMLEditor.AMLApplication.EditAMLDocument(document);
        }
        /// <summary>
        ///  Test, if the <see cref="LoadDocumentsCommand"/> can execute.
        /// </summary>
        /// <param name="parameter">
        ///  TODO The parameter.
        /// </param>
        /// <returns>
        ///  true, if command can execute
        /// </returns>
        private bool LoadDocumentsCommandCanExecute(object parameter)
        {
            return SettingsViewModel.IsConnected && !string.IsNullOrEmpty(SettingsViewModel.Database);
        }

        /// <summary>
        ///  The <see cref="LoadDocumentsCommand"/> Execution Action.
        /// </summary>
        /// <param name="parameter">
        ///  TODO The parameter.
        /// </param>
        private async void LoadDocumentsCommandExecute(object parameter)
        {
            if (string.IsNullOrEmpty(SettingsViewModel.Database))
            {
                return;
            }
            var result = await Service.GetDocumentListAsync(SettingsViewModel.Database);

            Documents = result.ToList();
            RaisePropertyChanged(nameof(Documents));
            RaisePropertyChanged(nameof(DocumentsBase));
            SelectedDocument = Documents.FirstOrDefault();
        }

        #endregion Methods

    }
}