using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;

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
                    responses[parts[0].Trim().ToLower()] = parts[1].Trim();
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var input = InputBox.Text.ToLower();
            ChatBox.AppendText("Du: " + input + Environment.NewLine);

            var response = GetResponse(input);
            ChatBox.AppendText("Bot: " + response + Environment.NewLine);

            InputBox.Clear();
        }

        private string GetResponse(string input)
        {
            foreach (var key in responses.Keys)
            {
                if (LevenshteinDistance(input, key) <= 2)
                {
                    return responses[key];
                }
            }
            return "Ich verstehe das nicht.";
        }

        private int LevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0) return m;
            if (m == 0) return n;

            for (int i = 0; i <= n; d[i, 0] = i++) { }
            for (int j = 0; j <= m; d[0, j] = j++) { }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = t[j - 1] == s[i - 1] ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }

            return d[n, m];
        }
    }
}
