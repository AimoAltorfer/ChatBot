using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;

namespace ChatBot
{
    public partial class MainWindow : Window
    {
        Dictionary<string, string> responses;
        ObservableCollection<string> chatMessages;
        private string logFilePath = "chatlog.txt";

        public MainWindow()
        {
            InitializeComponent();
            LoadResponses();
            chatMessages = new ObservableCollection<string>();
            ChatList.ItemsSource = chatMessages;
            EnsureLogFileExists();
            LoadChatHistory();
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

        private void EnsureLogFileExists()
        {
            if (!File.Exists(logFilePath))
            {
                using (File.Create(logFilePath)) { }
            }
        }

        private void LoadChatHistory()
        {
            foreach (var line in File.ReadLines(logFilePath))
            {
                chatMessages.Add(line);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var input = InputBox.Text.ToLower();
            var userMessage = "Du: " + input;
            chatMessages.Add(userMessage);
            LogMessage(userMessage);

            var response = GetResponse(input);
            var botMessage = "Bot: " + response;
            chatMessages.Add(botMessage);
            LogMessage(botMessage);

            InputBox.Clear();
        }

        private void ClearHistory_Click(object sender, RoutedEventArgs e)
        {
            chatMessages.Clear();
            File.WriteAllText(logFilePath, string.Empty);
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

        private void LogMessage(string message)
        {
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now}: {message}");
            }
        }
    }
}
