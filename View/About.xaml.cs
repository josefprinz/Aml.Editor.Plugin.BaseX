using ControlzEx.Theming;
using MahApps.Metro.Controls;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace Aml.Editor.Plugin.BaseX.View
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : MetroWindow
    {
        #region Public Constructors

        public About()
        {
            DataContext = this;

            var ass = Assembly.GetAssembly(GetType());
            if (ass != null)
            {
                var attributes = ass.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true);
                if (attributes?[0] != null)
                {
                    Version = (attributes[0] as AssemblyFileVersionAttribute)?.Version;
                }
            }

            InitializeComponent();

            var applicationTheme = ThemeManager.Current.DetectTheme(Application.Current);
            if (applicationTheme != null)
            {
                Plugin.Contract.Theming.ThemeManager.ChangeTheme(this.Resources, applicationTheme.BaseColorScheme);
                ThemeManager.Current.ChangeTheme(this, this.Resources, applicationTheme);
            }
        }

        #endregion Public Constructors

        #region Public Properties

        public string? Version { get; set; }

        #endregion Public Properties

        #region Private Methods

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                ProcessStartInfo psi = new()
                {
                    FileName = e.Uri.ToString(),
                    UseShellExecute = true
                };

                Process.Start(psi);
            }
            catch { }
        }

        #endregion Private Methods
    }
}