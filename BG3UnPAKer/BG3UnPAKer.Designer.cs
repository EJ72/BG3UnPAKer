using System.Windows.Forms;
using System;

namespace BG3UnPAKer
{
    partial class BG3UnPAKer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BG3UnPAKer));
            this.ExtractProgressBar = new System.Windows.Forms.ProgressBar();
            this.PathButton = new System.Windows.Forms.Button();
            this.ArchiveListBox = new System.Windows.Forms.ListBox();
            this.UnpackButton = new System.Windows.Forms.Button();
            this.LogRichTextBox = new System.Windows.Forms.RichTextBox();
            this.ProgressLabel = new System.Windows.Forms.Label();
            this.ExtractCancelButton = new System.Windows.Forms.Button();
            this.ExtractCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // ExtractProgressBar
            // 
            resources.ApplyResources(this.ExtractProgressBar, "ExtractProgressBar");
            this.ExtractProgressBar.Name = "ExtractProgressBar";
            // 
            // PathButton
            // 
            resources.ApplyResources(this.PathButton, "PathButton");
            this.PathButton.Name = "PathButton";
            this.PathButton.UseVisualStyleBackColor = true;
            this.PathButton.Click += new System.EventHandler(this.Button1_Click);
            // 
            // ArchiveListBox
            // 
            this.ArchiveListBox.FormattingEnabled = true;
            resources.ApplyResources(this.ArchiveListBox, "ArchiveListBox");
            this.ArchiveListBox.Name = "ArchiveListBox";
            // 
            // UnpackButton
            // 
            resources.ApplyResources(this.UnpackButton, "UnpackButton");
            this.UnpackButton.Name = "UnpackButton";
            this.UnpackButton.UseVisualStyleBackColor = true;
            this.UnpackButton.Click += new System.EventHandler(this.Button2_Click);
            // 
            // LogRichTextBox
            // 
            this.LogRichTextBox.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.LogRichTextBox, "LogRichTextBox");
            this.LogRichTextBox.Name = "LogRichTextBox";
            this.LogRichTextBox.ReadOnly = true;
            // 
            // ProgressLabel
            // 
            resources.ApplyResources(this.ProgressLabel, "ProgressLabel");
            this.ProgressLabel.Name = "ProgressLabel";
            // 
            // ExtractCancelButton
            // 
            resources.ApplyResources(this.ExtractCancelButton, "ExtractCancelButton");
            this.ExtractCancelButton.Name = "ExtractCancelButton";
            this.ExtractCancelButton.UseVisualStyleBackColor = true;
            this.ExtractCancelButton.Click += new System.EventHandler(this.Button3_Click);
            // 
            // ExtractCheckBox
            // 
            resources.ApplyResources(this.ExtractCheckBox, "ExtractCheckBox");
            this.ExtractCheckBox.Name = "ExtractCheckBox";
            this.ExtractCheckBox.UseVisualStyleBackColor = true;
            // 
            // BG3UnPAKer
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.ExtractCheckBox);
            this.Controls.Add(this.ExtractCancelButton);
            this.Controls.Add(this.ProgressLabel);
            this.Controls.Add(this.LogRichTextBox);
            this.Controls.Add(this.UnpackButton);
            this.Controls.Add(this.PathButton);
            this.Controls.Add(this.ArchiveListBox);
            this.Controls.Add(this.ExtractProgressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "BG3UnPAKer";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ProgressBar ExtractProgressBar;
        private System.Windows.Forms.Button PathButton;
        private Button UnpackButton;
        private ListBox ArchiveListBox;
        private RichTextBox LogRichTextBox;
        private Label ProgressLabel;
        private Button ExtractCancelButton;
        private CheckBox ExtractCheckBox;
    }
}

