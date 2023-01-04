using Aml.Editor.Plugin.BaseX.ViewModel;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using System.Windows;

namespace Aml.Editor.Plugin.BaseX.View
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : MetroWindow
    {
        public SettingsView()
        {
            InitializeComponent();

            var applicationTheme = ThemeManager.Current.DetectTheme(Application.Current);
            if (applicationTheme != null)
            {
                Plugin.Contract.Theming.ThemeManager.ChangeTheme(this.Resources, applicationTheme.BaseColorScheme);
                ThemeManager.Current.ChangeTheme(this, this.Resources, applicationTheme);
            }

            DataContextChanged += SettingsView_DataContextChanged;
        }

        private void SettingsView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is SettingsViewModel vm)
            {
                vm.Closed += (o, s) => Close();
            }
        }
    }
}