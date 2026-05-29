using System;
using System.Collections.Generic;

namespace CybersecurityChatbotGUI
{
    // A delegate is like a "variable that holds a method".
    // This one holds any method that takes a (string sentiment, string color)
    // We use it so the brain can tell the UI "hey, display this in red/green"

    public delegate void SentimentDetectedHandler(string sentiment, string colorHex);

    public class ChatbotBrain
    {
        public string UserName { get; set; } = "";
        public string FavouriteTopic { get; private set; } = "";
        public string LastTopic { get; private set; } = "";

        // When we detect a sentiment, we "fire" this delegate
        public SentimentDetectedHandler? OnSentimentDetected;

        // Random object used for picking random responses
        private readonly Random _random = new Random();

        // Dictionary maps a keyword (string) to a LIST of possible responses.
        // The chatbot randomly picks one from the list.
        private readonly Dictionary<string, List<string>> _keywordResponses = new Dictionary<string, List<string>>
        {
            ["password"] = new List<string>
            {
                "A password is a secret word or phrase used to authenticate access to a digital account or device.",
                "In computing, passwords are combined with usernames to verify identity and grant access to systems.",
                "Password strength is measured by length, complexity, and unpredictability against guessing attacks.",
                "Passwords are the most common form of authentication, storing a hash rather than the actual password for security."
            },
            ["phishing"] = new List<string>
            {
                "Phishing is a cyberattack where attackers disguise themselves as legitimate entities to steal sensitive data.",
                "The term 'phishing' was coined in 1996 and combines 'fishing' with 'phreaking' (hacking phone systems).",
                "Phishing attacks typically arrive via email, text, or phone calls impersonating trusted organizations.",
                "Spear phishing targets specific individuals while whaling targets high-profile executives."
            },
            ["scam"] = new List<string>
            {
                "A scam is a fraudulent scheme designed to trick victims into giving away money, data, or valuables.",
                "Online scams include advance-fee fraud, fake tech support, romance scams, and investment fraud.",
                "Scams rely on social engineering - manipulating human psychology rather than technical hacking.",
                "The global cost of online scams exceeds $1 trillion annually, affecting millions of victims worldwide."
            },
            ["privacy"] = new List<string>
            {
                "Data privacy refers to the proper handling of personal information including collection, storage, and sharing.",
                "Privacy laws like POPIA (South Africa) and GDPR (Europe) give individuals rights over their personal data.",
                "Digital privacy encompasses everything from browsing history and location data to biometric information.",
                "Privacy is not the same as security - security protects data, privacy controls who can access it."
            },
            ["2fa"] = new List<string>
            {
                "Two-Factor Authentication (2FA) requires two different authentication factors to verify identity.",
                "The three authentication factors are: something you know (password), have (phone), or are (fingerprint).",
                "2FA adds a second layer after password entry, typically a 6-digit time-based code from an authenticator app.",
                "Multi-factor authentication (MFA) extends 2FA by requiring two or more factors for verification."
            },
            ["malware"] = new List<string>
            {
                "Malware (malicious software) is any program intentionally designed to damage or exploit computer systems.",
                "Common malware types include viruses (self-replicating), worms (network-spreading), and trojans (disguised).",
                "Ransomware is malware that encrypts files and demands payment for decryption keys.",
                "Spyware secretly monitors user activity while adware forces unwanted advertisements onto devices."
            },
            ["wifi"] = new List<string>
            {
                "Wi-Fi (Wireless Fidelity) is a wireless networking technology using radio waves for internet connectivity.",
                "Wi-Fi operates on 2.4 GHz or 5 GHz frequencies under IEEE 802.11 standards.",
                "Open Wi-Fi networks have no encryption, allowing anyone on the same network to intercept unencrypted traffic.",
                "WPA3 is the current Wi-Fi security standard, replacing the older WPA2 protocol."
            },
            ["update"] = new List<string>
            {
                "A software update (patch) is a code change that fixes bugs, adds features, or closes security vulnerabilities.",
                "Security patches address known vulnerabilities (CVEs) that attackers could otherwise exploit.",
                "Zero-day vulnerabilities are flaws exploited before the developer creates a security update.",
                "Automated updates are critical because attackers scan for unpatched systems within hours of patch releases."
            }
        };

