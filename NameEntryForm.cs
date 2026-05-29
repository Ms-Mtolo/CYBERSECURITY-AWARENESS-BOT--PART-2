using System;
using System.Drawing;
using System.Windows.Forms;
using System.Media;
using System.IO;

namespace CybersecurityChatbotGUI
{
    public class NameEntryForm : Form
    {
        
        // CONTROLS - these are the visual elements on the form
        // We declare them here so all methods can access them
       
        private Panel headerPanel;       // dark top section
        private Label asciiArtLabel;     // shows the logo
        private Label subtitleLabel;     // "Stay Safe Online!"
        private Label instructionLabel;  // "Please enter your name"
        private TextBox nameTextBox;       // text input field
        private Button startButton;       // "Start Chatting!" button
        private Label errorLabel;        // shows validation error

        
        // CONSTRUCTOR - runs when the form is first created
        
        public NameEntryForm()
        {
            // Set up the window itself
            SetupWindow();

            // Add all the visual controls
            BuildUI();

            // Wire up the Load event - fires AFTER the window is fully visible
            // This ensures the greeting plays while the user can already see the screen
            this.Load += NameEntryForm_Load;
        }

        
        // METHOD: SetupWindow
        // Configures the form/window properties
        
        private void SetupWindow()
        {
            this.Text = "Cybersecurity Awareness Bot";
             this.Size = new Size(680, 600);
              this.StartPosition = FormStartPosition.CenterScreen;
               this.BackColor = Color.FromArgb(13, 17, 23);   // very dark background
                this.FormBorderStyle = FormBorderStyle.FixedSingle;   // prevent resizing
                 this.MaximizeBox = false;
                  this.Font = new Font("Consolas", 9f);
        }

        
        // METHOD: BuildUI
        // Creates and positions every control on the form
        
        private void BuildUI()
        {
            // --- HEADER PANEL (dark green top section) ---
            headerPanel = new Panel();
             headerPanel.BackColor = Color.FromArgb(0, 40, 20);
              headerPanel.Size = new Size(680, 280);
               headerPanel.Location = new Point(0, 0);

            // --- ASCII ART LOGO inside the header ---
             asciiArtLabel = new Label();
              asciiArtLabel.Text =
                "  ╔═══════════════════════════════════════════════════╗\n" +
                "  ║        CYBERSECURITY AWARENESS BOT               ║\n" +
                "  ╠═══════════════════════════════════════════════════╣\n" +
                "  ║  ██████╗██╗   ██╗██████╗ ███████╗██████╗        ║\n" +
                "  ║ ██╔════╝╚██╗ ██╔╝██╔══██╗██╔════╝██╔══██╗       ║\n" +
                "  ║ ██║      ╚████╔╝ ██████╔╝█████╗  ██████╔╝       ║\n" +
                "  ║ ██║       ╚██╔╝  ██╔══██╗██╔══╝  ██╔══██╗       ║\n" +
                "  ║ ╚██████╗   ██║   ██████╔╝███████╗██║  ██║       ║\n" +
                "  ║  ╚═════╝   ╚═╝   ╚═════╝ ╚══════╝╚═╝  ╚═╝       ║\n" +
                "  ╚═══════════════════════════════════════════════════╝";
            asciiArtLabel.Font = new Font("Consolas", 8.5f, FontStyle.Bold);
             asciiArtLabel.ForeColor = Color.LimeGreen;
              asciiArtLabel.BackColor = Color.Transparent;
               asciiArtLabel.AutoSize = false;
                asciiArtLabel.Size = new Size(660, 220);
                 asciiArtLabel.Location = new Point(10, 10);
                  asciiArtLabel.TextAlign = ContentAlignment.MiddleCenter;

            //subtitle label
subtitleLabel = new Label();
 subtitleLabel.Text = "🔒  STAY SAFE ONLINE  🔒";
  subtitleLabel.Font = new Font("Consolas", 12f, FontStyle.Bold);
   subtitleLabel.ForeColor = Color.Cyan;
    subtitleLabel.BackColor = Color.Transparent;
     subtitleLabel.AutoSize = false;
      subtitleLabel.Size = new Size(660, 30);
       subtitleLabel.Location = new Point(0, 235);
        subtitleLabel.TextAlign = ContentAlignment.MiddleCenter;

            // Added controls to the header panel
            headerPanel.Controls.Add(asciiArtLabel);
            headerPanel.Controls.Add(subtitleLabel);

// instruction label
instructionLabel = new Label();
 instructionLabel.Text = "Please enter your name to begin:";
  instructionLabel.Font = new Font("Consolas", 11f);
   instructionLabel.ForeColor = Color.White;
    instructionLabel.AutoSize = false;
     instructionLabel.Size = new Size(500, 30);
      instructionLabel.Location = new Point(90, 310);

// name text label
nameTextBox = new TextBox();
 nameTextBox.Font = new Font("Consolas", 14f);
  nameTextBox.BackColor = Color.FromArgb(30, 40, 50);
   nameTextBox.ForeColor = Color.LimeGreen;
    nameTextBox.BorderStyle = BorderStyle.FixedSingle;
     nameTextBox.Size = new Size(380, 40);
      nameTextBox.Location = new Point(90, 350);
       nameTextBox.PlaceholderText = "Type your name here...";
            // When user presses Enter in the text box, it's the same as clicking Start
            nameTextBox.KeyPress += NameTextBox_KeyPress;

// error label will appear when the user does not type their name
errorLabel = new Label();
 errorLabel.Text = " Please enter your name before continuing.";
  errorLabel.ForeColor = Color.OrangeRed;
   errorLabel.Font = new Font("Consolas", 9f);
    errorLabel.AutoSize = false;
     errorLabel.Size = new Size(460, 25);
      errorLabel.Location = new Point(90, 395);
       errorLabel.Visible = false;  // hidden until needed

            // START BUTTON 
startButton = new Button();
 startButton.Text = " START CHATTING";
  startButton.Font = new Font("Consolas", 12f, FontStyle.Bold);
   startButton.BackColor = Color.FromArgb(0, 100, 50);
    startButton.ForeColor = Color.White;
     startButton.FlatStyle = FlatStyle.Flat;
      startButton.Size = new Size(380, 50);
       startButton.Location = new Point(90, 430);
        startButton.Cursor = Cursors.Hand;
         startButton.FlatAppearance.BorderColor = Color.LimeGreen;
            // Wire up the click event to our method
            startButton.Click += StartButton_Click;

            // VERSION LABEL 
var versionLabel = new Label();
 versionLabel.Text = "Cybersecurity Awareness Bot  •  Part 2 GUI Edition";
  versionLabel.ForeColor = Color.DimGray;
   versionLabel.Font = new Font("Consolas", 8f);
    versionLabel.AutoSize = false;
     versionLabel.Size = new Size(660, 20);
      versionLabel.Location = new Point(0, 555);
       versionLabel.TextAlign = ContentAlignment.MiddleCenter;

            // add all the labels and texts
this.Controls.Add(headerPanel);
 this.Controls.Add(instructionLabel);
  this.Controls.Add(nameTextBox);
   this.Controls.Add(errorLabel);
    this.Controls.Add(startButton);
     this.Controls.Add(versionLabel);
        }

       
        
