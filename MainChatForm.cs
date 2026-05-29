
// MainChatForm.cs
// This is the MAIN CHAT WINDOW - the "heart" of the application.
//
// FEATURES IMPLEMENTED HERE:
//    GUI Design (coloured panels, styled controls)
//    Keyword Recognition (handled by ChatbotBrain)
//    Random Responses (handled by ChatbotBrain)
//    Conversation Flow (follow-up prompts)
//    Memory & Recall (favourite topic stored in brain)
//    Sentiment Detection (colour changes + empathy)
//    Error Handling (default responses for unknown input)
//    Topic Buttons (quick access shortcuts)
//    Chat history with timestamps


using System;
using System.Drawing;
using System.Windows.Forms;

namespace CybersecurityChatbotGUI
{
    public class MainChatForm : Form
    {
        
        

        // The brain - handles all chatbot logic
        private ChatbotBrain _brain;

        // Current sentiment colour (changes based on user mood)
        // Default is cyan (neutral)
        private Color _currentSentimentColor = Color.FromArgb(0, 200, 255);

        // CONTROLS - all visual elements
       
        private Panel headerPanel;
        private Label titleLabel;
        private Label userStatusLabel;
        private RichTextBox chatDisplay;       // shows the chat history
        private TextBox inputBox;          // where user types
        private Button sendButton;
        private Panel topicButtonPanel;  // holds the quick-topic buttons
        private Panel memoryPanel;       // shows memory/recall info
        private Label memoryLabel;
        private Label sentimentLabel;    // shows detected sentiment
        private Panel bottomPanel;
        private Button exitButton;        // lets user exit the program cleanly

       
        
        // Receives the user's name from the NameEntryForm
        
        public MainChatForm(string userName)
        {
            // Create the brain and set the user's name
            _brain = new ChatbotBrain();
            _brain.UserName = userName;

            // This is where we connect the delegate.
            // We assign our method "HandleSentimentDetected" to the delegate.
            // Now whenever the brain calls OnSentimentDetected, our method runs!
            _brain.OnSentimentDetected = HandleSentimentDetected;

            // Build the window
            SetupWindow();
            BuildUI();

            // Shows welcome message after controls are ready
            // (We use Load event so the form is fully visible first)
            this.Load += MainChatForm_Load;
        }

        
        
        // works once when the form first appears on screen
        
        private void MainChatForm_Load(object? sender, EventArgs e)
        {
            // Display welcome message in the chat
            string welcome = _brain.GetWelcomeResponse(_brain.UserName);
            AppendBotMessage(welcome);

            // Focus the input box so user can type immediately
            inputBox.Focus();
        }

        private void SetupWindow()
        {
            this.Text= $"Cybersecurity Bot - Chatting with {_brain.UserName}";
            this.Size = new Size(900, 780);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(13, 17, 23);
            this.Font = new Font("Consolas", 9f);
            this.MinimumSize = new Size(800, 700);
        }

        
        // Creates every control on the main chat form
        
