using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Lab_21
{
    public partial class DocumentForm : Form
    {
        private RichTextBox richTextBox;
        private string currentFilePath = null;
        private ToolStripMenuItem saveMenuItem;
        private ToolStripMenuItem saveAsMenuItem;

        public DocumentForm(string title, ToolStripMenuItem globalSaveMenuItem, ToolStripMenuItem globalSaveAsMenuItem)
        {
            InitializeComponent();
            Text = title;
            this.saveMenuItem = globalSaveMenuItem;
            this.saveAsMenuItem = globalSaveAsMenuItem;

            InitEditor();
            ApplySettings();

            saveMenuItem.Click -= GlobalSaveMenuItem_Click;
            saveMenuItem.Click += GlobalSaveMenuItem_Click;

            saveAsMenuItem.Click -= GlobalSaveAsMenuItem_Click;
            saveAsMenuItem.Click += GlobalSaveAsMenuItem_Click;
        }

        private void GlobalSaveMenuItem_Click(object sender, EventArgs e)
        {
            if (this == ActiveMdiChild)
            {
                SaveFile_Click(sender, e);
            }
        }

        private void GlobalSaveAsMenuItem_Click(object sender, EventArgs e)
        {
            if (this == ActiveMdiChild)
            {
                SaveFileAs_Click(sender, e);
            }
        }

        private void InitEditor()
        {
            richTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                WordWrap = true,
                AllowDrop = true,
                AcceptsTab = true // табуляція
            };
            richTextBox.DragEnter += RichTextBox_DragEnter;
            richTextBox.DragDrop += RichTextBox_DragDrop;
            richTextBox.TextChanged += RichTextBox_TextChanged;
            Controls.Add(richTextBox);

            var contextMenu = new ContextMenuStrip();
            var cutMenuItem = new ToolStripMenuItem("Cut", null, (s, e) => richTextBox.Cut()) { ShortcutKeys = Keys.Control | Keys.X };
            var copyMenuItem = new ToolStripMenuItem("Copy", null, (s, e) => richTextBox.Copy()) { ShortcutKeys = Keys.Control | Keys.C };
            var pasteMenuItem = new ToolStripMenuItem("Paste", null, (s, e) => richTextBox.Paste()) { ShortcutKeys = Keys.Control | Keys.V };
            var fontMenuItem = new ToolStripMenuItem("Font", null, Font_Click);
            var colorMenuItem = new ToolStripMenuItem("Color", null, Color_Click);
            var alignLeftMenuItem = new ToolStripMenuItem("Align Left", null, (s, e) => richTextBox.SelectionAlignment = HorizontalAlignment.Left);
            var alignCenterMenuItem = new ToolStripMenuItem("Align Center", null, (s, e) => richTextBox.SelectionAlignment = HorizontalAlignment.Center);
            var alignRightMenuItem = new ToolStripMenuItem("Align Right", null, (s, e) => richTextBox.SelectionAlignment = HorizontalAlignment.Right);

            contextMenu.Items.AddRange(new ToolStripItem[] { cutMenuItem, copyMenuItem, pasteMenuItem, new ToolStripSeparator(), fontMenuItem, colorMenuItem, new ToolStripSeparator(), alignLeftMenuItem, alignCenterMenuItem, alignRightMenuItem});
            richTextBox.ContextMenuStrip = contextMenu;
        }


        private void RichTextBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0 && IsImageFile(files[0]))
                {
                    e.Effect = DragDropEffects.Copy;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
        }
        private void RichTextBox_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0 && IsImageFile(files[0]))
            {
                try
                {
                    var image = new Bitmap(files[0]);
                    Clipboard.SetImage(image);
                    richTextBox.Paste();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка вставки зображення: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool IsImageFile(string filePath)
        {
            try
            {
                var ext = Path.GetExtension(filePath).ToLower();
                return ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp" || ext == ".gif";
            }
            catch
            {
                return false;
            }
        }

        private void RichTextBox_TextChanged(object sender, EventArgs e)
        {
            saveMenuItem.Enabled = true;
            saveAsMenuItem.Enabled = true;
        }

        public void SaveFile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                SaveFileAs_Click(sender, e);
            }
            else
            {
                richTextBox.SaveFile(currentFilePath);
                Text = Path.GetFileName(currentFilePath);
            }
        }

        public void SaveFileAs_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Rich Text Format (*.rtf)|*.rtf|All Files (*.*)|*.*",
                FileName = string.IsNullOrEmpty(currentFilePath) ? Text + ".rtf" : Path.GetFileName(currentFilePath)
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                currentFilePath = saveFileDialog.FileName;
                richTextBox.SaveFile(currentFilePath);
                Text = Path.GetFileName(currentFilePath);
            }
        }

        public void LoadFile(string filePath)
        {
            currentFilePath = filePath;
            richTextBox.LoadFile(currentFilePath);
            Text = Path.GetFileName(currentFilePath);
        }

        public void Font_Click(object sender, EventArgs e)
        {
            using (var fontDialog = new FontDialog())
            {
                fontDialog.Font = richTextBox.SelectionFont ?? richTextBox.Font;
                if (fontDialog.ShowDialog() == DialogResult.OK)
                {
                    richTextBox.SelectionFont = fontDialog.Font;
                }
            }
        }

        public void Color_Click(object sender, EventArgs e)
        {
            using (var colorDialog = new ColorDialog
            {
                AllowFullOpen = true,
                AnyColor = true
            })
            {
                colorDialog.Color = richTextBox.SelectionColor;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    richTextBox.SelectionColor = colorDialog.Color;
                }
            }
        }

        private void ApplySettings()
        {
            var font = new Font(Settings.FontFamily, Settings.FontSize);
            richTextBox.Font = font;
            saveMenuItem.Enabled = false;
            saveAsMenuItem.Enabled = false;
        }
    }
}