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

namespace WPF_BytEdit
{
    /// <summary>
    /// Logique d'interaction pour SubWindow.xaml
    /// </summary>
    public partial class SubWindow : Window
    {
        public SubWindow()
        {
            InitializeComponent();
        }

        private void minimize_btn_png_MouseDown(object sender, MouseButtonEventArgs e)
        {
            editor.WindowState = WindowState.Minimized;
        }

        private void maximize_btn_png_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (editor.WindowState == WindowState.Maximized)
            {
                editor.WindowState = WindowState.Normal;
                maximize_btn_png.Source = new BitmapImage(
    new Uri("D:/Users/Bureau/test/WPF_BytEdit/WPF_BytEdit/images/maximize_btn.png"));
            }
            else
            {
                editor.WindowState = WindowState.Maximized;
                maximize_btn_png.Source = new BitmapImage(
    new Uri("D:/Users/Bureau/test/WPF_BytEdit/WPF_BytEdit/images/normal_btn.png"));
            }
        }

        private void close_btn_png_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void editor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            fichier_panel.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF333333"));
        }

        private void StackPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            fichier_panel.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF1A1A1A"));
        }
    }
}