        // Dictionary maps a sentiment word to a (feeling description, color)
        private readonly Dictionary<string, (string message, string colorHex)> _sentimentMap =
            new Dictionary<string, (string, string)>
            {
                ["worried"] = ("I can sense you're worried. That's completely understandable!", "#FF8C00"),
                ["scared"] = ("It's okay to feel scared about cyber threats.", "#FF8C00"),
                ["anxious"] = ("Feeling anxious about online safety is very common.", "#FF8C00"),
                ["frustrated"] = ("I hear you - cybersecurity can feel overwhelming.", "#FF4500"),
                ["confused"] = ("No worries at all - these topics can be confusing at first.", "#9370DB"),
                ["curious"] = ("I love your curiosity! That's the best mindset for learning.", "#00CED1"),
                ["excited"] = ("That excitement will take you far in cybersecurity!", "#00CED1"),
                ["happy"] = ("Great! A positive mindset makes learning much easier.", "#32CD32"),
                ["good"] = ("Wonderful! Let's keep that energy going.", "#32CD32"),
                ["great"] = ("Fantastic! You're in a great headspace to learn today.", "#32CD32"),
                ["terrible"] = ("I'm sorry to hear that. I'll try to keep things simple and helpful.", "#FF8C00"),
                ["bad"] = ("I'm sorry you're not feeling great. Let's focus on what we can control online.", "#FF8C00"),
                ["overwhelmed"] = ("Let's take it one step at a time - cybersecurity doesn't have to be complicated.", "#FF8C00"),
            };

        // PHISHING TIPS (for "give me another tip" feature)
        private readonly List<string> _phishingTips = new List<string>
        {
            "Be cautious of emails with urgent subject lines like 'Your account will be suspended!'",
            "Always check: does the email address match the real company domain?",
            "Poor spelling and grammar in professional emails is a big red flag.",
            "Hover over links BEFORE clicking - check if the URL matches what's shown.",
            "If you're unsure, go directly to the company's official website instead of clicking the link.",
            "Legitimate banks never ask for your PIN or full password via email.",
            "Be suspicious of unexpected attachments, even from people you know - their account may be hacked."
        };
        private int _phishingTipIndex = 0; // tracks which tip we're on

        // CONVERSATION FLOW TRACKING
        public string CurrentTopic { get; private set; } = "";

        // This is the main method the UI calls.
        // Input: what the user typed
        // Output: the chatbot's response as a string
        public string ProcessInput(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
                return "I didn't catch that. Could you try rephrasing?";

            string input = userInput.Trim().ToLower();

            if (input == "exit" || input == "quit" || input == "bye" ||
                input == "goodbye" || input.Contains("exit program") || input.Contains("close"))
            {
                return "EXIT_CONFIRMED";
            }

            // Check sentiment first 
            string sentimentResponse = CheckSentiment(input);

            // Check for follow-up requests
            if (input.Contains("another tip") || input.Contains("tell me more") ||
                input.Contains("more info") || input.Contains("explain more") ||
                input.Contains("give me more") || input.Contains("another"))
            {
                return HandleFollowUp();
            }

            // Check for keyword matches 
            string? keywordResponse = CheckKeywords(input);
            if (keywordResponse != null)
            {
                // If we also detected a sentiment, prepend the sentiment response
                if (!string.IsNullOrEmpty(sentimentResponse))
                    return sentimentResponse + "\n\n" + keywordResponse;
                return keywordResponse;
            }

            // Sentiment-only response (no keyword found) 
            if (!string.IsNullOrEmpty(sentimentResponse))
                return sentimentResponse + "\n\nWould you like me to share some cybersecurity tips on a topic? Try asking about passwords, phishing, privacy, or scams.";

            // Greetings 
            if (input.Contains("hello") || input.Contains("hi") || input.Contains("hey") ||
                input.Contains("good day") || input.Contains("good morning") || input.Contains("good evening"))
            {
                return $"Hello {UserName}! Great to see you. What cybersecurity topic would you like to explore today?";
            }

            // Thanks
            if (input.Contains("thank"))
                return $"You're very welcome, {UserName}! Staying informed is the first step to staying safe online.";

            // Default - unknown input
            return $"I'm not sure I understand that, {UserName}. Could you try rephrasing?\n\n" +
                   "You can ask me about: passwords, phishing, scams, privacy, 2FA, malware, WiFi safety, or software updates.";
        }