        // starts when user clicks the "Start Chatting" button
        
        private void StartButton_Click(object? sender, EventArgs e)
        {
            string name = nameTextBox.Text.Trim();

            // Validate that name is not empty
            if (string.IsNullOrWhiteSpace(name))
            {
                errorLabel.Visible = true;
                nameTextBox.Focus();
                return;
            }

            // Capitalise first letter (like Part 1)
            name = char.ToUpper(name[0]) + name.Substring(1).ToLower();

            // Open the main chat window and pass the name
            MainChatForm chatForm = new MainChatForm(name);
            chatForm.Show();

            // Close this name entry form
            this.Hide();

            // When the chat form closes, also close this app
            chatForm.FormClosed += (s, args) => this.Close();
        }

       
        // EVENT: NameTextBox_KeyPress
        // Fires every time user presses a key in the text box
        // If they press Enter, trigger the start button
        
        private void NameTextBox_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                StartButton_Click(sender, EventArgs.Empty);
            }
        }

        
        // EVENT: NameEntryForm_Load
        // Fires automatically once the window is fully visible on screen.
        // We use this instead of the constructor so the user sees the
        // window FIRST, then hears the greeting — not the other way around.
      
        private void NameEntryForm_Load(object? sender, EventArgs e)
        {
            PlayVoiceGreeting();
        }

        // Plays Greetings.wav as a welcome greeting.
     
        private void PlayVoiceGreeting()
        {
            try
            {
                // Build the full path to the wav file
                string exeFolder = AppDomain.CurrentDomain.BaseDirectory;
                string audioFilePath = Path.Combine(exeFolder, "Greetings.wav");

                if (File.Exists(audioFilePath))
                {
                    SoundPlayer player = new SoundPlayer(audioFilePath);
                    player.Load();   // load into memory for clean playback
                    player.Play();   // play in background - window stays active
                }
                // If file is missing, silently skip - program still runs fine
            }
            catch
            {
                // Never crash the program because of a missing sound file
            }
        }
    }
}