        private void BuildUI()
        {
            
            //  header panel (top bar)
            
headerPanel = new Panel();
 headerPanel.BackColor = Color.FromArgb(0, 40, 20);
  headerPanel.Dock = DockStyle.Top;
   headerPanel.Height = 60;

titleLabel = new Label();
 titleLabel.Text = " CYBERSECURITY AWARENESS BOT";
  titleLabel.Font  = new Font("Consolas", 14f, FontStyle.Bold);
   titleLabel.ForeColor = Color.LimeGreen;
    titleLabel.AutoSize  = false;
     titleLabel.Size      = new Size(500, 60);
      titleLabel.Location  = new Point(15, 0);
       titleLabel.TextAlign = ContentAlignment.MiddleLeft;

userStatusLabel = new Label();
 userStatusLabel.Text = $" {_brain.UserName}";
  userStatusLabel.Font = new Font("Consolas", 10f);
   userStatusLabel.ForeColor = Color.Cyan;
    userStatusLabel.AutoSize  = false;
     userStatusLabel.Size = new Size(350, 60);
      userStatusLabel.Location  = new Point(530, 0);
       userStatusLabel.TextAlign = ContentAlignment.MiddleRight;

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(userStatusLabel);

            
            // sentiment ditactor (below header)
           
sentimentLabel = new Label();
 sentimentLabel.Text = " Mood: Neutral";
  sentimentLabel.Font = new Font("Consolas", 9f);
   sentimentLabel.ForeColor = Color.Gray;
    sentimentLabel.BackColor = Color.FromArgb(20, 25, 35);
     sentimentLabel.Dock = DockStyle.None;
      sentimentLabel.AutoSize  = false;
       sentimentLabel.Size = new Size(900, 25);
        sentimentLabel.Location  = new Point(0, 60);
         sentimentLabel.TextAlign = ContentAlignment.MiddleLeft;
          sentimentLabel.Padding = new Padding(10, 0, 0, 0);

            
            topicButtonPanel  = new Panel();
             topicButtonPanel.BackColor = Color.FromArgb(20, 30, 20);
              topicButtonPanel.AutoSize  = false;
               topicButtonPanel.Size = new Size(900, 50);
                topicButtonPanel.Location  = new Point(0, 85);

            var topicLabel  = new Label();
            topicLabel.Text = "Quick Topics:";
             topicLabel.ForeColor = Color.Gray;
              topicLabel.Font = new Font("Consolas", 8f);
               topicLabel.AutoSize = true;
                topicLabel.Location = new Point(8, 16);

            // Helper to create topic buttons - keeps code DRY (Don't Repeat Yourself)
            Button MakeTopicButton(string text, string topic, int x)
            {
var btn = new Button();
 btn.Text = text;
  btn.Tag = topic;  // store the topic name in Tag property
   btn.Font = new Font("Consolas", 8f);
    btn.BackColor = Color.FromArgb(0, 60, 30);
     btn.ForeColor = Color.LimeGreen;
      btn.FlatStyle = FlatStyle.Flat;
       btn.Size = new Size(115, 32);
        btn.Location = new Point(x, 9);
         btn.Cursor = Cursors.Hand;
          btn.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 60);
           btn.Click += TopicButton_Click;  // all buttons share one handler
                return btn;
            }

            // Create all topic buttons
            topicButtonPanel.Controls.Add(topicLabel);
             topicButtonPanel.Controls.Add(MakeTopicButton(" Passwords",  "password", 100));
              topicButtonPanel.Controls.Add(MakeTopicButton(" Phishing",   "phishing", 220));
               topicButtonPanel.Controls.Add(MakeTopicButton(" Privacy",    "privacy",  340));
                topicButtonPanel.Controls.Add(MakeTopicButton(" 2FA",        "2fa",      460));
                 topicButtonPanel.Controls.Add(MakeTopicButton(" Scams",      "scam",     580));
                  topicButtonPanel.Controls.Add(MakeTopicButton(" WiFi Safety","wifi",     700));

            
memoryPanel = new Panel();
 memoryPanel.BackColor = Color.FromArgb(20, 20, 40);
  memoryPanel.Size = new Size(900, 28);
   memoryPanel.Location  = new Point(0, 135);
    memoryPanel.Visible = false;  // hidden until we have something to remember

            memoryLabel = new Label();
             memoryLabel.Text = " Memory: ...";
              memoryLabel.Font = new Font("Consolas", 8.5f, FontStyle.Italic);
               memoryLabel.ForeColor = Color.Plum;
                memoryLabel.AutoSize  = false;
                 memoryLabel.Size = new Size(880, 28);
                  memoryLabel.Location  = new Point(10, 0);
                   memoryLabel.TextAlign = ContentAlignment.MiddleLeft;

            memoryPanel.Controls.Add(memoryLabel);

            
chatDisplay = new RichTextBox();
 chatDisplay.BackColor = Color.FromArgb(13, 17, 23);
  chatDisplay.ForeColor = Color.White;
   chatDisplay.Font = new Font("Consolas", 10f);
    chatDisplay.ReadOnly = true;   // user can't edit chat history
     chatDisplay.BorderStyle = BorderStyle.None;
      chatDisplay.ScrollBars = RichTextBoxScrollBars.Vertical;
       chatDisplay.Size = new Size(900, 430);
        chatDisplay.Location = new Point(0, 163);
         chatDisplay.Padding = new Padding(10);
          chatDisplay.WordWrap = true;

            
            bottomPanel = new Panel();
              bottomPanel.BackColor = Color.FromArgb(20, 25, 35);
               bottomPanel.Size = new Size(900, 70);
                bottomPanel.Location = new Point(0, 693);
                 bottomPanel.Dock = DockStyle.Bottom;

