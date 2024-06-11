using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace ChatBot
{
    public partial class MainWindow : Window
    {
        Dictionary<string, string> responses;

        public MainWindow()
        {
            InitializeComponent();
            LoadResponses();
        }

        private void LoadResponses()
        {
            responses = new Dictionary<string, string>();
            foreach (var line in File.ReadLines("responses.txt"))
            {
                var parts = line.Split('=');
                if (parts.Length == 2)
                {
                    responses[parts[0].Trim()] = parts[1].Trim();
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var input = InputBox.Text;
            ChatBox.AppendText("Du: " + input + Environment.NewLine);

            if (responses.ContainsKey(input))
            {
                ChatBox.AppendText("Bot: " + responses[input] + Environment.NewLine);
            }
            else
            {
                ChatBox.AppendText("Bot: Ich verstehe das nicht." + Environment.NewLine);
            }

            InputBox.Clear();
        }
    }
}
