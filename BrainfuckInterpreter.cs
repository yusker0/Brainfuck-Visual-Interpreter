using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Threading;
using Microsoft.Win32;
using System.Windows.Media;
using Brainfuck_Visual_Interpreter;

namespace Brainfuck_Visual_Interpreter
{
    public class BFCode
    {
        private Console console;
        private MainWindow mainWindow;

        public string code;
        public string filepath;
        public int MAX;
        public int ptr;
        public int chPtr;
        public double speed;
        public readonly double minSpeed = -1;
        public readonly double maxSpeed = 6;
        public bool isPlaying;

        int close;
        int prevCh;
        int prevInput;
        public int[] arr;

        char[] chars = {'+', '-', '>', '<', '.', ',', '[', ']'};
        
        public BFCode(string filePath, MainWindow main)
        {
            mainWindow = main;
            if (filePath == "default")
            {
                code = "++++++[->++++++++++++<]>.< +++++++[->++++<] > +.++++++ + ..+++.> ++++++++[-<< ++++>>] << +.-.>>> ++++++++++++[-< ++++++>] < +.< +++++.<.>-----------.++++++++++.<.>> -------.< -.>> ++++[-<< ---->>] << -.++++++++.++++ +.--------.>> +++[-<< +++++>>] <<.>> ++++++[-<< --->>] <<.++++++++. <. >>> +++++[-< ++++>] <.< --.++++++++++.++.>> ++++[-<< ----->>] <<.++++++++++ +.<.>> -------------.< ++.++++++.-------------- -.++++++++++++ +.--.++.------------ -.++++++++++++++ +.-------------- -.++++++++++++ +.< +.";
            }
            else
                code = File.ReadAllText(filePath);
            
            filepath = filePath;

            MAX = findMax(code);
            ptr = 0;
            chPtr = 0;
            close = 0;
            isPlaying = false;
            speed = 1;
            arr = new int[MAX];
        }
        public bool nextStep()
        {
            prevCh = chPtr;
            switch (code[chPtr])
            {
                case '+':
                    if (arr[ptr] < 255)
                        arr[ptr]++;
                    else
                        arr[ptr] = 0;
                    mainWindow.panels[(int)Math.Floor((decimal)ptr/15), ptr % 15].Text = arr[ptr].ToString();
                    break;
                case '-':
                    if (arr[ptr] > 0)
                        arr[ptr]--;
                    else
                        arr[ptr] = 255;
                    mainWindow.panels[(int)Math.Floor((decimal)ptr / 15), ptr % 15].Text = arr[ptr].ToString();

                    break;
                case '>':
                    if (ptr < MAX - 1)
                        ptr++;
                    else
                        return false;
                    mainWindow.pointer.SetValue(Grid.RowProperty, (int)Math.Floor((decimal)ptr / 15) * 2 );
                    mainWindow.pointer.SetValue(Grid.ColumnProperty, ptr % 15 + 1);
                    break;
                case '<':
                    if (ptr > 0)
                        ptr--;
                    else
                        return false;
                    mainWindow.pointer.SetValue(Grid.RowProperty, (int)Math.Floor((decimal)ptr / 15) * 2);
                    mainWindow.pointer.SetValue(Grid.ColumnProperty, ptr % 15 + 1);
                    break;
                case '.':
                    console = Application.Current.Windows[1] as Console;
                    console.consoleText.Text += Encoding.ASCII.GetString(new byte[] { (byte)arr[ptr] });
                    break;
                case ',':
                    console = Application.Current.Windows[1] as Console;
                    InputDialog inputDialog = new InputDialog();
                    if (inputDialog.ShowDialog() == true)
                    {
                        console.consoleText.Text += inputDialog.Answer;
                        prevInput = arr[ptr];
                        arr[ptr] = (int)inputDialog.Answer[0];
                        mainWindow.panels[(int)Math.Floor((decimal)ptr / 15), ptr % 15].Text = arr[ptr].ToString();
                    }
                    break;
                case '[':
                    if (arr[ptr] == 0)
                        chPtr += findClose(code.Substring(chPtr+1));
                    break;
                case ']':
                    close = findOpen(code.Substring(0, chPtr));
                    
                    if (close == -1)
                        return false;
                    if (arr[ptr] != 0)
                        chPtr = close;//*/
                    break;
            }
            if (chPtr + 1 < code.Length)
            {
                while (!chars.Contains(code[chPtr+1]))
                {
                    if (chPtr+2 == code.Length)
                    {
                        chPtr++;
                        return true;
                    }
                    else
                        chPtr++;
                }
            }
            chPtr++;
            return true;
        }
        