            inputBox                  = new TextBox();
             inputBox.Font             = new Font("Consolas", 11f);
              inputBox.BackColor        = Color.FromArgb(30, 40, 50);
               inputBox.ForeColor        = Color.White;
                 inputBox.BorderStyle      = BorderStyle.FixedSingle;
                  inputBox.Size             = new Size(580, 40);
                   inputBox.Location         = new Point(10, 15);
                    inputBox.PlaceholderText  = "Type your message here and press Enter or click Send...";
            // Wire up Enter key
            inputBox.KeyPress += InputBox_KeyPress;

sendButton = new Button();
 sendButton.Text = "SEND  ";
  sendButton.Font = new Font("Consolas", 10f, FontStyle.Bold);
   sendButton.BackColor = Color.FromArgb(0, 120, 60);
    sendButton.ForeColor = Color.White;
     sendButton.FlatStyle = FlatStyle.Flat;
      sendButton.Size = new Size(150, 40);
       sendButton.Location = new Point(600, 15);
        sendButton.Cursor  = Cursors.Hand;
         sendButton.FlatAppearance.BorderColor = Color.LimeGreen;
          sendButton.Click += SendButton_Click;

            // EXIT BUTTON - lets user leave the program gracefully
exitButton = new Button();
 exitButton.Text = " EXIT";
  exitButton.Font = new Font("Consolas", 10f, FontStyle.Bold);
   exitButton.BackColor = Color.FromArgb(120, 20, 20);   // dark red
    exitButton.ForeColor = Color.White;
     exitButton.FlatStyle = FlatStyle.Flat;
      exitButton.Size = new Size(110, 40);
       exitButton.Location = new Point(760, 15);
        exitButton.Cursor = Cursors.Hand;
         exitButton.FlatAppearance.BorderColor = Color.OrangeRed;
            // Wire up click to our exit method
            exitButton.Click += ExitButton_Click;

            bottomPanel.Controls.Add(inputBox);
             bottomPanel.Controls.Add(sendButton);
              bottomPanel.Controls.Add(exitButton);

            
            // Adds EVERYTHING TO THE FORM
            // Order matters! Controls added later appear on top.
           
            this.Controls.Add(headerPanel);
              this.Controls.Add(sentimentLabel);
               this.Controls.Add(topicButtonPanel);
                this.Controls.Add(memoryPanel);
                 this.Controls.Add(chatDisplay);
                  this.Controls.Add(bottomPanel);

            // Handle form resize to keep chat display the right size
            this.Resize += MainChatForm_Resize;
        }

        
        // Keeps the chat display filling the available space
        // when the user resizes the window
        
        private void MainChatForm_Resize(object? sender, EventArgs e)
        {
            int topY = memoryPanel.Visible ? 163 : 163;
            int bottomHeight = bottomPanel.Height;
            int available = this.ClientSize.Height - topY - bottomHeight;

            chatDisplay.Width  = this.ClientSize.Width;
             chatDisplay.Height = available > 100 ? available : 100;
              chatDisplay.Top  = topY;

            bottomPanel.Width = this.ClientSize.Width;

            topicButtonPanel.Width  = this.ClientSize.Width;
             sentimentLabel.Width = this.ClientSize.Width;
              memoryPanel.Width = this.ClientSize.Width;
               headerPanel.Width = this.ClientSize.Width;

            userStatusLabel.Left = this.ClientSize.Width - userStatusLabel.Width - 10;
            // Keep exit button at the far right, send button next to it, input box fills the rest
            exitButton.Left  = this.ClientSize.Width - exitButton.Width - 10;
            sendButton.Left  = exitButton.Left - sendButton.Width - 8;
            inputBox.Width = sendButton.Left - inputBox.Left - 8;
        }

        
        
