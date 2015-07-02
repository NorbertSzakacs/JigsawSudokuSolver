using DesktopApp.Controller;
using DesktopApp.Structure;
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
using System.Windows.Shapes;

namespace DesktopApp
{
    /// <summary>
    /// Interaction logic for OpenMap.xaml
    /// </summary>
    public partial class OpenMap : Window
    {
        MapController mapCtrl;

        public OpenMap(MapController mc)
        {
            InitializeComponent();

            mapCtrl = mc;

            titlePanel.MouseLeftButtonDown += new MouseButtonEventHandler(Window_MouseDown);
            exitIcon.MouseLeftButtonDown += new MouseButtonEventHandler(Window_Exit);

            RenderListBox();
        }

        private void RenderListBox()
        {
            mapList.ItemsSource = mapCtrl.Problems;
        }

        private void Window_Exit(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnOpenMap_Click(object sender, RoutedEventArgs e)
        {
            mapCtrl.GenerateMap((Problem)mapList.SelectedItem);
        }
    }


    public class TodoItem
    {
        public string Title { get; set; }
        public int Completion { get; set; }
    }
}
