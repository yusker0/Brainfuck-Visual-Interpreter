using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

namespace Brainfuck_Visual_Interpreter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    public partial class MainWindow : Window
    {
        public BFCode code;
        public Console console;
        int linesCount;

        public TextBlock[,] panels;
        
        Brush panelBrush = new SolidColorBrush() { Color = Color.FromRgb(102,132,212)};
        
        public MainWindow()
        {
            InitializeComponent();    // Case 1: Color one character from the current caret 
            code = new BFCode("default", this);
            linesCount = Math.Max((int)Math.Ceiling((decimal)code.MAX/7), 7);
            sizeElements(true);
        }
        private void header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                normalizeButton.Visibility = Visibility.Hidden;
                maximizeButton.Visibility = Visibility.Visible;
                sizeElements();
            }
            else if (e.ClickCount == 2 && WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
                maximizeButton.Visibility = Visibility.Hidden;
                normalizeButton.Visibility = Visibility.Visible;
                sizeElements();
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
            sizeElements();
        }

        private void normalizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
            normalizeButton.Visibility = Visibility.Hidden;
            maximizeButton.Visibility = Visibility.Visible;
            sizeElements();
        }

        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void fileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Brainfuck files (*.b, *.bf)|*.b;*.bf";
            if (openFileDialog.ShowDialog() == true)
            {
                string filepath = openFileDialog.FileName.ToString();
                filename.Text = System.IO.Path.GetFileName(filepath);
                code = new BFCode(filepath, this);
                codeBlock.Text = code.code;
                linesCount = Math.Max((int)Math.Ceiling((decimal)code.MAX / 15), 7);
                sizeElements(true);

                runButton.IsEnabled = true;
            }
        }

        private void runButton_Click(object sender, RoutedEventArgs e)
        {
            if (runButton.Content.ToString() == "Run")
            {
                console = new Console(code);
                console.Show();
                codeBlock.Inlines.Clear();
                codeBlock.Inlines.Add(new Run(code.code.Substring(0, code.chPtr)));
                codeBlock.Inlines.Add(new Run(code.code.Substring(code.chPtr, 1)) { Foreground = Brushes.Black });
                codeBlock.Inlines.Add(new Run(code.code.Substring(code.chPtr + 1)));

                playButton.IsEnabled = true;
                nextStepButton.IsEnabled = true;
                speedPlusButton.IsEnabled= true;
                speedMinusButton.IsEnabled= true;
                
                runButton.Content = "Stop";
            }
            else
            {
                code = new BFCode(code.filepath, this);
                codeBlock.Inlines.Clear();
                codeBlock.Inlines.Add(new Run(code.code.Substring(0, code.chPtr)));
                codeBlock.Inlines.Add(new Run(code.code.Substring(code.chPtr, 1)) { Foreground = Brushes.Black });
                codeBlock.Inlines.Add(new Run(code.code.Substring(code.chPtr + 1)));
                sizeElements(true);


                playButton.IsEnabled = false;
                speedPlusButton.IsEnabled = false;
                speedMinusButton.IsEnabled = false;
                nextStepButton.IsEnabled = false;

                console.Close();
                runButton.Content = "Run";
            }
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (playButton.Content.ToString() == "Play")
            {
                nextStepButton.IsEnabled = false;
                cancelStepButton.IsEnabled = false;

                playButton.Content = "Pause";
                code.isPlaying = true;
                code.playAsync();
            }
            else
            {
                code.isPlaying = false;
                nextStepButton.IsEnabled = true;
                if(code.chPtr > 0)
                   cancelStepButton.IsEnabled = true;
                playButton.Content = "Play";
            }

        }

        private void speedPlusButton_Click(object sender, RoutedEventArgs e)
        {
            if (code.speed < code.maxSpeed)
            {
                code.speed += 1;
                if (code.speed >= code.maxSpeed)
                {
                    speedPlusButton.IsEnabled = false;
                }
                speedMinusButton.IsEnabled= true;

                if (code.speed > 1)
                {
                    speedPlusTip.Content = String.Format("{0} steps/second", Math.Pow(2, code.speed));
                    speedMinusTip.Content = String.Format("{0} steps/second", Math.Pow(2, code.speed));
                }
                else
                {
                    speedPlusTip.Content = String.Format("{0} step/second", Math.Pow(2, code.speed));
                    speedMinusTip.Content = String.Format("{0} step/second", Math.Pow(2, code.speed));
                }
            }
        }

        private void speedMinusButton_Click(object sender, RoutedEventArgs e)
        {

            if (code.speed > code.minSpeed)
            {
                code.speed -= 1;
                if (code.speed <= code.minSpeed)
                {
                    speedMinusButton.IsEnabled = false;
                }
                speedPlusButton.IsEnabled = true;

                if (code.speed > 1)
                {
                    speedPlusTip.Content = String.Format("{0} steps/second", Math.Pow(2, code.speed));
                    speedMinusTip.Content = String.Format("{0} steps/second", Math.Pow(2, code.speed));
                }
                else
                {
                    speedPlusTip.Content = String.Format("{0} step/second", Math.Pow(2, code.speed));
                    speedMinusTip.Content = String.Format("{0} step/second", Math.Pow(2, code.speed));
                }
            }
        }

        private void nextStepButton_Click(object sender, RoutedEventArgs e)
        {
            code.nextStep();
            cancelStepButton.IsEnabled=true;
            if (code.chPtr < code.code.Length)
            {
                codeBlock.Inlines.Clear();
                codeBlock.Inlines.Add(new Run(code.code.Substring(0, code.chPtr)));
                codeBlock.Inlines.Add(new Run(code.code.Substring(code.chPtr, 1)) { Foreground = Brushes.Black });
                codeBlock.Inlines.Add(new Run(code.code.Substring(code.chPtr + 1)));
            }
            else
                nextStepButton.IsEnabled = false;
        }

        private void cancelStepButton_Click(object sender, RoutedEventArgs e)
        {
            if (code.chPtr > 0)
            {
                code.cancelStep();
                nextStepButton.IsEnabled = true;
                cancelStepButton.IsEnabled = false;
            }
            codeBlock.Inlines.Clear();
            codeBlock.Inlines.Add(new Run(code.code.Substring(0, code.chPtr)));
            codeBlock.Inlines.Add(new Run(code.code.Substring(code.chPtr, 1)) { Foreground = Brushes.Black });
            codeBlock.Inlines.Add(new Run(code.code.Substring(code.chPtr + 1)));
        }

        public void sizeElements(bool resetLines = false)
        {
            body.Height = (this.Height - 30)*0.964 + (linesCount - 7)*(((this.Height - 30) * 0.992)/7);
            if (resetLines)
            {
                bodyScroll.VerticalScrollBarVisibility = linesCount > 7 ? ScrollBarVisibility.Visible : ScrollBarVisibility.Hidden;

                Border[,] borders = new Border[linesCount, 15];
                panels = new TextBlock[linesCount, 15];
                ImageSource pointerSource = pointer.Source;
                body.Children.Clear();
                body.ColumnDefinitions.Clear();
                body.RowDefinitions.Clear();

                body.ColumnDefinitions.Add(new ColumnDefinition() {Width = new GridLength(0.26, GridUnitType.Star)});
                for (int i = 0; i < 15; i++)
                {
                    body.ColumnDefinitions.Add(new ColumnDefinition() {Width = new GridLength(1, GridUnitType.Star) });
                }
                for (int row = 0; row < linesCount; row++)
                {
                    body.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.45, GridUnitType.Star) });
                    body.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    for (int column = 0; column < 15; column++)
                    {
                        panels[row, column] = new TextBlock{Foreground = Brushes.White, Text = "0"};
                        borders[row, column] = new Border
                        {
                            Background = panelBrush,
                            BorderBrush = Brushes.Black,
                            BorderThickness = column == 0 ? new Thickness(2, 2, 1, 2) : column == 14 ? new Thickness(1, 2, 2, 2) : new Thickness(1, 2, 1, 2),
                            Child = new Viewbox{Child = panels[row, column]}
                        };
                        Grid.SetRow(borders[row, column], row*2 + 1 );
                        Grid.SetColumn(borders[row, column], column + 1);
                        body.Children.Add(borders[row, column]);

                    }
                }
                pointer = new Image() { Source = pointerSource};
                pointer.SetValue(Grid.RowProperty, 0);
                pointer.SetValue(Grid.ColumnProperty, 1);
                body.Children.Add(pointer);
            }
        }

        private void codeBlock_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (codeBlock.ActualHeight < (this.Height - 30) * 0.964)
                sidebarScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            else
                sidebarScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible; //*/

        }
        
    }
}
