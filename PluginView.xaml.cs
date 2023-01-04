// Copyright (c) 2022 AutomationML and Contributors. All rights reserved.
// Licensed to the AutomationML association under one or more agreements.
// The AutomationML association licenses this file to you under the MIT license.
using Aml.Editor.Plugin.BaseX.View;
using Aml.Editor.Plugin.BaseX.ViewModel;
using Aml.Editor.Plugin.Contracts;
using Aml.Editor.Plugin.WPFBase;
using Aml.Engine.CAEX;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Aml.Editor.Plugin.BaseX
{
    /// <summary>
    ///
    /// </summary>
    [ExportMetadata("Author", "Josef Prinz")]
    [ExportMetadata("Owner", "Josef Prinz")]
    [ExportMetadata("DisplayName", "AMLBaseX")]
    [ExportMetadata("Description",
        "This plugin can transfer AutomationML models between a BaseX database and the AutomationML Editor.")]
    [Export(typeof(IAMLEditorView))]
    public partial class PluginView : PluginViewBase, ISupportsUIZoom, ISupportsThemes
    {

        /// <summary>
        ///     <see cref="AboutCommand" />
        /// </summary>
        private RelayCommand<object>? aboutCommand;

        #region Constructors

        public PluginView()
        {
            InitializeComponent();
            DisplayName = "AMLBaseX";
            IsReactive = true;
            DataContext = new PluginViewModel();
            PaneImage = new BitmapImage(
                new Uri("pack://application:,,,/Aml.Editor.Plugin.BaseX;component/AMLBaseX.png"));

            Commands.Add(new PluginCommand
            {
                CommandName = "About",
                Command = AboutCommand,
                CommandToolTip = "About AMLBaseX"
            });
        }

        #endregion Constructors


        /// <summary>
        ///     The AboutCommand - Command
        /// </summary>
        /// <value>The about command.</value>
        public RelayCommand<object> AboutCommand => aboutCommand ??= new RelayCommand<object>(AboutCommandExecute, AboutCommandCanExecute);

        /// <summary>
        ///     Test, if the <see cref="AboutCommand" /> can execute.
        /// </summary>
        /// <param name="parameter">TODO The parameter.</param>
        /// <returns>true, if command can execute</returns>
        private bool AboutCommandCanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        ///     The <see cref="AboutCommand" /> Execution Action.
        /// </summary>
        /// <param name="parameter">TODO The parameter.</param>
        private void AboutCommandExecute(object parameter)
        {
            var dialog = new About { Owner = Application.Current.MainWindow };
            _ = dialog.ShowDialog();
        }

        #region Properties

        public override DockPositionEnum InitialDockPosition => DockPositionEnum.DockRight;

        public override bool CanClose => true;

        /// <summary>
        /// It is recommended to give all plugins a package name with the prefix Aml.Editor.Plugin.
        /// The package name is used as the ID of the plugin and should be unique.
        /// </summary>
        public override string PackageName => "Aml.Editor.Plugin.BaseX";

        #endregion Properties

        #region Methods

        public override void ChangeSelectedObject(CAEXBasicObject selectedObject)
        {
            base.ChangeSelectedObject(selectedObject);
            if (selectedObject == null)
            {
                return;
            }
        }

        /// <summary>
        /// The method is always called when the display mode is changed
        /// in the editor and when the plugin is activated.
        /// </summary>
        /// <param name="theme"></param>
        public void OnThemeChanged(ApplicationTheme theme)
        {
            // detect, which theme is currently used by mahapps.metro
            var applicationTheme = ControlzEx.Theming.ThemeManager.Current.DetectTheme(Application.Current);
            if (applicationTheme != null)
            {
                // change the theme for used toolkit elements. This method must be called in all xaml views
                // where aml.toolkit or aml.skins resources are used.
                Plugin.Contract.Theming.ThemeManager.ChangeTheme(this.Resources,
                    applicationTheme.BaseColorScheme);

                // change the theme for the used mahapps.metro ui elements
                ControlzEx.Theming.ThemeManager.Current.ChangeTheme(this, this.Resources, applicationTheme);
            }
        }

        private PluginViewModel ViewModel => (PluginViewModel)DataContext;

        /// <summary>
        /// Eventhandler for the zoom factor changed event.
        /// </summary>
        /// <param name="zoomFactor"></param>
        public void OnUIZoomChanged(double zoomFactor)
        {
            ViewModel.ZoomFactor = zoomFactor;
        }

        #endregion Methods

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri)
            { UseShellExecute = true });
            e.Handled = true;
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            var view = new SettingsView
            {
                DataContext = ViewModel.SettingsViewModel
            };
            view.ShowDialog();
        }
    }
}