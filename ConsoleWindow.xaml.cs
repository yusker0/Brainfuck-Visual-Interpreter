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
using Brainfuck_Visual_Interpreter;

namespace Brainfuck_Visual_Interpreter
{
    /// <summary>
    /// Логика взаимодействия для Console.xaml
    /// </summary>
    public partial class Console : Window
    {
        bool isCtrl = false;
        bool isInput = false;
        MainWindow mainWindow;
        public Console(BFCode code)
        {
            InitializeComponent();
            consoleText.Text = code.filepath + " > ";
            mainWindow = Application.Current.Windows[0] as MainWindow;

            UpdateTimeAsync();
        }
        private void header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                normalizeButton.Visibility = Visibility.Hidden;
                maximizeButton.Visibility = Visibility.Visible;
            }
            else if (e.ClickCount == 2 && WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
                maximizeButton.Visibility = Visibility.Hidden;
                normalizeButton.Visibility = Visibility.Visible;
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
            this.Close();
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

        private void Console_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl)
                isCtrl=true;
        }
        private void Console_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl)
                isCtrl = false;
        }
        private void zoomConsole(object sender, MouseWheelEventArgs wheelE)
        {

            if (isCtrl)
            {
                int delta = (wheelE.Delta > 0 ? 1 : -1);
                if (consoleText.FontSize + delta > 12 && consoleText.FontSize + delta < 36)
                    consoleText.FontSize += delta;//*/
            }
        }

        private void consoleBlock_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            if (consoleText.ActualHeight < (this.Height - 30) * 0.964)
                consoleScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            else
                consoleScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible; //*/
        }

        private void Console_Closed(object sender, EventArgs e)
        {

            mainWindow.code = new BFCode(mainWindow.code.filepath, mainWindow);
            mainWindow.codeBlock.Inlines.Clear();
            mainWindow.codeBlock.Inlines.Add(new Run(mainWindow.code.code.Substring(0, mainWindow.code.chPtr)));
            mainWindow.codeBlock.Inlines.Add(new Run(mainWindow.code.code.Substring(mainWindow.code.chPtr, 1)) { Foreground = Brushes.Black });
            mainWindow.codeBlock.Inlines.Add(new Run(mainWindow.code.code.Substring(mainWindow.code.chPtr + 1)));
            mainWindow.sizeElements(true);

            mainWindow.runButton.Content = "Run";
            mainWindow.playButton.IsEnabled = false;


            mainWindow.code.isPlaying = false;
            mainWindow.nextStepButton.IsEnabled = false;
            mainWindow.cancelStepButton.IsEnabled = false;
            mainWindow.playButton.Content = "Play";
        }
        private async void UpdateTimeAsync()
        {
            while (true)
            {
                consoleTime.Text = String.Format("{0:d} {0:t}", DateTime.Now); 
                await Task.Delay(60000);
            }
        }
    }
}
