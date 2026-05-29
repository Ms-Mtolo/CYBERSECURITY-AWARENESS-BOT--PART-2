# Cybersecurity Awareness Bot (Part 2)
A Windows Forms desktop chatbot to promote cybersecurity awareness through interactive, educational conversation.  
**Built with .NET 8 and C#.**

---

## Features

- **Conversational Chatbot:** Friendly UI to educate users about online safety.
- **Cybersecurity Topics:** Passwords, phishing, scams, privacy, 2FA, malware, WiFi safety, software updates, and more.
- **Emotion Awareness:** Detects sentiment and personalizes responses and UI colors.
- **Quick Topics:** One-click buttons for common cybersecurity concerns.
- **Inline Guidance:** Definitions, details, and tips with just a click.
- **Personalized Learning:** Remembers user’s name and favorite topic.

## How it Works

- The app starts with a name entry form, then opens the MainChatForm for chat.
- Users can type questions or use quick buttons for topics such as passwords, phishing, privacy, and scams.
- The chatbot responds using detailed knowledge and adapts its teaching to the user's interests and mood.
- Memory panel highlights the user's favorite cybersecurity topic.
- Built for beginner-to-intermediate users and suitable for educational demos.

## Main Components

- **MainChatForm.cs** — main window, UI logic, interaction flow.
- **ChatbotBrain.cs** — chatbot logic, knowledge, memory, response generation.
- **Program.cs** — app entry point, initializes Windows Forms.
- **CybersecurityChatbotGUIPart2.csproj** — project configuration (.NET 8, Windows).
- **NameEntryForm.cs** — collects user’s name before starting the chat.

## Getting Started

1. **Requirements:**
   - .NET 8 SDK or Visual Studio 2022+
   - Windows (required for WinForms)

2. **Run Locally:**
   ```
   git clone https://github.com/Ms-Mtolo/CYBERSECURITY-AWARENESS-BOT--PART-2.git
   cd CYBERSECURITY-AWARENESS-BOT--PART-2
   dotnet build
   dotnet run --project CybersecurityChatbotGUIPart2.csproj
   ```

3. **Build & Test:**
   - Runs are automated with GitHub Actions (see below for CI configuration).

---

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for more.

## Credits

Created by [Ms-Mtolo](https://github.com/Ms-Mtolo)
