using System;
using System.Windows.Forms;

namespace Lab_21
{
    public partial class SettingsForm : Form
    {
        private ComboBox comboLanguage;

        public SettingsForm()
        {
            InitializeComponent();
            InitComponents();
            LoadSettings();
        }

        private void InitComponents()
        {
            var labelLanguage = new Label
            {
                Text = "Language",
                AutoSize = true,
                Top = 20,
                Left = 20
            };
            comboLanguage = new ComboBox
            {
                Top = 20,
                Left = 100,
                Width = 150
            };
            comboLanguage.Items.AddRange(new[] { "English", "Add more languages by your own" });

            var saveButton = new Button
            {
                Text = "Save",
                Top = 60,
                Left = 20
            };
            saveButton.Click += SaveSettings;

            Controls.AddRange(new Control[] { labelLanguage, comboLanguage, saveButton });
        }

        private void LoadSettings()
        {
            comboLanguage.SelectedItem = Settings.Language;
        }

        private void SaveSettings(object sender, EventArgs e)
        {
            Settings.Language = comboLanguage.SelectedItem?.ToString() ?? "English";
            Close();
        }
    }
}