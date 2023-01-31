using ControlzEx.Theming;
using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace Aml.Editor.Plugin.BaseX.View
{
    /// <summary>
    /// Interaction logic for XQueryView.xaml
    /// </summary>
    public partial class XQueryView : UserControl
    {
        public XQueryView()
        {
            InitializeComponent();
            ChangeTheme();
        }

        private void LoadHighlighting(string key)
        {
            if ( key == "Light")
            { 
                LoadHLResource ("XMLLight", ".xml", "Aml.Editor.Plugin.BaseX.Resources.Light.XMLMode.xshd");
                LoadHLResource ("XQueryLight",".xq", "Aml.Editor.Plugin.BaseX.Resources.Light.XQuery.xshd");

                
                QueryEdit.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("XQueryLight");
                QueryResult.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("XMLLight");
            }
            else
            {
                LoadHLResource("XMLDark", ".xml", "Aml.Editor.Plugin.BaseX.Resources.Dark.XMLMode.xshd");
                LoadHLResource("XQueryDark", ".xq", "Aml.Editor.Plugin.BaseX.Resources.Dark.XQuery.xshd");

                QueryEdit.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("XQueryDark");
                QueryResult.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("XMLDark");
            }
        }

        private void LoadHLResource(string key, string extension, string resource)
        {
            // Load our custom highlighting definition
            IHighlightingDefinition customHighlighting;
            using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");

                using XmlReader reader = new XmlTextReader(s);
                customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                    HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }

            // and register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting(key, new[] { extension }, customHighlighting);
        }

        internal void ChangeTheme()
        {
            var applicationTheme = ThemeManager.Current.DetectTheme(Application.Current);
            if (applicationTheme != null)
            {
                LoadHighlighting (applicationTheme.BaseColorScheme);

                Plugin.Contract.Theming.ThemeManager.ChangeTheme(this.Resources, applicationTheme.BaseColorScheme);
                ThemeManager.Current.ChangeTheme(this, this.Resources, applicationTheme);
            }
        }
    }
}
