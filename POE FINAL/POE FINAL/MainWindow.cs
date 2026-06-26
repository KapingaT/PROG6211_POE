using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace POE_FINAL
{
    public partial class MainWindow : Window
    {
        private string userName = "";
        private ActivityLog activityLog;
        private bool quizActive = false;
        private Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();
            activityLog = new ActivityLog();
            activityLog.AddLog("Application started");

            // Play greeting audio from Part 2
            Audio.PlayGreeting();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            Environment.Exit(0);
        }

        private void SubmitNameButton_Click(object sender, RoutedEventArgs e)
        {
            userName = NameTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(userName))
            {
                NameInputPanel.Visibility = Visibility.Collapsed;
                activityLog.AddLog($"User '{userName}' started session");

                string welcomeMessage = GetWelcomeMessage();
                AddBotMessage(welcomeMessage);
                UpdateResponseDisplay(welcomeMessage);
                ScrollToBottom();
            }
            else
            {
                MessageBox.Show("Please enter your name to continue.", "Name Required",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private string GetWelcomeMessage()
        {
            return $"Hello {userName}! I'm CyberPal, your cybersecurity assistant.\n\n" +
                   " **I can help you with:**\n" +
                   " Manage cybersecurity tasks\n" +
                   " Take a cybersecurity quiz\n" +
                   " Get security tips and advice\n" +
                   " View activity log\n\n" +
                   "Type 'help' to see all commands, or just ask me anything about cybersecurity!";
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !(Keyboard.Modifiers == ModifierKeys.Shift))
            {
                e.Handled = true;
                SendMessage();
            }
        }

        private void VoiceButton_Click(object sender, RoutedEventArgs e)
        {
            string voiceResponse = " Voice input feature coming soon! I'll be able to listen to your questions in a future update.";
            AddBotMessage(voiceResponse);
            UpdateResponseDisplay(voiceResponse);
            ScrollToBottom();
        }

        private void QuickTopic_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Tag != null)
            {
                string message = button.Tag.ToString();
                MessageTextBox.Text = message;
                SendMessage();
            }
        }

        private void ClearChat_Click(object sender, RoutedEventArgs e)
        {
            ChatListBox.Items.Clear();
            if (!string.IsNullOrEmpty(userName))
            {
                string clearMessage = $" Chat cleared! Welcome back {userName}! Type 'help' to see what I can do for you.";
                AddBotMessage(clearMessage);
                UpdateResponseDisplay(clearMessage);
                ScrollToBottom();
                activityLog.AddLog("Chat cleared");
            }
            else
            {
                UpdateResponseDisplay("Chat history has been cleared.");
            }
        }

        private void SendMessage()
        {
            string message = MessageTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(message))
            {
                AddUserMessage(message);
                MessageTextBox.Clear();

                activityLog.AddLog($"User: {message}");
                ProcessBotResponse(message);
                ScrollToBottom();
            }
        }

        private void ProcessBotResponse(string userMessage)
        {
            string response = GetIntelligentResponse(userMessage);

            if (response.Length > 50)
                activityLog.AddLog($"Bot: {response.Substring(0, 50)}...");
            else
                activityLog.AddLog($"Bot: {response}");

            AddBotMessage(response);
            UpdateResponseDisplay(response);
            ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            if (ChatListBox.Items.Count > 0)
            {
                ChatListBox.ScrollIntoView(ChatListBox.Items[ChatListBox.Items.Count - 1]);
            }
        }

        private void UpdateResponseDisplay(string response)
        {
            ResponseDisplayBox.Text = response;
            ResponseTimestamp.Text = DateTime.Now.ToString("h:mm:ss tt");

            try
            {
                ResponseDisplayBox.Opacity = 0;
                var animation = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
                ResponseDisplayBox.BeginAnimation(UIElement.OpacityProperty, animation);
            }
            catch
            {
                ResponseDisplayBox.Opacity = 1;
            }
        }

        // ============ MAIN INTELLIGENT RESPONSE METHOD ============
        private string GetIntelligentResponse(string userMessage)
        {
            string input = userMessage.ToLower().Trim();

            // TASK 4: Activity Log Commands
            if (input.Contains("show activity log") || input.Contains("what have you done") ||
                input.Contains("activity log") || input.Contains("show log") || input.Contains("show full log"))
            {
                if (input.Contains("full"))
                    return activityLog.GetFullLog();
                else
                    return activityLog.GetRecentLogs();
            }

            // TASK 1: Task Management
            if (input.Contains("add task") || input.Contains("new task") ||
                input.Contains("create task") || input.Contains("add a task"))
            {
                return HandleAddTask(userMessage);
            }

            if (input.Contains("view tasks") || input.Contains("show tasks") ||
                input.Contains("list tasks") || input.Contains("my tasks") || input.Contains("tasks"))
            {
                return HandleViewTasks();
            }

            if (input.Contains("complete") || input.Contains("done") ||
                input.Contains("mark as completed") || input.Contains("finish task"))
            {
                return HandleCompleteTask(userMessage);
            }

            if (input.Contains("delete task") || input.Contains("remove task"))
            {
                return HandleDeleteTask(userMessage);
            }

            // TASK 2: Quiz
            if (input.Contains("start quiz") || input.Contains("play quiz") ||
                input.Contains("quiz") || input.Contains("test my knowledge"))
            {
                return HandleStartQuiz();
            }

            if (quizActive && input.Length == 1 && input[0] >= 'a' && input[0] <= 'd')
            {
                return HandleQuizAnswer(input);
            }

            // TASK 3: Enhanced NLP with Reminders
            if (input.Contains("remind") || input.Contains("reminder") || input.Contains("set a reminder"))
            {
                return HandleReminder(userMessage);
            }

            // EXISTING FEATURES
            if (input.Contains("bye") || input.Contains("goodbye") || input.Contains("exit") || input.Contains("quit"))
            {
                activityLog.AddLog($"User '{userName}' ended session");
                return $"Goodbye {userName}! Stay safe online! Remember, CyberPal is always here if you need help. 👋";
            }

            if (input.Contains("help") || input.Contains("what can you do") || input == "help me")
            {
                return GetHelpMessage();
            }

            if (input.Contains("about") || input.Contains("yourself") || input.Contains("who are you"))
            {
                return GetAboutMessage();
            }

            if (input.Contains("tip") || input.Contains("advice") || input.Contains("suggestion") || input == "tips")
            {
                return GetRandomTip();
            }

            // Greetings
            if (input == "hello" || input == "hi" || input == "hey" || input == "greetings")
            {
                string[] greetings = {
                    $"Hello {userName}! Nice to see you! What would you like to learn about cybersecurity today?",
                    $"Hi {userName}! Ready to learn how to stay safe online? Just ask me anything!",
                    $"Hey {userName}! I'm here to help you with cybersecurity. What's on your mind?"
                };
                return greetings[random.Next(greetings.Length)];
            }

            // FALLBACK: Use Part 2's Conversation class for general chat
            string response = Conversation.GetResponse(userMessage, userName);
            if (!response.Contains("I didn't quite understand") && !response.Contains("I'm not sure"))
            {
                activityLog.AddLog($"NLP detected: {input.Substring(0, Math.Min(30, input.Length))}...");
            }
            return response;
        }

        // ============ TASK 1: TASK MANAGEMENT METHODS ============
        private string HandleAddTask(string input)
        {
            try
            {
                string taskDescription = ExtractTaskDescription(input);

                if (string.IsNullOrEmpty(taskDescription))
                {
                    return "Please specify what task you'd like to add. Example: 'Add task: Enable two-factor authentication'";
                }

                DateTime reminderDate = DateTime.MinValue;
                string reminderText = "No reminder set";

                if (taskDescription.ToLower().Contains("remind") || taskDescription.ToLower().Contains("in "))
                {
                    if (taskDescription.ToLower().Contains("tomorrow"))
                        reminderDate = DateTime.Now.AddDays(1);
                    else if (taskDescription.ToLower().Contains("week") || taskDescription.ToLower().Contains("7 days"))
                        reminderDate = DateTime.Now.AddDays(7);
                    else if (taskDescription.ToLower().Contains("month") || taskDescription.ToLower().Contains("30 days"))
                        reminderDate = DateTime.Now.AddDays(30);
                    else
                        reminderDate = DateTime.Now.AddDays(1);

                    reminderText = $"Reminder set for {reminderDate.ToShortDateString()}";
                }

                Task newTask = new Task
                {
                    Title = taskDescription.Length > 50 ? taskDescription.Substring(0, 47) + "..." : taskDescription,
                    Description = taskDescription,
                    ReminderDate = reminderDate != DateTime.MinValue ? reminderDate : (DateTime?)null
                };

                TaskManager.AddTask(newTask);
                activityLog.AddLog($"✅ Task added: '{newTask.Title}'");

                string response = $"✅ Task added successfully!\n📝 '{newTask.Title}'";
                if (reminderDate != DateTime.MinValue)
                {
                    response += $"\n📅 {reminderText}";
                }
                else
                {
                    response += "\n💡 Would you like to set a reminder? Just say 'Set reminder for [task name]'";
                }
                return response;
            }
            catch (Exception ex)
            {
                return $"❌ Error adding task: {ex.Message}\nMake sure MySQL is running and the database is set up.";
            }
        }

        private string ExtractTaskDescription(string input)
        {
            string[] prefixes = { "add task", "new task", "create task", "add a task", "add task:" };
            string result = input;

            foreach (string prefix in prefixes)
            {
                if (result.ToLower().Contains(prefix))
                {
                    int index = result.ToLower().IndexOf(prefix) + prefix.Length;
                    result = result.Substring(index).Trim();
                    break;
                }
            }

            result = result.TrimStart(':', ' ', '-');
            return result;
        }

        private string HandleViewTasks()
        {
            try
            {
                var tasks = TaskManager.GetAllTasks();
                if (tasks.Count == 0)
                {
                    return "📭 You don't have any tasks yet. Would you like to add one?";
                }

                string response = "📋 **Your Tasks:**\n\n";
                int count = 1;
                foreach (var task in tasks)
                {
                    string status = task.IsCompleted ? "✅" : "⏳";
                    string reminder = task.ReminderDate != null ?
                        $" 📅 {task.ReminderDate.Value.ToShortDateString()}" : "";
                    response += $"{count}. {status} {task.Title}{reminder}\n";
                    count++;
                }
                return response;
            }
            catch (Exception ex)
            {
                return $"❌ Error loading tasks: {ex.Message}";
            }
        }

        private string HandleCompleteTask(string input)
        {
            try
            {
                var tasks = TaskManager.GetAllTasks();
                if (tasks.Count == 0)
                {
                    return "📭 You don't have any tasks to complete.";
                }

                string[] words = input.Split(' ');
                foreach (string word in words)
                {
                    if (int.TryParse(word, out int taskNumber) && taskNumber > 0 && taskNumber <= tasks.Count)
                    {
                        var task = tasks[taskNumber - 1];
                        if (!task.IsCompleted)
                        {
                            TaskManager.CompleteTask(task.Id);
                            activityLog.AddLog($"✅ Task completed: '{task.Title}'");
                            return $"✅ Task '{task.Title}' marked as completed! Great job! 🎉";
                        }
                        else
                        {
                            return $"ℹ️ Task '{task.Title}' is already completed!";
                        }
                    }
                }

                foreach (var task in tasks)
                {
                    if (input.ToLower().Contains(task.Title.ToLower()) ||
                        input.ToLower().Contains(task.Description.ToLower()))
                    {
                        if (!task.IsCompleted)
                        {
                            TaskManager.CompleteTask(task.Id);
                            activityLog.AddLog($"✅ Task completed: '{task.Title}'");
                            return $"✅ Task '{task.Title}' marked as completed! Great job! 🎉";
                        }
                        else
                        {
                            return $"ℹ️ Task '{task.Title}' is already completed!";
                        }
                    }
                }

                return "❌ Couldn't find that task. Please specify the task number (e.g., 'Complete task 1') or the task title.";
            }
            catch (Exception ex)
            {
                return $"❌ Error completing task: {ex.Message}";
            }
        }

        private string HandleDeleteTask(string input)
        {
            try
            {
                var tasks = TaskManager.GetAllTasks();
                if (tasks.Count == 0)
                {
                    return "📭 You don't have any tasks to delete.";
                }

                string[] words = input.Split(' ');
                foreach (string word in words)
                {
                    if (int.TryParse(word, out int taskNumber) && taskNumber > 0 && taskNumber <= tasks.Count)
                    {
                        var task = tasks[taskNumber - 1];
                        TaskManager.DeleteTask(task.Id);
                        activityLog.AddLog($"🗑️ Task deleted: '{task.Title}'");
                        return $"🗑️ Task '{task.Title}' has been deleted.";
                    }
                }

                foreach (var task in tasks)
                {
                    if (input.ToLower().Contains(task.Title.ToLower()) ||
                        input.ToLower().Contains(task.Description.ToLower()))
                    {
                        TaskManager.DeleteTask(task.Id);
                        activityLog.AddLog($"🗑️ Task deleted: '{task.Title}'");
                        return $"🗑️ Task '{task.Title}' has been deleted.";
                    }
                }

                return "❌ Couldn't find that task. Please specify the task number (e.g., 'Delete task 1') or the task title.";
            }
            catch (Exception ex)
            {
                return $"❌ Error deleting task: {ex.Message}";
            }
        }

        // ============ TASK 2: QUIZ METHODS ============
        private string HandleStartQuiz()
        {
            var quiz = QuizManager.Instance;
            if (quiz.IsQuizActive)
            {
                return "⚠️ A quiz is already in progress! Answer the current question (A, B, C, or D).";
            }

            quizActive = true;
            activityLog.AddLog("🎯 Quiz started");

            string response = "🎯 **Starting Cybersecurity Quiz!**\n\n";
            response += "I'll ask you 10 questions about cybersecurity. Type A, B, C, or D to answer.\n\n";

            var question = quiz.StartQuiz();
            response += $"**Q{quiz.CurrentQuestionIndex + 1}: {question.QuestionText}**\n\n";

            for (int i = 0; i < question.Options.Count; i++)
            {
                response += $"{(char)('A' + i)}) {question.Options[i]}\n";
            }

            return response;
        }

        private string HandleQuizAnswer(string input)
        {
            var quiz = QuizManager.Instance;
            if (!quiz.IsQuizActive)
            {
                quizActive = false;
                return "⚠️ No active quiz. Type 'start quiz' to begin!";
            }

            char selectedOption = char.ToUpper(input.Trim()[0]);
            int optionIndex = selectedOption - 'A';

            if (optionIndex < 0 || optionIndex >= 4)
            {
                return "❌ Please select A, B, C, or D.";
            }

            bool isCorrect = quiz.AnswerQuestion(optionIndex);
            var currentQuestion = quiz.GetCurrentQuestion();

            string response = isCorrect ? "✅ Correct! " : "❌ Incorrect. ";
            response += currentQuestion.Explanation + "\n\n";

            var nextQuestion = quiz.GetNextQuestion();
            if (nextQuestion != null)
            {
                response += $"**Q{quiz.CurrentQuestionIndex + 1}/{quiz.TotalQuestions}: {nextQuestion.QuestionText}**\n\n";
                for (int i = 0; i < nextQuestion.Options.Count; i++)
                {
                    response += $"{(char)('A' + i)}) {nextQuestion.Options[i]}\n";
                }
                return response;
            }
            else
            {
                quizActive = false;
                string feedback = quiz.GetScoreFeedback();
                activityLog.AddLog($"🎯 Quiz completed. Score: {quiz.GetScore()}/{quiz.TotalQuestions}");
                return $"🎉 **Quiz Complete!**\n\nYour score: {quiz.GetScore()}/{quiz.TotalQuestions}\n{feedback}";
            }
        }

        // ============ TASK 3: ENHANCED NLP ============
        private string HandleReminder(string input)
        {
            string reminderText = input;
            string[] prefixes = { "remind me to", "set reminder", "reminder for", "remind", "set a reminder" };

            foreach (string prefix in prefixes)
            {
                if (reminderText.ToLower().Contains(prefix))
                {
                    int index = reminderText.ToLower().IndexOf(prefix) + prefix.Length;
                    reminderText = reminderText.Substring(index).Trim();
                    break;
                }
            }

            if (string.IsNullOrEmpty(reminderText))
            {
                return "What would you like me to remind you about? Example: 'Remind me to review privacy settings'";
            }

            DateTime reminderDate = DateTime.Now.AddDays(1);
            string dateDescription = "tomorrow";

            if (input.ToLower().Contains("tomorrow"))
            {
                reminderDate = DateTime.Now.AddDays(1);
                dateDescription = "tomorrow";
            }
            else if (input.ToLower().Contains("week") || input.ToLower().Contains("7 days"))
            {
                reminderDate = DateTime.Now.AddDays(7);
                dateDescription = "in 7 days";
            }
            else if (input.ToLower().Contains("month") || input.ToLower().Contains("30 days"))
            {
                reminderDate = DateTime.Now.AddDays(30);
                dateDescription = "in 30 days";
            }

            Task reminderTask = new Task
            {
                Title = $"🔔 Reminder: {reminderText}",
                Description = reminderText,
                ReminderDate = reminderDate
            };

            TaskManager.AddTask(reminderTask);
            activityLog.AddLog($"🔔 Reminder set: '{reminderText}' for {reminderDate.ToShortDateString()}");

            return $"🔔 **Reminder Set!**\n📝 '{reminderText}'\n📅 Date: {reminderDate.ToShortDateString()} ({dateDescription})";
        }

        // ============ HELPER METHODS ============
        private string GetHelpMessage()
        {
            return $"=== 🛡️ **CYBERPAL HELP MENU** ===\n\n" +
                   $"Hi {userName}! Here's what I can help you with:\n\n" +
                   "📋 **TASKS**\n" +
                   "   • Add task: 'Add task: Enable 2FA'\n" +
                   "   • View tasks: 'Show my tasks'\n" +
                   "   • Complete task: 'Complete task 1'\n" +
                   "   • Delete task: 'Delete task 2'\n\n" +

                   "🎯 **QUIZ**\n" +
                   "   • Start quiz: 'Start quiz'\n" +
                   "   • Answer: 'A', 'B', 'C', or 'D'\n\n" +

                   "🔔 **REMINDERS**\n" +
                   "   • Set reminder: 'Remind me to update password tomorrow'\n\n" +

                   "🔒 **CYBERSECURITY TOPICS**\n" +
                   "   • Passwords, Phishing, Malware, Ransomware\n" +
                   "   • Firewalls, 2FA, Identity Theft\n" +
                   "   • Social Engineering, Public WiFi, Encryption\n\n" +

                   "📊 **ACTIVITY LOG**\n" +
                   "   • Show log: 'Show activity log'\n\n" +

                   "Type your question or command! 🚀";
        }

        private string GetAboutMessage()
        {
            return "🤖 **About CyberPal**\n\n" +
                   "I'm your friendly cybersecurity assistant! I explain online safety in simple terms.\n\n" +
                   "**What I can do:**\n" +
                   "• Teach you about cybersecurity topics in easy-to-understand words\n" +
                   "• Help you manage cybersecurity tasks with reminders\n" +
                   "• Test your knowledge with fun quizzes\n" +
                   "• Keep track of our interactions\n\n" +
                   "**Why I exist:**\n" +
                   "To help everyone stay safe online, regardless of technical experience!\n\n" +
                   "Type 'help' to see what I can do for you!";
        }

        private string GetRandomTip()
        {
            string[] tips = {
                "🔒 Tip: Always keep your phone and computer updated. Updates fix security problems!",
                "🔒 Tip: Use a different password for every account. That way, if one gets stolen, the others stay safe!",
                "🔒 Tip: Turn on two-factor authentication (2FA) for your email and bank accounts. It's like having a second lock on your door!",
                "🔒 Tip: If an email looks suspicious or asks for personal info, don't click anything. Delete it instead!",
                "🔒 Tip: Back up your important photos and files to an external drive or cloud storage. Then if something goes wrong, you won't lose them!",
                "🔒 Tip: Never share your passwords with anyone - not even friends or family. Keep them secret!",
                "🔒 Tip: Before clicking a link, hover your mouse over it to see where it really goes. If it looks strange, don't click!"
            };
            return tips[random.Next(tips.Length)];
        }

        // ============ UI HELPER METHODS ============
        private void AddUserMessage(string message)
        {
            ChatListBox.Items.Add(new ChatMessage
            {
                Sender = userName,
                Text = message,
                Alignment = HorizontalAlignment.Right,
                BubbleStyle = (Style)FindResource("UserBubbleStyle"),
                Timestamp = DateTime.Now.ToString("h:mm tt")
            });
        }

        private void AddBotMessage(string message)
        {
            ChatListBox.Items.Add(new ChatMessage
            {
                Sender = "CyberPal",
                Text = message,
                Alignment = HorizontalAlignment.Left,
                BubbleStyle = (Style)FindResource("BotBubbleStyle"),
                Timestamp = DateTime.Now.ToString("h:mm tt")
            });
        }
    }
}

    
