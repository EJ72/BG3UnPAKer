namespace BG3UnPAKer
{
    partial class BrowseFiles
    {
        private System.Windows.Forms.Label ArchiveFilesLabel;
        private System.Windows.Forms.Label FilesShownLabel; // New label for line count
        private System.Windows.Forms.TextBox searchTextBox;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.Button findNextButton;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BrowseFiles));
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.ExtractButton = new System.Windows.Forms.Button();
            this.ArchiveFilesLabel = new System.Windows.Forms.Label();
            this.FilesShownLabel = new System.Windows.Forms.Label();
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.searchButton = new System.Windows.Forms.Button();
            this.findNextButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.Location = new System.Drawing.Point(12, 41);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(824, 397);
            this.treeView1.TabIndex = 0;
            // 
            // ExtractButton
            // 
            this.ExtractButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ExtractButton.Location = new System.Drawing.Point(761, 444);
            this.ExtractButton.Name = "ExtractButton";
            this.ExtractButton.Size = new System.Drawing.Size(75, 23);
            this.ExtractButton.TabIndex = 1;
            this.ExtractButton.Text = "Extract";
            this.ExtractButton.UseVisualStyleBackColor = true;
            this.ExtractButton.Click += new System.EventHandler(this.ExtractButton_Click);
            // 
            // ArchiveFilesLabel
            // 
            this.ArchiveFilesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ArchiveFilesLabel.AutoSize = true;
            this.ArchiveFilesLabel.Location = new System.Drawing.Point(12, 449);
            this.ArchiveFilesLabel.Name = "ArchiveFilesLabel";
            this.ArchiveFilesLabel.Size = new System.Drawing.Size(79, 13);
            this.ArchiveFilesLabel.TabIndex = 2;
            this.ArchiveFilesLabel.Text = "Archive Files: 0";
            // 
            // FilesShownLabel
            // 
            this.FilesShownLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.FilesShownLabel.AutoSize = true;
            this.FilesShownLabel.Location = new System.Drawing.Point(173, 449);
            this.FilesShownLabel.Name = "FilesShownLabel";
            this.FilesShownLabel.Size = new System.Drawing.Size(109, 13);
            this.FilesShownLabel.TabIndex = 6;
            this.FilesShownLabel.Text = "Actual Files Shown: 0";
            // 
            // searchTextBox
            // 
            this.searchTextBox.Location = new System.Drawing.Point(12, 12);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(270, 20);
            this.searchTextBox.TabIndex = 3;
            // 
            // searchButton
            // 
            this.searchButton.Location = new System.Drawing.Point(288, 10);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(75, 23);
            this.searchButton.TabIndex = 4;
            this.searchButton.Text = "Search";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // findNextButton
            // 
            this.findNextButton.Location = new System.Drawing.Point(370, 10);
            this.findNextButton.Name = "findNextButton";
            this.findNextButton.Size = new System.Drawing.Size(75, 23);
            this.findNextButton.TabIndex = 5;
            this.findNextButton.Text = "Find Next";
            this.findNextButton.UseVisualStyleBackColor = true;
            this.findNextButton.Click += new System.EventHandler(this.FindNextButton_Click);
            // 
            // BrowseFiles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(848, 472);
            this.Controls.Add(this.FilesShownLabel);
            this.Controls.Add(this.findNextButton);
            this.Controls.Add(this.searchButton);
            this.Controls.Add(this.searchTextBox);
            this.Controls.Add(this.ArchiveFilesLabel);
            this.Controls.Add(this.ExtractButton);
            this.Controls.Add(this.treeView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BrowseFiles";
            this.Text = "Browse Files";
            this.Load += new System.EventHandler(this.BrowseFiles_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button ExtractButton;
    }
}
