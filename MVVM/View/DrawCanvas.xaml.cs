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

namespace Paint.MVVM.View
{
    /// <summary>
    /// Interaction logic for DrawCanvas.xaml
    /// </summary>
    public partial class DrawCanvas : UserControl
    {
        private InkCanvas _ThisCanvas;

        public InkCanvas ThisCanvas
        {
            get { return _ThisCanvas; }
            set { _ThisCanvas = value; }
        }

        public DrawCanvas()
        {
            InitializeComponent();
            
        }
    }
}
