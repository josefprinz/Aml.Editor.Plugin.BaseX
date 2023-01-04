using Aml.Editor.MVVMBase;
using Aml.Editor.Plugin.Contracts;
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
        ///  <see cref="LoadDocumentsCommand"/>
        /// </summary>
        private RelayCommand<object>? _loadDocumentsCommand;

        /// <summary>
        ///  <see cref="SelectedDocument"/>
        /// </summary>
        private DocumentInfo _selectedDocument;

        private double _zoom;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginViewModel"/> class.
        /// </summary>
        internal PluginViewModel()
        {
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
        public string? DocumentsBase
        {
            get => SettingsViewModel.IsConnected ?
                    SettingsViewModel.Database : "no database";
        }

        /// <summary>
        ///  The LoadDocuments - Command
        /// </summary>
        public System.Windows.Input.ICommand LoadDocumentsCommand => this._loadDocumentsCommand ??= new RelayCommand<object>(this.LoadDocumentsCommandExecute, this.LoadDocumentsCommandCanExecute);

        /// <summary>
        ///  Gets and sets the SelectedDocument
        /// </summary>
        public DocumentInfo SelectedDocument
        {
            get => _selectedDocument;
            set => Set(ref _selectedDocument, value);
        }

        /// <summary>
        /// Gets the database service.
        /// </summary>
        /// <value>
        /// The service.
        /// </value>
        public AMLDatabaseService Service { get; }

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
            get => _zoom;
            set => Set(ref _zoom, value);
        }

        #endregion Properties

        #region Methods

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