        // Looks through the _keywordResponses dictionary
        // Returns a random response if a keyword is found
        // Returns null if nothing matched
        private string? CheckKeywords(string input)
        {
            foreach (var entry in _keywordResponses)
            {
                // entry.Key = "password", entry.Value = List of responses
                if (input.Contains(entry.Key))
                {
                    CurrentTopic = entry.Key;  // Remember what we're talking about
                    RememberFavouriteTopic(entry.Key, input);

                    // Pick a RANDOM response from the list
                    int randomIndex = _random.Next(entry.Value.Count);
                    string response = entry.Value[randomIndex];

                    // Add personalisation if we know their favourite topic
                    if (FavouriteTopic == entry.Key && !string.IsNullOrEmpty(FavouriteTopic))
                        response = $"As someone interested in {FavouriteTopic}, here's something important: " + response;

                    return response + "\n\n Type 'another tip' to get a different tip on this topic!";
                }
            }
            return null; // Nothing matched
        }

        // Scans the input for emotion words.
        // If found, fires the delegate so the UI can change color, and returns an empathetic response string.
        private string CheckSentiment(string input)
        {
            foreach (var sentiment in _sentimentMap)
            {
                if (input.Contains(sentiment.Key))
                {
                    // FIRE THE DELEGATE - notify the UI about this sentiment
                    // The UI will then change the chat bubble color
                    OnSentimentDetected?.Invoke(sentiment.Key, sentiment.Value.colorHex);

                    return sentiment.Value.message;
                }
            }
            return ""; // No sentiment found
        }

        private void RememberFavouriteTopic(string keyword, string fullInput)
        {
            if (fullInput.Contains("interested in") || fullInput.Contains("i like") ||
                fullInput.Contains("i love") || fullInput.Contains("i want to learn about") ||
                fullInput.Contains("favourite topic"))
            {
                FavouriteTopic = keyword;
                LastTopic = keyword;
            }
        }

        // Called when user selects a topic from the menu buttons
        public string SetFavouriteTopic(string topic)
        {
            FavouriteTopic = topic;
            return $"Great! I'll remember that you're interested in {topic}. It's a crucial part of staying safe online. 🔒\n\nI'll tailor my tips to your interest as we chat!";
        }

        // Handles "tell me more", "another tip", etc.
        private string HandleFollowUp()
        {
            if (string.IsNullOrEmpty(CurrentTopic))
                return "What topic would you like more information on? Try asking about passwords, phishing, scams, or privacy.";

            if (CurrentTopic == "phishing" || CurrentTopic == "scam")
            {
                // Cycle through the phishing tips list
                string tip = _phishingTips[_phishingTipIndex % _phishingTips.Count];
                _phishingTipIndex++;
                return $"Here's another tip on phishing/scams:\n\n {tip}";
            }

            // For other topics, pick a new random response
            if (_keywordResponses.ContainsKey(CurrentTopic))
            {
                int randomIndex = _random.Next(_keywordResponses[CurrentTopic].Count);
                return $"Here's more on {CurrentTopic}:\n\n" + _keywordResponses[CurrentTopic][randomIndex];
            }

            return $"I have more tips on {CurrentTopic} if you'd like. What specifically would you like to know?";
        }

