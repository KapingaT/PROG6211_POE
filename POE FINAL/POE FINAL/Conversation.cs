using POE_FINAL;
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
                   "🛡️ **I can help you with:**\n" +
                   "📋 Manage cybersecurity tasks\n" +
                   "🎯 Take a cybersecurity quiz\n" +
                   "🔒 Get security tips and advice\n" +
                   "📊 View activity log\n\n" +
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
            string voiceResponse = "🎤 Voice input feature coming soon! I'll be able to listen to your questions in a future update.";
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
                string clearMessage = $"🧹 Chat cleared! Welcome back {userName}! Type 'help' to see what I can do for you.";
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

            // ENHANCED NLP
            string response = GetEnhancedNLPResponse(input);
            if (!response.Contains("I'm not sure about that"))
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

        private string GetEnhancedNLPResponse(string input)
        {
            // Password topics
            if (input.Contains("password") || input.Contains("passphrase"))
            {
                string[] responses = {
                    "🔑 **Passwords:** Use a passphrase of 4+ random words (e.g., 'PurpleTigerJumpsHigh') instead of a single complex word.\n\nNever reuse passwords across different accounts - each account needs its own unique password.\n\nEnable Two-Factor Authentication (2FA) whenever possible for an extra layer of security.",
                    "🔑 **Password Manager:** Use a password manager like Bitwarden or LastPass to generate and store strong passwords securely.\n\nChange your passwords immediately if you suspect any account has been compromised.\n\nA good password manager creates super-strong passwords and fills them in automatically.",
                    "🔑 **Password Tips:** Create passwords that are long and easy to remember.\n\nFor example: 'BlueTacoTuesday$' is better than 'Btt123'.\n\nRemember: Never write passwords on sticky notes or share them with anyone!"
                };
                return responses[random.Next(responses.Length)];
            }

            // Phishing topics
            if (input.Contains("phish") || input.Contains("scam") || input.Contains("fake email"))
            {
                string[] responses = {
                    "🎣 **Phishing:** Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organizations.\n\nHover over links before clicking to see the actual URL. Don't trust shortened links from unknown sources.\n\nWhen in doubt, contact the company directly using their official website.",
                    "🎣 **Scam Detection:** Look for spelling and grammar errors - legitimate companies proofread their emails carefully.\n\nCheck the sender's email address carefully. 'amaz0n@gmail.com' is NOT the real Amazon!\n\nRemember: If something seems too good to be true (like 'You won $1000!'), it's probably a scam.",
                    "🎣 **Email Safety:** Never enter your password on a site you reached through an email link. Type the URL manually instead.\n\nIf an email says 'urgent' or asks for your password, it's probably a scam. Real companies never ask for passwords by email!"
                };
                return responses[random.Next(responses.Length)];
            }

            // Malware topics
            if (input.Contains("malware") || input.Contains("virus") || input.Contains("trojan"))
            {
                string[] responses = {
                    "🦠 **Malware:** Malware (short for MALicious softWARE) is a bad program that hurts your computer. Viruses are one type of malware.\n\nMalware is like a cold for your computer. It can steal your info, show you ads, or lock your files.\n\nInstall antivirus software (Windows has free one called Defender). It catches viruses before they hurt your computer.",
                    "🦠 **Virus Protection:** A computer virus spreads from one computer to another, just like a sick person spreads a cold virus.\n\nDon't download programs from strange websites. Stick to official app stores and trusted sites!\n\nNever open email attachments from people you don't know - that's how viruses spread!"
                };
                return responses[random.Next(responses.Length)];
            }

            // Ransomware
            if (input.Contains("ransomware"))
            {
                string[] responses = {
                    "🔒 **Ransomware:** Ransomware locks your files and demands payment. Best defense: Regular backups and never click suspicious links!\n\nBack up your important photos and documents! Save copies to a USB drive or cloud storage.\n\nIf you have backups, ransomware can't hurt you. You can just restore your files!",
                    "🔒 **Ransomware Warning:** Never pay the ransom! Paying encourages criminals and doesn't guarantee you'll get your files back.\n\nKeep your computer updated and don't click suspicious links. That's the best way to avoid ransomware!"
                };
                return responses[random.Next(responses.Length)];
            }

            // Firewall
            if (input.Contains("firewall"))
            {
                string[] responses = {
                    "🛡️ **Firewall:** A firewall is like a security guard for your internet connection. It decides what data can come in and go out of your computer.\n\nWindows already has a free firewall built-in. Just make sure it's turned on!\n\nA firewall blocks bad guys from accessing your computer through the internet.",
                    "🛡️ **Firewall Protection:** Think of a firewall as the wall around a castle. It lets friendly people in but keeps enemies out.\n\nCheck your firewall settings by searching 'Windows Security' on your computer.\n\nYou need BOTH a firewall AND antivirus software. They work together to protect you!"
                };
                return responses[random.Next(responses.Length)];
            }

            // 2FA
            if (input.Contains("2fa") || input.Contains("two factor") || input.Contains("multi-factor"))
            {
                string[] responses = {
                    "📱 **Two-Factor Authentication:** 2FA adds a second verification step. Enable it on email, banking, and social media!\n\nEven if someone steals your password, they still can't get in without the second code!\n\nUse an authenticator app instead of text messages. It's more secure and works even without cell service!",
                    "📱 **2FA Options:** Authenticator apps are more secure than SMS codes. Hardware keys are even better!\n\nHardware keys are small devices (like YubiKey) that you plug into your computer. They're the strongest form of 2FA.\n\nFor very important accounts (like your email), consider buying a $20-30 security key."
                };
                return responses[random.Next(responses.Length)];
            }

            // Identity Theft
            if (input.Contains("identity theft") || input.Contains("identity"))
            {
                string[] responses = {
                    "🆔 **Identity Theft:** Identity theft is when someone steals your personal information and pretends to be you.\n\nNever share your Social Security number, birth date, or address with strangers online or over the phone.\n\nCheck your bank statements every month. If you see charges you don't recognize, tell your bank immediately!",
                    "🆔 **Identity Protection:** Thieves can use your identity to open credit cards, take loans, or even commit crimes in your name.\n\nFreeze your credit with the three credit bureaus. It's free and stops criminals from opening accounts in your name.\n\nYou can check your credit report for free once a year at AnnualCreditReport.com."
                };
                return responses[random.Next(responses.Length)];
            }

            // Social Engineering
            if (input.Contains("social engineering") || input.Contains("social"))
            {
                string[] responses = {
                    "🧠 **Social Engineering:** Social engineering is when criminals trick you into giving them information instead of hacking your computer.\n\nScammers might call pretending to be tech support or send emails pretending to be your boss. Always verify before sharing info!\n\nIf someone calls asking for your password, credit card, or personal info - hang up! Call the company back using their official number.",
                    "🧠 **Stay Safe:** Criminals use psychology to manipulate you. They create fear ('Your account will be closed!') or excitement ('You won a prize!') to make you act without thinking.\n\nStop and think before clicking or sharing. Ask yourself: 'Is this normal? Could this be a trick?'\n\nReal emergencies don't happen by email. If something feels wrong, it probably is!"
                };
                return responses[random.Next(responses.Length)];
            }

            // Public WiFi
            if (input.Contains("public wifi") || input.Contains("wifi") || input.Contains("hotspot"))
            {
                string[] responses = {
                    "📶 **Public WiFi:** Public WiFi is risky! Use a VPN, avoid banking, and turn off file sharing.\n\nDon't do banking or shopping on public WiFi. Save those for your home internet or phone's data plan.\n\nOn public WiFi, assume people can see your activity. Don't type passwords or credit card numbers!",
                    "📶 **WiFi Safety:** A VPN (Virtual Private Network) creates a secret tunnel for your internet traffic, even on public WiFi.\n\nUse a paid VPN service if you often use public WiFi. It keeps your data private.\n\nFree VPNs might sell your data - that's worse than not using one! Stick with paid, trusted services."
                };
                return responses[random.Next(responses.Length)];
            }

            // Encryption
            if (input.Contains("encryption"))
            {
                return "🔐 **Encryption:** Encryption scrambles data so only authorized parties can read it.\n\nLook for 'https://' in your browser address bar - it means your connection is encrypted.\n\nUse encrypted messaging apps like Signal or WhatsApp for private conversations.";
            }

            // Default responses
            string[] defaultResponses = {
                $"I'm not sure about that, {userName}. Could you ask me about a cybersecurity topic?\n\nTry:\n• 'What is phishing?'\n• 'How do I create strong passwords?'\n• 'What is malware?'\n• 'Tell me about 2FA'\n\nType 'help' to see everything I can teach you!",

                $"Great question! You can ask me things like:\n\n• 'What is ransomware?'\n• 'How does a firewall work?'\n• 'What is identity theft?'\n• 'Give me security tips'\n\nWhat would you like to learn?",

                $"I want to help, but I need you to ask about cybersecurity topics. Try asking:\n\n• 'What is social engineering?'\n• 'Is public WiFi safe?'\n• 'How do I spot a scam email?'\n\nType 'help' for the full list of topics I know about!"
            };

            return defaultResponses[random.Next(defaultResponses.Length)];
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

    public class ChatMessage
    {
        public string Sender { get; set; }
        public string Text { get; set; }
        public HorizontalAlignment Alignment { get; set; }
        public Style BubbleStyle { get; set; }
        public string Timestamp { get; set; }
    }
}