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

        private double _zoom;

        #endregion Fields

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginViewModel"/> class.
        /// </summary>
        internal PluginViewModel()
        {
            SettingsViewModel = new(this);
            Service = AMLDatabaseService.Register();
        }

        #region Properties

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
        /// Gets the database service.
        /// </summary>
        /// <value>
        /// The service.
        /// </value>
        public AMLDatabaseService Service { get; }

        #endregion Properties

        /// <summary>
        ///  <see cref="LoadDocuments"/>
        /// </summary>
        private RelayCommand<object>? _loadDocuments;

        /// <summary>
        ///  The LoadDocuments - Command
        /// </summary>
        public System.Windows.Input.ICommand LoadDocuments => this._loadDocuments ??= new RelayCommand<object>(this.LoadDocumentsExecute, this.LoadDocumentsCanExecute);

        /// <summary>
        ///  The <see cref="LoadDocuments"/> Execution Action.
        /// </summary>
        /// <param name="parameter">
        ///  TODO The parameter.
        /// </param>
        private async void LoadDocumentsExecute(object parameter)
        {
            if (string.IsNullOrEmpty(SettingsViewModel.Database))
            {
                return;
            }
            var result = await Service.GetDocumentListAsync(SettingsViewModel.Database);
            
            Documents = result.ToList();
            RaisePropertyChanged(nameof(Documents));
        }

        public List<DocumentInfo> Documents { get; private set; }

        /// <summary>
        ///  Test, if the <see cref="LoadDocuments"/> can execute.
        /// </summary>
        /// <param name="parameter">
        ///  TODO The parameter.
        /// </param>
        /// <returns>
        ///  true, if command can execute
        /// </returns>
        private bool LoadDocumentsCanExecute(object parameter)
        {
            return SettingsViewModel.IsConnected && !string.IsNullOrEmpty(SettingsViewModel.Database);
        }
    }
}