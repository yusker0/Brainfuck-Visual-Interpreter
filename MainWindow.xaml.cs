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

namespace Brainfuck_Visual_Interpreter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (WindowState == WindowState.Normal)
                {
                    WindowState = WindowState.Maximized;
                    maximizeButton.Visibility = Visibility.Hidden;
                    normalizeButton.Visibility = Visibility.Visible;
                }
                else if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                    normalizeButton.Visibility = Visibility.Hidden;
                    maximizeButton.Visibility = Visibility.Visible;
                }
            }
            else
            {
                base.OnMouseLeftButtonDown(e);

                // Begin dragging the window
                this.DragMove();
            }
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void maximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
            maximizeButton.Visibility = Visibility.Hidden;
            normalizeButton.Visibility = Visibility.Visible;
        }

        private void normalizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
            normalizeButton.Visibility = Visibility.Hidden;
            maximizeButton.Visibility = Visibility.Visible;
        }

        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