        // starts when user clicks the Send button
        
        private void SendButton_Click(object? sender, EventArgs e)
        {
            ProcessUserMessage(inputBox.Text);
        }

        
        // starts on every key press in the input box
        // We use it to detect Enter key
        
        private void InputBox_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;  // prevents the 'ding' sound on Enter
                ProcessUserMessage(inputBox.Text);
            }
        }

        
        // starts when user clicks any of the quick-topic buttons
        private void TopicButton_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is string topic)
            {
                AppendUserMessage($"[Clicked: {btn.Text.Trim()}]");

                // Show definition only first
                string definition = _brain.GetTopicDefinition(topic);
                AppendBotMessage(definition);

                // Now show two inline buttons: "Yes, tell me more!" and "No thanks"
                ShowKnowMoreButtons(topic);

                _brain.SetFavouriteTopic(topic);
                UpdateMemoryPanel(topic);
                inputBox.Focus();
            }
        }

       
        // Adds two small buttons directly into the chat area
        // so the user can choose whether they want more details.
        
        private void ShowKnowMoreButtons(string topic)
        {
            // Container panel to hold both buttons side-by-side
            var panel = new Panel();
            panel.BackColor  = Color.FromArgb(20, 30, 20);
            panel.Size = new Size(440, 45);
            // Position it just below the chat display, above the bottom bar
            panel.Location = new Point(10, chatDisplay.Bottom - 50);
            panel.BringToFront();

var yesBtn = new Button();
 yesBtn.Text = "Yes, tell me more!";
  yesBtn.Font  = new Font("Consolas", 9f, FontStyle.Bold);
   yesBtn.BackColor = Color.FromArgb(0, 100, 40);
    yesBtn.ForeColor = Color.White;
     yesBtn.FlatStyle = FlatStyle.Flat;
      yesBtn.Size = new Size(210, 35);
       yesBtn.Location = new Point(0, 5);
        yesBtn.Cursor = Cursors.Hand;
         yesBtn.FlatAppearance.BorderColor = Color.LimeGreen;

var noBtn = new Button();
 noBtn.Text = "  No thanks";
  noBtn.Font = new Font("Consolas", 9f, FontStyle.Bold);
   noBtn.BackColor = Color.FromArgb(80, 30, 30);
    noBtn.ForeColor = Color.White;
     noBtn.FlatStyle = FlatStyle.Flat;
      noBtn.Size = new Size(210, 35);
       noBtn.Location = new Point(220, 5);
        noBtn.Cursor = Cursors.Hand;
         noBtn.FlatAppearance.BorderColor = Color.OrangeRed;

            // YES clicked: show full details then remove the buttons
            yesBtn.Click += (s, e) =>
            {
                AppendUserMessage("Yes, tell me more!");
                string details = _brain.GetTopicDetails(topic);
                AppendBotMessage(details);
                this.Controls.Remove(panel);  // remove the yes/no buttons
                panel.Dispose();              // free memory
                inputBox.Focus();
            };

            // NO clicked: politely move on then remove the buttons
            noBtn.Click += (s, e) =>
            {
                AppendUserMessage("No thanks.");
                AppendBotMessage($"No problem! Feel free to ask about any other topic, or click one of the buttons above. 😊");
                this.Controls.Remove(panel);
                panel.Dispose();
                inputBox.Focus();
            };

            panel.Controls.Add(yesBtn);
            panel.Controls.Add(noBtn);
            this.Controls.Add(panel);
            panel.BringToFront();
        }

        
        // Central method called whenever user sends a message
       
        private void ProcessUserMessage(string userText)
        {
            string trimmed = userText.Trim();

            // Don't process empty messages
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                inputBox.Clear();
                return;
            }

            // Show user's message in the chat
            AppendUserMessage(trimmed);

            // Clear the input box
            inputBox.Clear();

            // Get the bot's response from the brain
            string response = _brain.ProcessInput(trimmed);

            //  Check if the brain returned an exit signal
            // The brain returns "EXIT_CONFIRMED" when user types exit/bye/quit
            if (response == "EXIT_CONFIRMED")
            {
                ConfirmAndExit();
                return;
            }

            // Show bot's response
            AppendBotMessage(response);

            // Update memory panel if brain now remembers a favourite topic
            if (!string.IsNullOrEmpty(_brain.FavouriteTopic))
            {
                UpdateMemoryPanel(_brain.FavouriteTopic);
            }

            inputBox.Focus();
        }


        // starts when the red EXIT button is clicked
        private void ExitButton_Click(object? sender, EventArgs e)
        {
            ConfirmAndExit();
        }

        
        // Shows a goodbye message in the chat, then after a short
        // pause closes the application.       
        // Application.Exit()= closes the entire program
       
        private void ConfirmAndExit()
        {
            // Ask the user to confirm before quitting
            DialogResult result = MessageBox.Show(
                $"Are you sure you want to exit, {_brain.UserName}?\n\nRemember to stay safe online! 🔒",
                "Exit Cybersecurity Bot",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            // Only exit if they clicked Yes
            if (result == DialogResult.Yes)
            {
                // Show a goodbye message in the chat first
                AppendBotMessage(
                    $"Goodbye, {_brain.UserName}! \n\n" +
                    "Thank you for learning about cybersecurity today.\n" +
                    "Remember:\n" +
                    "   Use strong, unique passwords\n" +
                    "   Watch out for phishing scams\n" +
                    "   Enable 2FA on important accounts\n\n" +
                    "Stay safe online! The program will now close."
                );

                // Refresh the chat so the message is visible before closing
                chatDisplay.Refresh();
                System.Threading.Thread.Sleep(1500);  // wait 1.5 seconds so user can read it

                // Close the application
                Application.Exit();
            }
            // If they clicked No, nothing happens - conversation continues
        }

        
        // Adds a user message to the chat display with cyan colour       
        private void AppendUserMessage(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm");

            // Add username in colour
            chatDisplay.SelectionColor = Color.Cyan;
            chatDisplay.AppendText($"\n[{timestamp}] {_brain.UserName}: ");

            // Add message in white
            chatDisplay.SelectionColor = Color.White;
            chatDisplay.AppendText(message + "\n");

            // Scroll to the bottom so user sees the latest message
            chatDisplay.ScrollToCaret();
        }

        
        // Adds a bot message to the chat display 
        private void AppendBotMessage(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm");

            // Add a subtle separator line
            chatDisplay.SelectionColor = Color.FromArgb(40, 60, 40);
            chatDisplay.AppendText("─────────────────────────────────────\n");

            // Bot name in the current sentiment colour
            chatDisplay.SelectionColor = _currentSentimentColor;
            chatDisplay.AppendText($"[{timestamp}]  Bot: ");

            // Message in light green
            chatDisplay.SelectionColor = Color.FromArgb(180, 255, 180);
            chatDisplay.AppendText(message + "\n");

            chatDisplay.ScrollToCaret();
        }

        private void HandleSentimentDetected(string sentiment, string colorHex)
        {
           
            Color sentimentColor = ColorTranslator.FromHtml(colorHex);

            // Store it so future bot messages use this colour
            _currentSentimentColor = sentimentColor;

            // Update the sentiment indicator label at the top
            string emoji = sentiment switch
            {
                "worried" or "scared" or "anxious" => "",
                "frustrated" => "",
                "confused" => "",
                "curious" or "excited" => "",
                "happy" or "good" or "great" => "",
                "terrible" or "bad" or "overwhelmed" => "",
                _ => ""
            };

            sentimentLabel.Text = $"{emoji} Mood detected: {sentiment.ToUpper()}  |  Adjusting response tone...";
            sentimentLabel.ForeColor = sentimentColor;
        }


        // Shows the memory bar when the bot remembers something

       private void UpdateMemoryPanel(string topic)
        {
            memoryLabel.Text  = $" Memory: I remember you're interested in {topic}. I'll personalise my tips for you!";
            memoryPanel.Visible = true;

            
            MainChatForm_Resize(null, EventArgs.Empty);
        }
    }
}
