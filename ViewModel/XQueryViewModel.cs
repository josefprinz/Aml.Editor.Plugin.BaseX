using Aml.Editor.MVVMBase;
using Aml.Editor.Plugin.Contracts;
using ICSharpCode.AvalonEdit.Document;
using Microsoft.Win32;
using System;
using System.IO;

namespace Aml.Editor.Plugin.BaseX.ViewModel
{
    internal class XQueryViewModel : ViewModelBase
    {
        #region Fields

        /// <summary>
        ///  <see cref="QueryResultDocument"/>
        /// </summary>
        private TextDocument? _queryResultDocument;

        /// <summary>
        ///  <see cref="XQueryDocument"/>
        /// </summary>
        private TextDocument? _xQueryDocument;

        private PluginViewModel? _pluginViewModel;
        private string? _xqueryFile;

        #endregion Fields

        #region Constructors

        public XQueryViewModel(PluginViewModel pluginViewModel)
        {
            this._pluginViewModel = pluginViewModel;
            XQueryDocument = new();
            QueryResultDocument = new();
            ClearCommand = new(ClearCommandExecute);
            LoadQueryCommand = new(LoadQueryCommandExecute, LoadQueryCommandCanExecute);
            RunQueryCommand = new(RunQueryCommandExecute, RunQueryCommandCanExecute);
            AddRootQueryCommand = new(AddRootQueryCommandExecute, AddRootQueryCommandCanExecute);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        ///  The LoadQueryCommand - Command
        /// </summary>
        public RelayCommand<object> LoadQueryCommand { get; }

        public RelayCommand<object> ClearCommand { get; }

        public RelayCommand<object> RunQueryCommand { get; }

        public RelayCommand<object> AddRootQueryCommand { get; }


        private void AddRootQueryCommandExecute(object parameter)
        {
            if (XQueryDocument == null || _pluginViewModel == null || _pluginViewModel.SelectedDocument == null) 
            { 
                return; 
            }
            XQueryDocument.Text = $"let $root:= doc('{_pluginViewModel.SettingsViewModel.Database}/{_pluginViewModel.SelectedDocument.Name}')/CAEXFile";
        }

        private bool AddRootQueryCommandCanExecute(object parameter)
        {
            return IsConnected;
        }

        private async void RunQueryCommandExecute(object parameter)
        {
            if (_pluginViewModel != null && IsConnected && XQueryDocument?.Text != null)
            { 
                QueryResultDocument.Text = "";
                _pluginViewModel.Error = "";

                var result = await _pluginViewModel.Service.RunQueryAsync (_pluginViewModel.SettingsViewModel.Database, XQueryDocument.Text);

                if (!string.IsNullOrEmpty(result )) 
                { 
                    QueryResultDocument.Text = result;
                }
                else
                {
                    _pluginViewModel.Error = _pluginViewModel.Service.ErrorMessage;
                }

                RaisePropertyChanged (nameof(HasNoResult));
            }
        }

        private bool IsConnected => _pluginViewModel?.SettingsViewModel.IsConnected==true && 
            !string.IsNullOrEmpty(_pluginViewModel.SettingsViewModel.Database);

        private bool RunQueryCommandCanExecute(object parameter)
        {
            return IsConnected && !HasNoQuery;
        }

        private void ClearCommandExecute(object parameter)
        {
            XQueryDocument.Text = string.Empty;
        }

        /// <summary>
        ///  The <see cref="LoadQueryCommand"/> Execution Action.
        /// </summary>
        /// <param name="parameter">
        ///  TODO The parameter.
        /// </param>
        private void LoadQueryCommandExecute(object parameter)
        {
            var dlg = new OpenFileDialog
            {
                DefaultExt = ".xq", // Default file extension
                Filter = "XQuery documents (.xq)|*.xq"
            };

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                _xqueryFile = dlg.FileName;
                var text = File.ReadAllText(_xqueryFile);
                XQueryDocument.Text = text;

                RaisePropertyChanged(nameof(HasNoQuery));
            }
        }

        public bool HasNoQuery => XQueryDocument?.Text == null || XQueryDocument.TextLength == 0;

        public bool HasNoResult => QueryResultDocument?.Text == null || QueryResultDocument.TextLength == 0;

        /// <summary>
        ///  Test, if the <see cref="LoadQueryCommand"/> can execute.
        /// </summary>
        /// <param name="parameter">
        ///  TODO The parameter.
        /// </param>
        /// <returns>
        ///  true, if command can execute
        /// </returns>
        private bool LoadQueryCommandCanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        ///  Gets and sets the QueryResultDocument
        /// </summary>
        public TextDocument? QueryResultDocument
        {
            get => _queryResultDocument;
            set => Set(ref _queryResultDocument, value);
        }

        /// <summary>
        ///  Gets and sets the XQueryDocument
        /// </summary>
        public TextDocument? XQueryDocument
        {
            get => _xQueryDocument;
            set => Set(ref _xQueryDocument, value);
        }

        #endregion Properties
    }
}