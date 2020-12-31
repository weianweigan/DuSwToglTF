using DuSwToglTF.ViewModel;
using SolidWorks.Interop.sldworks;
using System.Windows;

namespace DuSwToglTF.View
{
    /// <summary>
    /// ExportWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ExportWindow : Window
    {
        private ExportWindowViewModel _vm;

        public ExportWindow()
        {
            InitializeComponent();
            
        }

        internal void Init(ModelDoc2 doc)
        {
            _vm = new ExportWindowViewModel(doc);
            DataContext = _vm;
        }
    }
}
