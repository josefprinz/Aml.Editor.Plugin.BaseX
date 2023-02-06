using Aml.Editor.Plugin.BaseX.ViewModel;
using ControlzEx.Theming;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace Aml.Editor.Plugin.BaseX.View
{
    /// <summary>
    /// Interaction logic for XQueryView.xaml
    /// </summary>
    public partial class XQueryView : UserControl
    {
        /// <summary>
        ///     The folding manager
        /// </summary>
        private FoldingManager foldingManager;

        /// <summary>
        ///     The folding strategy
        /// </summary>
        private XmlFragmentFoldingStrategy foldingStrategy;


        public XQueryView()
        {
            InitializeComponent();
            ChangeTheme();

            QueryResult.TextChanged += (o,s)=>UpdateFoldingManager();
            QueryEdit.TextChanged += (o,s) =>
                ViewModel.RaisePropertyChanged(nameof(XQueryViewModel.HasNoQuery));
        }

        private void UpdateFoldingManager()
        {
            if (QueryResult.Document != null )
            {
                foldingManager ??= FoldingManager.Install(QueryResult.TextArea);
                foldingStrategy ??= new XmlFragmentFoldingStrategy();
                foldingStrategy.UpdateFoldings(foldingManager, QueryResult.Document);
                ViewModel.RaisePropertyChanged(nameof(XQueryViewModel.HasNoResult));
            }            
        }

        XQueryViewModel ViewModel => DataContext as XQueryViewModel;


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
