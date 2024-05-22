using System;
using System.IO;
using System.Windows.Forms;

namespace Lab_21
{
    public partial class Form1 : Form
    {
        private int documentCount = 1;
        private ToolStripMenuItem saveMenuItem;
        private ToolStripMenuItem saveAsMenuItem;

        public Form1()
        {
            InitializeComponent();
            InitMenu();
            InitStatusBar();
            IsMdiContainer = true;
        }

        private void InitMenu()
        {
            var menuStrip = new MenuStrip();

            var fileMenu = new ToolStripMenuItem("File");
            var newMenuItem = new ToolStripMenuItem("New", null, NewFile_Click) { ShortcutKeys = Keys.Control | Keys.N };
            var openMenuItem = new ToolStripMenuItem("Open", null, OpenFile_Click) { ShortcutKeys = Keys.Control | Keys.O };
            saveMenuItem = new ToolStripMenuItem("Save", null, SaveFile_Click) { ShortcutKeys = Keys.Control | Keys.S };
            saveAsMenuItem = new ToolStripMenuItem("Save As", null, SaveFileAs_Click);
            var settingsMenuItem = new ToolStripMenuItem("Settings", null, Settings_Click) { ShortcutKeys = Keys.Control | Keys.P };
            var exitMenuItem = new ToolStripMenuItem("Exit", null, Exit_Click) { ShortcutKeys = Keys.Alt | Keys.F4 };

            fileMenu.DropDownItems.AddRange(new ToolStripItem[] { newMenuItem, openMenuItem, saveMenuItem, saveAsMenuItem, settingsMenuItem, exitMenuItem });
            menuStrip.Items.Add(fileMenu);

            var formatMenu = new ToolStripMenuItem("Format");
            var fontMenuItem = new ToolStripMenuItem("Font", null, Font_Click);
            var colorMenuItem = new ToolStripMenuItem("Color", null, Color_Click);
            formatMenu.DropDownItems.AddRange(new ToolStripItem[] { fontMenuItem, colorMenuItem });
            menuStrip.Items.Add(formatMenu);

            var helpMenu = new ToolStripMenuItem("Help");
            var aboutMenuItem = new ToolStripMenuItem("About", null, About_Click);
            helpMenu.DropDownItems.Add(aboutMenuItem);
            menuStrip.Items.Add(helpMenu);

            Controls.Add(menuStrip);
            MainMenuStrip = menuStrip;

            saveMenuItem.Enabled = false;
            saveAsMenuItem.Enabled = false;
        }

        private void InitStatusBar()
        {
            var statusBar = new StatusStrip();
            var statusLabel = new ToolStripStatusLabel("Ready");
            statusBar.Items.Add(statusLabel);
            Controls.Add(statusBar);
        }

        private void NewFile_Click(object sender, EventArgs e)
        {
            var newDocument = new DocumentForm($"Untitled-{documentCount++}", saveMenuItem, saveAsMenuItem)
            {
                MdiParent = this
            };
            newDocument.Show();
            UpdateSaveMenuItems(true);
        }

        private void OpenFile_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Rich Text Format (*.rtf)|*.rtf|All Files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var newDocument = new DocumentForm(Path.GetFileName(openFileDialog.FileName), saveMenuItem, saveAsMenuItem)
                {
                    MdiParent = this
                };
                newDocument.Show();
                newDocument.LoadFile(openFileDialog.FileName);
                UpdateSaveMenuItems(true);
            }
        }

        private void SaveFile_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild is DocumentForm activeDocument)
            {
                activeDocument.SaveFile_Click(sender, e);
            }
        }

        private void SaveFileAs_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild is DocumentForm activeDocument)
            {
                activeDocument.SaveFileAs_Click(sender, e);
            }
        }

        private void Font_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild is DocumentForm activeDocument)
            {
                activeDocument.Font_Click(sender, e);
            }
        }

        private void Color_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild is DocumentForm activeDocument)
            {
                activeDocument.Color_Click(sender, e);
            }
        }

        private void UpdateSaveMenuItems(bool enabled)
        {
            saveMenuItem.Enabled = enabled;
            saveAsMenuItem.Enabled = enabled;
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            var settingsForm = new SettingsForm();
            settingsForm.ShowDialog();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void About_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Multi-Window Text Editor\nVersion 1.0.1\nNUPP2024", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}