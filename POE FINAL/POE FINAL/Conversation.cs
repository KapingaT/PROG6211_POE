using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace POE_FINAL
{
    public class Conversation
    {
        // Memory storage
        private static Dictionary<string, string> userInterests = new Dictionary<string, string>();
        private static Dictionary<string, string> lastTopic = new Dictionary<string, string>();
        private static Dictionary<string, int> followUpCount = new Dictionary<string, int>();
        private static Random random = new Random();

        // Random response arrays
        private static string[] phishingResponses = new string[]
        {
            "Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organizations.",
            "Hover over links before clicking to see the actual URL. Don't trust shortened links from unknown sources.",
            "Look for spelling and grammar errors - legitimate companies proofread their emails carefully.",
            "Never enter your password on a site you reached through an email link. Type the URL manually instead.",
            "When in doubt, contact the company directly using their official website."
        };

        private static string[] passwordResponses = new string[]
        {
            "Use a passphrase of 4+ random words (e.g., 'PurpleTigerJumpsHigh') instead of a single complex word.",
            "Never reuse passwords across different accounts - each account needs its own unique password.",
            "Enable Two-Factor Authentication (2FA) whenever possible for an extra layer of security.",
            "Use a password manager like Bitwarden or LastPass to generate and store strong passwords.",
            "Change your passwords immediately if you suspect any account has been compromised."
        };

        private static string[] encouragementMessages = new string[]
        {
            "You're doing great! Every step makes you safer online.",
            "Learning about security is the best protection you can have!",
            "Keep asking questions - cybersecurity knowledge is power!",
            "You're building excellent security habits!",
            "Stay curious! The more you know, the safer you'll be."
        };

        // Sentiment detection
        public enum Sentiment { Worried, Curious, Frustrated, Positive, Neutral }

        public static string GetResponse(string input, string userName)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "I didn't catch that. Could you please type a question? Try asking about passwords, phishing, or malware.";
            }

            input = input.ToLower().Trim();
            input = Regex.Replace(input, @"[^\w\s]", "");

            // Detect sentiment
            Sentiment userSentiment = DetectSentiment(input);
            string sentimentPrefix = GetSentimentPrefix(userSentiment, userName);

            // Check for follow-up
            if (IsFollowUpRequest(input))
            {
                return HandleFollowUp(userName, userSentiment);
            }

            // Store user interests
            if (input.Contains("interested in") || input.Contains("like to learn about"))
            {
                return StoreUserInterest(input, userName);
            }

            // Recall user info
            if (input.Contains("what do you know about me") || input.Contains("remember me"))
            {
                return RecallUserInfo(userName);
            }

            // Check for exit
            if (input.Contains("exit") || input.Contains("bye") || input.Contains("goodbye"))
            {
                return $"Goodbye {userName}! Stay safe online and remember - cybersecurity is a journey, not a destination!";
            }

            // Check for help
            if (input.Contains("help") || input.Contains("what can you do"))
            {
                return GetHelpMessage(userName);
            }

            // Keyword recognition
            string response = GetKeywordResponse(input, userName, userSentiment);
            response = sentimentPrefix + response;

            return response;
        }

        // Sentiment detection method
        private static Sentiment DetectSentiment(string input)
        {
            if (Regex.IsMatch(input, @"worried|scared|afraid|nervous|anxious|stressed|overwhelmed|concerned"))
                return Sentiment.Worried;

            if (Regex.IsMatch(input, @"curious|interesting|tell me|learn|teach|explain|how does|what is"))
                return Sentiment.Curious;

            if (Regex.IsMatch(input, @"frustrated|annoyed|angry|mad|tired|confused|don't understand"))
                return Sentiment.Frustrated;

            if (Regex.IsMatch(input, @"great|awesome|love|thanks|thank you|helpful|good"))
                return Sentiment.Positive;

            return Sentiment.Neutral;
        }

        private static string GetSentimentPrefix(Sentiment sentiment, string userName)
        {
            switch (sentiment)
            {
                case Sentiment.Worried:
                    return "I understand your concern, " + userName + ". It's normal to feel worried about online security. ";
                case Sentiment.Frustrated:
                    return "I hear your frustration, " + userName + ". Cybersecurity can be challenging, but you've got this! ";
                case Sentiment.Curious:
                    return "Great question, " + userName + "! I appreciate your curiosity about security. ";
                case Sentiment.Positive:
                    return "I'm glad you're engaged with cybersecurity, " + userName + "! ";
                default:
                    return "";
            }
        }

        // Follow-up detection
        private static bool IsFollowUpRequest(string input)
        {
            string[] followUpPhrases = { "another", "more", "continue", "tell me more", "explain more", "again", "another tip" };
            return followUpPhrases.Any(phrase => input.Contains(phrase));
        }

        private static string HandleFollowUp(string userName, Sentiment sentiment)
        {
            if (!lastTopic.ContainsKey(userName))
            {
                return "What topic would you like me to tell you more about? Try asking about passwords or phishing first!";
            }

            string topic = lastTopic[userName];
            if (!followUpCount.ContainsKey(userName))
                followUpCount[userName] = 0;

            followUpCount[userName]++;
            string prefix = GetSentimentPrefix(sentiment, userName);

            switch (topic)
            {
                case "password":
                    if (followUpCount[userName] < passwordResponses.Length)
                        return prefix + passwordResponses[followUpCount[userName]];
                    else
                        return prefix + "You've learned a lot about passwords! Would you like to learn about Two-Factor Authentication next?";

                case "phishing":
                    if (followUpCount[userName] < phishingResponses.Length)
                        return prefix + phishingResponses[followUpCount[userName]];
                    else
                        return prefix + "You've mastered phishing basics! Would you like to learn about Social Engineering next?";

                default:
                    return prefix + "I can tell you more about passwords or phishing. What interests you?";
            }
        }

        // Memory methods
        private static string StoreUserInterest(string input, string userName)
        {
            string interest = "";
            if (input.Contains("password")) interest = "password security";
            else if (input.Contains("phish")) interest = "phishing protection";
            else if (input.Contains("malware")) interest = "malware protection";
            else if (input.Contains("privacy")) interest = "online privacy";
            else interest = "cybersecurity";

            if (userInterests.ContainsKey(userName))
                userInterests[userName] = interest;
            else
                userInterests.Add(userName, interest);

            string encouragement = encouragementMessages[random.Next(encouragementMessages.Length)];
            return $"Great! I'll remember that you're interested in {interest}. {encouragement}\n\n" +
                   $"As someone interested in {interest}, here's a quick tip:\n" +
                   GetTipForInterest(interest);
        }

        private static string RecallUserInfo(string userName)
        {
            if (userInterests.ContainsKey(userName))
            {
                string interest = userInterests[userName];
                return $"Of course I remember, {userName}! You're interested in {interest}.\n\n" +
                       $"Would you like to continue learning about {interest}, or explore something new?";
            }
            else
            {
                return $"I don't have any interests saved for you yet, {userName}. " +
                       $"Tell me what you're interested in! For example: 'I'm interested in password security'";
            }
        }

        private static string GetTipForInterest(string interest)
        {
            switch (interest)
            {
                case "password security":
                    return passwordResponses[0];
                case "phishing protection":
                    return phishingResponses[0];
                default:
                    return "Staying informed is your best defense against online threats!";
            }
        }

        // Keyword recognition with random responses
        private static string GetKeywordResponse(string input, string userName, Sentiment sentiment)
        {
            // Greetings
            if (input == "hello" || input == "hi" || input == "hey")
            {
                string[] greetings = {
                    $"Hello {userName}! How can I assist you with cybersecurity today?",
                    $"Hi {userName}! Ready to learn about online safety?"
                };
                return greetings[random.Next(greetings.Length)];
            }

            // Random responses for phishing
            if (input.Contains("phish") || input.Contains("scam") || input.Contains("fake email"))
            {
                lastTopic[userName] = "phishing";
                followUpCount[userName] = 0;
                return phishingResponses[random.Next(phishingResponses.Length)];
            }

            // Random responses for passwords
            if (input.Contains("password") || input.Contains("passphrase"))
            {
                lastTopic[userName] = "password";
                followUpCount[userName] = 0;
                return passwordResponses[random.Next(passwordResponses.Length)];
            }

            // Malware
            if (input.Contains("malware") || input.Contains("virus"))
            {
                string[] malwareResponses = {
                    "Malware includes viruses, worms, and Trojans. Protect yourself by keeping software updated and using antivirus.",
                    "Malware can steal data or take control of your device. Always download from official sources only!"
                };
                return malwareResponses[random.Next(malwareResponses.Length)];
            }

            // Ransomware
            if (input.Contains("ransomware"))
            {
                string[] ransomwareResponses = {
                    "Ransomware locks your files and demands payment. Best defense: Regular backups and never click suspicious links!",
                    "Never pay the ransom! Paying encourages criminals and doesn't guarantee you'll get your files back."
                };
                return ransomwareResponses[random.Next(ransomwareResponses.Length)];
            }

            // Firewall
            if (input.Contains("firewall"))
            {
                string[] firewallResponses = {
                    "A firewall monitors network traffic. Keep Windows Firewall ON - it's your first line of defense!",
                    "Firewalls act like a security guard for your network. Make sure yours is enabled!"
                };
                return firewallResponses[random.Next(firewallResponses.Length)];
            }

            // 2FA
            if (input.Contains("two factor") || input.Contains("2fa"))
            {
                string[] faResponses = {
                    "Two-Factor Authentication adds a second verification step. Enable it on email, banking, and social media!",
                    "2FA options: Authenticator apps are more secure than SMS codes. Hardware keys are even better!"
                };
                return faResponses[random.Next(faResponses.Length)];
            }

            // Identity Theft
            if (input.Contains("identity theft") || input.Contains("identity") || input.Contains("theft"))
            {
                return "Protect against identity theft: Freeze your credit, monitor accounts, use strong passwords, and never share personal info online.";
            }

            // Social Engineering
            if (input.Contains("social engineering"))
            {
                return "Social engineering manipulates people into revealing info. Always verify requests through official channels!";
            }

            // Public WiFi
            if (input.Contains("public wifi"))
            {
                return "Public WiFi is risky! Use a VPN, avoid banking, and turn off file sharing. Better yet, use mobile data for sensitive tasks.";
            }

            // Encryption
            if (input.Contains("encryption"))
            {
                return "Encryption scrambles data so only authorized parties can read it. Use encrypted messaging apps like Signal!";
            }

            // How are you?
            if (input.Contains("how are you"))
            {
                string[] responses = {
                    $"I'm doing great, {userName}! Happy to help you learn about cybersecurity!",
                    $"I'm fantastic, {userName}! Ready to boost your cybersecurity knowledge?"
                };
                return responses[random.Next(responses.Length)];
            }

            // Default response for unrecognized input
            string[] helpfulSuggestions = {
                "I didn't quite understand that. Try asking about: passwords, phishing, malware, ransomware, firewall, or 2FA.",
                "I'm not sure about that. Could you ask about a cybersecurity topic like 'What is a virus?' or 'How do I create strong passwords?'",
                "I can help with cybersecurity topics like passwords, phishing, malware, ransomware, firewalls, and 2FA. What would you like to learn about?"
            };
            return helpfulSuggestions[random.Next(helpfulSuggestions.Length)];
        }

        private static string GetHelpMessage(string userName)
        {
            return $"Cybersecurity Topics I Can Help With, {userName}!\n\n" +
                   "Passwords - How to create and manage strong passwords\n" +
                   "Phishing - Recognize and avoid email/text scams\n" +
                   "Malware and Viruses - Protection against malicious software\n" +
                   "Ransomware - Prevent file-encrypting attacks\n" +
                   "Firewalls - Network protection basics\n" +
                   "2FA - Two-Factor Authentication setup\n" +
                   "Identity Theft - Protect your personal information\n" +
                   "Social Engineering - Recognize manipulation tactics\n" +
                   "Public WiFi - Stay safe on public networks\n" +
                   "Encryption - Keep your data private\n\n" +
                   "Just ask me about any of these topics!\n" +
                   "Try saying: 'Tell me about passwords' or 'Give me a phishing tip'";
        }
    }
}