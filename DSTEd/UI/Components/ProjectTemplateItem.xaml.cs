using System;
using System.Collections.Generic;
using System.Linq;
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

namespace DSTEd.UI.Components
{
    /// <summary>
    /// ProjectTemplateItem.xaml 的交互逻辑
    /// </summary>
    public partial class ProjectTemplateItem : UserControl
    {
        static SolidColorBrush UnSelectedColor = new SolidColorBrush(Color.FromArgb(0xFF, 0x5A, 0x5A, 0x5D));
        static SolidColorBrush SelectedColor = new SolidColorBrush(Color.FromArgb(0xFF, 0x5A, 0x5A, 0x5D));//TODO: Find an another color
        public string TemplateFullPath { get; private set; }
        public string TemplateName {
            get
            {
                return Core.I18N.__(TemplateLabel.Content.ToString());
            }
        }
        private bool IsSelected = false;
        public void OnSelectionChanged()
        {
            if(IsSelected)
            {
                Background = UnSelectedColor;
            }
            else
            {
                Background = SelectedColor;
            }
        }

        public ProjectTemplateItem(System.IO.DirectoryInfo template)
        {
            InitializeComponent();
            TemplateFullPath = template.FullName;
            TemplateLabel.Content = Core.I18N.__(template.Name);
            
        }
    }
}
