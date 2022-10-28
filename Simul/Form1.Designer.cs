
namespace PLCSimulation
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.file_Menu = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_New = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Open = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Save = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_SaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Close = new System.Windows.Forms.ToolStripMenuItem();
            this.window_Menu = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.file_Menu,
            this.window_Menu});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1570, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // file_Menu
            // 
            this.file_Menu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_New,
            this.menu_Open,
            this.menu_Save,
            this.menu_SaveAs,
            this.menu_Close});
            this.file_Menu.Name = "file_Menu";
            this.file_Menu.Size = new System.Drawing.Size(37, 20);
            this.file_Menu.Text = "&File";
            // 
            // menu_New
            // 
            this.menu_New.Name = "menu_New";
            this.menu_New.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.menu_New.Size = new System.Drawing.Size(178, 22);
            this.menu_New.Text = "&New";
            this.menu_New.Click += new System.EventHandler(this.menu_New_Click);
            // 
            // menu_Open
            // 
            this.menu_Open.Name = "menu_Open";
            this.menu_Open.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menu_Open.Size = new System.Drawing.Size(178, 22);
            this.menu_Open.Text = "&Open";
            this.menu_Open.Click += new System.EventHandler(this.menu_Open_Click);
            // 
            // menu_Save
            // 
            this.menu_Save.Name = "menu_Save";
            this.menu_Save.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.menu_Save.Size = new System.Drawing.Size(178, 22);
            this.menu_Save.Text = "&Save";
            this.menu_Save.Click += new System.EventHandler(this.menu_Save_Click);
            // 
            // menu_SaveAs
            // 
            this.menu_SaveAs.Name = "menu_SaveAs";
            this.menu_SaveAs.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.S)));
            this.menu_SaveAs.Size = new System.Drawing.Size(178, 22);
            this.menu_SaveAs.Text = "Save &as";
            this.menu_SaveAs.Click += new System.EventHandler(this.menu_SaveAs_Click);
            // 
            // menu_Close
            // 
            this.menu_Close.Name = "menu_Close";
            this.menu_Close.Size = new System.Drawing.Size(178, 22);
            this.menu_Close.Text = "&Close";
            this.menu_Close.Click += new System.EventHandler(this.menu_Close_Click);
            // 
            // window_Menu
            // 
            this.window_Menu.Name = "window_Menu";
            this.window_Menu.Size = new System.Drawing.Size(63, 20);
            this.window_Menu.Text = "&WIndow";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1570, 842);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "PLC_Simulator";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem file_Menu;
        private System.Windows.Forms.ToolStripMenuItem menu_New;
        private System.Windows.Forms.ToolStripMenuItem window_Menu;
        private System.Windows.Forms.ToolStripMenuItem menu_Save;
        private System.Windows.Forms.ToolStripMenuItem menu_SaveAs;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolStripMenuItem menu_Open;
        private System.Windows.Forms.ToolStripMenuItem menu_Close;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
    }
}