        // Called when user submits their name - returns opening message
        public string GetWelcomeResponse(string name)
        {
            UserName = name;
            return $"Welcome, {UserName}! \n\n" +
                   "I'm your Cybersecurity Awareness Assistant. I'm here to help you stay safe online.\n\n" +
                   "You can ask me about:\n" +
                   "  Passwords & Password Managers\n" +
                   "  Phishing & Scam Emails\n" +
                   "  Privacy Online\n" +
                   "  Two-Factor Authentication (2FA)\n" +
                   "  Malware & Viruses\n" +
                   "  Public WiFi Safety\n" +
                   "  Software Updates\n\n" +
                   "Or just type how you're feeling and I'll respond accordingly!\n\nHow are you doing today?";
        }

        // Called when user first selects a topic (button click or keyword).
        // Returns ONLY a short definition — the UI then shows
        // "Would you like to know more?" buttons.
        public string GetTopicDefinition(string topic)
        {
            CurrentTopic = topic;

            return topic switch
            {
                "password" =>
                    " PASSWORD SAFETY\n" +
                    "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n" +
                    " DEFINITION:\n" +
                    "A password is a secret word or phrase used to prove your identity online.\n" +
                    "Password safety means choosing passwords that are hard for others to guess\n" +
                    "or crack, so that only YOU can access your accounts.\n\n" +
                    "Would you like to know more about Password Safety?",

                "phishing" =>
                    " PHISHING ATTACKS\n" +
                    "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n" +
                    " DEFINITION:\n" +
                    "Phishing is a type of online scam where criminals send fake emails, texts,\n" +
                    "or messages pretending to be a trusted company (like your bank or PayPal)\n" +
                    "to trick you into giving away your passwords or personal information.\n\n" +
                    "Would you like to know more about Phishing Attacks?",

                "privacy" =>
                    " PRIVACY ONLINE\n" +
                    "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n" +
                    " DEFINITION:\n" +
                    "Online privacy is your right to control what personal information you share\n" +
                    "on the internet — including your name, location, photos, and browsing habits.\n" +
                    "Protecting your privacy means fewer people can track, target, or exploit you.\n\n" +
                    "Would you like to know more about Online Privacy?",

                "2fa" =>
                    " TWO-FACTOR AUTHENTICATION (2FA)\n" +
                    "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n" +
                    " DEFINITION:\n" +
                    "Two-Factor Authentication (2FA) is an extra security step when logging in.\n" +
                    "Instead of just a password, you also need a second proof of identity —\n" +
                    "usually a code sent to your phone. Even if someone steals your password,\n" +
                    "they still can't get in without that second code.\n\n" +
                    "Would you like to know more about 2FA?",

                "scam" =>
                    " SCAM AWARENESS\n" +
                    "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n" +
                    " DEFINITION:\n" +
                    "An online scam is a dishonest scheme carried out over the internet to\n" +
                    "trick people into giving away money or personal information.\n" +
                    "Scammers often pretend to be officials, businesses, or even people you know.\n\n" +
                    "Would you like to know more about Scam Awareness?",

                "wifi" =>
                    " PUBLIC WiFi SAFETY\n" +
                    "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n" +
                    " DEFINITION:\n" +
                    "Public WiFi refers to wireless internet connections available in public places\n" +
                    "like cafés, airports, and malls. These networks are often unsecured, meaning\n" +
                    "hackers nearby can potentially intercept and read your internet traffic.\n\n" +
                    "Would you like to know more about WiFi Safety?",

                _ => "I have information on passwords, phishing, privacy, 2FA, scams, and WiFi safety. Which would you like to explore?"
            };
        }

