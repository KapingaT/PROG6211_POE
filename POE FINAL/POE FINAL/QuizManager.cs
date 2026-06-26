using System;
using System.Collections.Generic;

namespace POE_FINAL
{
    public class QuizQuestion
    {
        public string QuestionText { get; set; }
        public List<string> Options { get; set; }
        public int CorrectAnswer { get; set; }
        public string Explanation { get; set; }
    }

    public class QuizManager
    {
        private static QuizManager instance;
        private List<QuizQuestion> questions;
        private int currentQuestionIndex;
        private int score;
        private bool isActive;
        private List<int> userAnswers;

        private QuizManager()
        {
            InitializeQuestions();
            ResetQuiz();
        }

        public static QuizManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new QuizManager();
                return instance;
            }
        }

        public bool IsQuizActive => isActive;
        public int CurrentQuestionIndex => currentQuestionIndex;
        public int TotalQuestions => questions.Count;

        private void InitializeQuestions()
        {
            questions = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    QuestionText = "What should you do if you receive an email asking for your password?",
                    Options = new List<string> { "Reply with your password", "Delete the email", "Report the email as phishing", "Forward it to a friend" },
                    CorrectAnswer = 2,
                    Explanation = "✅ Reporting phishing emails helps prevent scams and protects your organization."
                },
                new QuizQuestion
                {
                    QuestionText = "Which of the following is a strong password?",
                    Options = new List<string> { "password123", "P@ssw0rd!", "qwerty", "12345678" },
                    CorrectAnswer = 1,
                    Explanation = "✅ A strong password includes uppercase, lowercase, numbers, and special characters."
                },
                new QuizQuestion
                {
                    QuestionText = "What does 2FA stand for?",
                    Options = new List<string> { "Two Factor Authentication", "Two Form Access", "Total Function Approval", "Time Frame Adjustment" },
                    CorrectAnswer = 0,
                    Explanation = "✅ Two Factor Authentication adds an extra layer of security beyond just a password."
                },
                new QuizQuestion
                {
                    QuestionText = "Is it safe to use public Wi-Fi for online banking?",
                    Options = new List<string> { "Yes, always safe", "Only if you use a VPN", "Yes, if you have a password", "No, never" },
                    CorrectAnswer = 1,
                    Explanation = "✅ Public Wi-Fi is unsafe; always use a VPN for sensitive transactions."
                },
                new QuizQuestion
                {
                    QuestionText = "What is social engineering in cybersecurity?",
                    Options = new List<string> { "Building social networks", "Manipulating people to gain access", "Engineering social media platforms", "Creating social policies" },
                    CorrectAnswer = 1,
                    Explanation = "✅ Social engineering exploits human psychology to gain unauthorized access."
                },
                new QuizQuestion
                {
                    QuestionText = "How often should you update your passwords?",
                    Options = new List<string> { "Never", "Every year", "Every 3-6 months", "Only when forced" },
                    CorrectAnswer = 2,
                    Explanation = "✅ Regular password updates (every 3-6 months) help maintain security."
                },
                new QuizQuestion
                {
                    QuestionText = "What should you do with sensitive documents you no longer need?",
                    Options = new List<string> { "Throw them in the trash", "Shred them", "Recycle them", "Store them" },
                    CorrectAnswer = 1,
                    Explanation = "✅ Shredding sensitive documents prevents unauthorized access to information."
                },
                new QuizQuestion
                {
                    QuestionText = "What is the safest way to use multiple online accounts?",
                    Options = new List<string> { "Use the same password", "Use a password manager", "Write passwords on a sticky note", "Memorize all passwords" },
                    CorrectAnswer = 1,
                    Explanation = "✅ Password managers help you create and store unique, strong passwords for each account."
                },
                new QuizQuestion
                {
                    QuestionText = "What should you do if your device is stolen?",
                    Options = new List<string> { "Nothing", "Report it immediately", "Wait a few days", "Only report if it's expensive" },
                    CorrectAnswer = 1,
                    Explanation = "✅ Immediately report stolen devices to prevent data breaches and unauthorized access."
                },
                new QuizQuestion
                {
                    QuestionText = "What is the best way to protect your online privacy?",
                    Options = new List<string> { "Share everything on social media", "Use privacy settings", "Never go online", "Use only one website" },
                    CorrectAnswer = 1,
                    Explanation = "✅ Using privacy settings and being selective about what you share helps protect your privacy."
                }
            };
        }

        public QuizQuestion StartQuiz()
        {
            ResetQuiz();
            isActive = true;
            return questions[currentQuestionIndex];
        }

        public bool AnswerQuestion(int selectedOption)
        {
            if (!isActive || currentQuestionIndex >= questions.Count)
                return false;

            var question = questions[currentQuestionIndex];
            bool isCorrect = selectedOption == question.CorrectAnswer;

            if (isCorrect)
                score++;

            userAnswers.Add(selectedOption);
            return isCorrect;
        }

        public QuizQuestion GetCurrentQuestion()
        {
            if (currentQuestionIndex < questions.Count)
                return questions[currentQuestionIndex];
            return null;
        }

        public QuizQuestion GetNextQuestion()
        {
            currentQuestionIndex++;
            if (currentQuestionIndex < questions.Count)
                return questions[currentQuestionIndex];

            isActive = false;
            return null;
        }

        public int GetScore()
        {
            return score;
        }

        public string GetScoreFeedback()
        {
            int score = GetScore();
            int total = questions.Count;
            double percentage = (double)score / total * 100;

            if (percentage >= 80)
                return $"🌟 Excellent! {score}/{total} - You're a cybersecurity pro!";
            else if (percentage >= 60)
                return $"👍 Good job! {score}/{total} - Keep learning to improve!";
            else
                return $"📚 {score}/{total} - Keep learning to stay safe online!";
        }

        private void ResetQuiz()
        {
            currentQuestionIndex = 0;
            score = 0;
            isActive = false;
            userAnswers = new List<int>();
        }
    }
}