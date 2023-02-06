using Aml.Editor.MVVMBase;
using Aml.Editor.Plugin.Contracts;
using Aml.Engine.Services.BaseX.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aml.Editor.Plugin.BaseX.ViewModel
{
    internal class SettingsViewModel : ViewModelBase
    {
        internal event EventHandler? Closed;

        #region Fields

        /// <summary>
        ///  <see cref="BaseXServer"/>
        /// </summary>
        private string? _baseXServer;

        /// <summary>
        ///  <see cref="ConnectCommand"/>
        /// </summary>
        private RelayCommand<object>? _connectCommand;

        /// <summary>
        ///  <see cref="ConnectionError"/>
        /// </summary>
        private string? _connectionError;

        /// <summary>
        ///  <see cref="IsConnected"/>
        /// </summary>
        private bool _isConnected;

        /// <summary>
        ///  <see cref="Password"/>
        /// </summary>
        private string? _password;

        /// <summary>
        ///  <see cref="Username"/>
        /// </summary>
        private string? _username;

        private DatabaseInfo _selectedDatabase;

        #endregion Fields

        #region Constructors

        public SettingsViewModel(PluginViewModel pluginViewModel)
        {
            PluginViewModel = pluginViewModel;
#if DEBUG
            BaseXServer = "http://localhost:8080/rest/";
            Username = "admin";
            Password = "josef";
#endif
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        ///  Gets and sets the BaseXServer
        /// </summary>
        public string? BaseXServer
        {
            get => _baseXServer;
            set => Set(ref _baseXServer, value);
        }

        /// <summary>
        ///  The ConnectCommand - Command
        /// </summary>
        public System.Windows.Input.ICommand ConnectCommand => _connectCommand ??= new RelayCommand<object>(this.ConnectCommandExecute, this.ConnectCommandCanExecute);

        /// <summary>
        ///  Gets and sets the ConnectionError
        /// </summary>
        public string? ConnectionError
        {
            get => _connectionError;
            set => Set(ref _connectionError, value);
        }

        /// <summary>
        ///  Gets and sets the Database
        /// </summary>
        public string? Database => Databases?.Count > 0 ? SelectedDatabase.Name : null;

        /// <summary>
        ///  Gets and sets the IsConnected
        /// </summary>
        public bool IsConnected
        {
            get => _isConnected;
            set => Set(ref _isConnected, value);
        }

        /// <summary>
        ///  Gets and sets the Password
        /// </summary>
        public string? Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        public PluginViewModel PluginViewModel { get; }

        /// <summary>
        ///  Gets and sets the Username
        /// </summary>
        public string? Username
        {
            get => _username;
            set => Set(ref _username, value);
        }

        public List<DatabaseInfo>? Databases { get; set; }

        public DatabaseInfo SelectedDatabase
        {
            get => _selectedDatabase;
            set
            {
                Set(ref _selectedDatabase, value);
                RaisePropertyChanged(nameof(Database));
                _closeCommand?.RaiseCanExecuteChanged();
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        ///  <see cref="CloseCommand"/>
        /// </summary>
        private RelayCommand<object>? _closeCommand;

        /// <summary>
        ///  The CloseCommand - Command
        /// </summary>
        public System.Windows.Input.ICommand CloseCommand => this._closeCommand ??= new RelayCommand<object>(this.CloseCommandExecute, this.CloseCommandCanExecute);

        /// <summary>
        ///  The <see cref="CloseCommand"/> Execution Action.
        /// </summary>
        /// <param name="parameter">
        ///  TODO The parameter.
        /// </param>
        private void CloseCommandExecute(object parameter)
        {
            Closed?.Invoke(this, EventArgs.Empty);

            if (!string.IsNullOrEmpty(Database))
            {
                PluginViewModel.LoadDocumentsCommand.Execute (null);
            }
        }

        /// <summary>
        ///  Test, if the <see cref="CloseCommand"/> can execute.
        /// </summary>
        /// <param name="parameter">
        ///  TODO The parameter.
        /// </param>
        /// <returns>
        ///  true, if command can execute
        /// </returns>
        private bool CloseCommandCanExecute(object parameter)
        {
            return !string.IsNullOrEmpty(Database);
        }

        /// <summary>
        ///  Test, if the <see cref="ConnectCommand"/> can execute.
        /// </summary>
        /// <param name="parameter">
        ///  not used
        /// </param>
        /// <returns>
        ///  true, if command can execute
        /// </returns>
        private bool ConnectCommandCanExecute(object parameter)
        {
            return !IsConnected &&
                !string.IsNullOrEmpty(Username) &&
                !string.IsNullOrEmpty(Password) &&
                !string.IsNullOrEmpty(BaseXServer);
        }

        /// <summary>
        ///  The <see cref="ConnectCommand"/> Execution Action.
        /// </summary>
        /// <param name="parameter">
        ///  not used.
        /// </param>
        private async void ConnectCommandExecute(object parameter)
        {
            if (string.IsNullOrEmpty(Username) ||
                string.IsNullOrEmpty(Password) ||
                string.IsNullOrEmpty(BaseXServer))
            {
                return;
            }

            var result = await PluginViewModel.Service.Connect(BaseXServer, Username, Password);
            if (!result.Any())
            {
                IsConnected = false;
                ConnectionError = PluginViewModel.Service.ErrorMessage;
                return;
            }

            IsConnected = true;
            Databases = result.ToList();

            RaisePropertyChanged(nameof(Databases));
            _connectCommand?.RaiseCanExecuteChanged();
        }

        #endregion Methods
    }
}