        // Called when user clicks "Yes, tell me more" button
        public string GetTopicDetails(string topic)
        {
            return topic switch
            {
                "password" =>
                    "PASSWORD SAFETY — FULL GUIDE\n" +
                    "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n" +
                    " DO:\n" +
                    "  • Use 12+ characters\n" +
                    "  • Mix uppercase, lowercase, numbers, symbols\n" +
                    "  • Use a different password for every account\n" +
                    "  • Use a password manager (Bitwarden is free!)\n\n" +
                    " DON'T:\n" +
                    "  • Use 'password123' or 'qwerty'\n" +
                    "  • Use your birthday or pet's name\n" +
                    "  • Reuse the same password on multiple sites\n\n" +
                    " Strong example: 'Purple$Coffee%Mountain#42'\n" +
                    " Type 'another tip' to get a random password tip!",

                "phishing" =>
                    " PHISHING ATTACKS — FULL GUIDE\n" +
                    "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n" +
                    " RED FLAGS:\n" +
                    "  • Urgent language: 'Your account will be CLOSED!'\n" +
                    "  • Wrong email domain: 'paypa1.com' instead of 'paypal.com'\n" +
                    "  • Generic greeting: 'Dear Customer' (not your name)\n" +
                    "  • Spelling mistakes in professional emails\n" +
                    "  • Links that look slightly wrong\n\n" +
                    " WHAT TO DO:\n" +
                    "  • Don't click links in suspicious emails\n" +
                    "  • Go directly to the website instead\n" +
                    "  • Call the company using their official number\n\n" +
                    " Type 'another tip' to get another phishing tip!",

                "privacy" =>
                    " PRIVACY ONLINE — FULL GUIDE\n" +
                    "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n" +
                    " SOCIAL MEDIA:\n" +
                    "  • Set your profile to Private\n" +
                    "  • Don't post your home address or ID number\n" +
                    "  • Don't post 'I'm on holiday!' - it tells thieves you're away\n\n" +
                    " BROWSING:\n" +
                    "  • Only use HTTPS websites (look for the padlock 🔒)\n" +
                    "  • Use a VPN on public WiFi\n" +
                    "  • Check app permissions - does a game need your contacts?\n\n" +
                    " Type 'another tip' for more privacy tips!",

                "2fa" =>
                    " TWO-FACTOR AUTHENTICATION — FULL GUIDE\n" +
                    "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n" +
                    " HOW IT WORKS:\n" +
                    "  1. You enter your password ✓\n" +
                    "  2. Website sends a code to your phone\n" +
                    "  3. You enter the code ✓\n" +
                    "  4. Now you're in!\n\n" +
                    "Even if a hacker steals your password, they STILL can't get in without your phone!\n\n" +
                    " TYPES:\n" +
                    "  • SMS text code (basic)\n" +
                    "  • Authenticator app - Google Authenticator (more secure)\n" +
                    "  • Hardware key - YubiKey (most secure)\n\n" +
                    " Enable 2FA on your email, banking, and social media accounts NOW!",

                "scam" =>
                    " SCAM AWARENESS — FULL GUIDE\n" +
                    "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n" +
                    " COMMON SCAMS IN SOUTH AFRICA:\n" +
                    "  • 'You've won a prize!' - you haven't, it's fake\n" +
                    "  • Fake job offers asking for upfront payment\n" +
                    "  • Romance scams - building trust then asking for money\n" +
                    "  • SARS / government impersonation calls\n\n" +
                    " SCAMMER TACTICS:\n" +
                    "  • Creating urgency ('Act NOW!')\n" +
                    "  • Requesting gift cards or cash transfers\n" +
                    "  • Pretending to be official (police, bank, government)\n\n" +
                    " RULE: If something feels wrong, it probably is. Stop, verify, then act.\n\n" +
                    " Report scams: www.sabric.co.za",

                "wifi" =>
                    " PUBLIC WiFi SAFETY — FULL GUIDE\n" +
                    "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n" +
                    " WHAT COULD HAPPEN ON PUBLIC WiFi:\n" +
                    "  • Hackers create fake hotspots with similar names\n" +
                    "  • They capture your passwords and messages\n" +
                    "  • Your banking details could be stolen\n\n" +
                    " HOW TO STAY SAFE:\n" +
                    "  • Ask staff for the EXACT WiFi name\n" +
                    "  • Use a VPN (Virtual Private Network)\n" +
                    "  • Avoid banking or shopping on public WiFi\n" +
                    "  • Use your phone's mobile data hotspot instead\n\n" +
                    " Type 'another tip' for more WiFi safety tips!",

                _ => "I have more details on passwords, phishing, privacy, 2FA, scams, and WiFi. Which would you like to explore?"
            };
        }
    }
}