        public bool cancelStep()
        {
            chPtr = prevCh;
            switch (code[chPtr])
            {
                case '+':
                    if (arr[ptr] > 0)
                        arr[ptr]--;
                    else
                        arr[ptr] = 255;
                    mainWindow.panels[(int)Math.Floor((decimal)ptr / 15), ptr % 15].Text = arr[ptr].ToString();
                    break;
                case '-':
                    if (arr[ptr] < 255)
                        arr[ptr]++;
                    else
                        arr[ptr] = 0;
                    mainWindow.panels[(int)Math.Floor((decimal)ptr / 15), ptr % 15].Text = arr[ptr].ToString();
                    break;
                case '>':
                    ptr--;
                    mainWindow.pointer.SetValue(Grid.RowProperty, (int)Math.Floor((decimal)ptr / 15) * 2);
                    mainWindow.pointer.SetValue(Grid.ColumnProperty, ptr % 15 + 1);
                    break;
                case '<':
                    ptr++;
                    mainWindow.pointer.SetValue(Grid.RowProperty, (int)Math.Floor((decimal)ptr / 15) * 2);
                    mainWindow.pointer.SetValue(Grid.ColumnProperty, ptr % 15 + 1);
                    break;
                case '.':
                    console = Application.Current.Windows[1] as Console;
                    console.consoleText.Text = console.consoleText.Text.Substring(0, console.consoleText.Text.Length-1);
                    break;
                case ',':
                    console = Application.Current.Windows[1] as Console;
                    console.consoleText.Text = console.consoleText.Text.Substring(0, console.consoleText.Text.Length-1);
                    mainWindow.panels[(int)Math.Floor((decimal)ptr / 15), ptr % 15].Text = prevInput.ToString();
                    break;
            }
            return true;
        }

        public async void playAsync()
        {
            while (isPlaying && chPtr < code.Length)
            {
                nextStep();
                if (chPtr < code.Length)
                {
                    mainWindow.codeBlock.Inlines.Clear();
                    mainWindow.codeBlock.Inlines.Add(new Run(code.Substring(0, chPtr)));
                    mainWindow.codeBlock.Inlines.Add(new Run(code.Substring(chPtr, 1)) { Foreground = Brushes.Black });
                    mainWindow.codeBlock.Inlines.Add(new Run(code.Substring(chPtr + 1)));//*/
                }
                await Task.Delay((int)Math.Round(1000/Math.Pow(2, speed)));
            }
            isPlaying = false;
            mainWindow.playButton.Content = "Play";
            mainWindow.playButton.IsEnabled = chPtr < code.Length ? true : false;
            mainWindow.nextStepButton.IsEnabled = false;
            mainWindow.cancelStepButton.IsEnabled = true;


        }
        /*
        public bool prevStep()
        {
            chPtr--;
            while (!chars.Contains(code[chPtr]))
            {
                if (chPtr == 0)
                    return true;
                else
                    chPtr--;
            }
            switch (code[chPtr])
            {
                case '+':
                    if (arr[ptr] > 0)
                        arr[ptr]--;
                    else
                        arr[ptr] = 255;
                    mainWindow.panels[(int)Math.Floor((decimal)ptr / 15), ptr % 15].Text = arr[ptr].ToString();
                    break;
                case '-':
                    if (arr[ptr] < 255)
                        arr[ptr]++;
                    else
                        arr[ptr] = 0;
                    mainWindow.panels[(int)Math.Floor((decimal)ptr / 15), ptr % 15].Text = arr[ptr].ToString();
                    break;
                case '>':
                    ptr--;
                    mainWindow.pointer.SetValue(Grid.RowProperty, (int)Math.Floor((decimal)ptr / 15) * 2);
                    mainWindow.pointer.SetValue(Grid.ColumnProperty, ptr % 15 + 1);
                    break;
                case '<':
                    ptr++;
                    mainWindow.pointer.SetValue(Grid.RowProperty, (int)Math.Floor((decimal)ptr / 15) * 2);
                    mainWindow.pointer.SetValue(Grid.ColumnProperty, ptr % 15 + 1);
                    break;
                case '.':
                    console = Application.Current.Windows[1] as Console;
                    console.consoleText.Text += arr[ptr].ToString();
                    break;
                case ',':
                    break;
                case '[':
                    if (close == chPtr && arr[ptr] == 0)
                        chPtr += findClose(code.Substring(chPtr + 1)) + 1;
                    break;
                case ']':
                    if (arr[ptr] != 0)
                        chPtr = findOpen(code.Substring(0, chPtr));
                    break;
            }
            return true;
        }
        //*/

        private int findClose(string code)
        {
            int opens = 1;
            for (int i = 0; i < code.Length; i++)
            {
                if (code[i] == ']' && opens == 1)
                    return i;
                else if (code[i] == ']')
                    opens--;
                else if (code[i] == '[')
                    opens++;
            }
            return -1;
        }

        private int findOpen(string code)
        {
            int closes= 1;
            for(int i = code.Length - 1; i >= 0; i--)
            {
                if (code[i]=='[' && closes == 1)
                    return i;
                else if (code[i]=='[')
                    closes--;
                else if (code[i]==']')
                    closes++;
            }
            return -1;
        }
        private int findMax(string code)
        {
            int rights = 1;
            int max = 0;
            foreach(char ch in code)
            {
                switch (ch)
                {
                    case '>':
                        rights++;
                        break;
                    case '<':
                        rights--;
                        break;
                }
                if (rights > max)
                    max = rights;
            }
            return max;
        }
    }
}
