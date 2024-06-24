using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BG3UnPAKer
{
    public partial class BrowseFiles : Form
    {
        private readonly BG3UnPAKer mainForm;
        private readonly List<TreeNode> searchResults;
        private int currentSearchIndex;
        private List<string> fileEntries;

        public BrowseFiles(List<string> fileEntries, BG3UnPAKer mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            this.fileEntries = fileEntries;
            searchResults = new List<TreeNode>();
            currentSearchIndex = -1;

            PopulateTreeView();
        }

        private void PopulateTreeView()
        {
            TreeNode rootNode = new TreeNode("Files");
            treeView1.Nodes.Add(rootNode);
            UpdateFileCount(fileEntries.Count);

            var nodeDict = new Dictionary<string, TreeNode>();

            foreach (var file in fileEntries)
            {
                AddFileToTree(rootNode, file, nodeDict);
            }

            rootNode.ExpandAll();
            UpdateLineCount();
        }

        private void AddFileToTree(TreeNode rootNode, string filePath, Dictionary<string, TreeNode> nodeDict)
        {
            string[] parts = filePath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            TreeNode currentNode = rootNode;
            string currentPath = "";

            foreach (string part in parts)
            {
                currentPath = currentPath == "" ? part : currentPath + "\\" + part;

                if (!nodeDict.ContainsKey(currentPath))
                {
                    TreeNode newNode = new TreeNode(part) { Name = part };
                    nodeDict[currentPath] = newNode;
                    currentNode.Nodes.Add(newNode);
                }
                currentNode = nodeDict[currentPath];
            }
        }

        private void UpdateFileCount(int count)
        {
            ArchiveFilesLabel.Text = $"Files in archive: {count}";
        }

        private void UpdateLineCount()
        {
            int lineCount = CountNodes(treeView1.Nodes);
            FilesShownLabel.Text = $"Files Shown: {lineCount - 1}"; // Subtracting 1 to exclude the root node
        }

        private int CountNodes(TreeNodeCollection nodes)
        {
            int count = 0;
            foreach (TreeNode node in nodes)
            {
                count++;
                count += CountNodes(node.Nodes);
            }
            return count;
        }

        private void BrowseFiles_Load(object sender, EventArgs e)
        {
            // Any additional logic you want to execute when the form loads
        }

        private void ExtractButton_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeView1.SelectedNode;
            if (selectedNode != null && selectedNode.Level > 0)
            {
                // Build the full path of the selected file
                string fullPath = GetFullPath(selectedNode);

                // Temporarily disable the TreeView
                treeView1.Enabled = false;

                // Extract the selected file
                mainForm.ExtractSpecificFile(fullPath);

                // Re-enable the TreeView
                treeView1.Enabled = true;

                // Re-select the node after extraction
                treeView1.SelectedNode = selectedNode;
                selectedNode.EnsureVisible();
                treeView1.Focus();
            }
            else
            {
                MessageBox.Show("Please select a file to extract.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetFullPath(TreeNode node)
        {
            List<string> parts = new List<string>();
            TreeNode currentNode = node;

            while (currentNode != null && currentNode.Text != "Files")
            {
                parts.Insert(0, currentNode.Text);
                currentNode = currentNode.Parent;
            }

            return string.Join("\\", parts);
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            string searchTerm = searchTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchResults.Clear();
                currentSearchIndex = -1;
                FindNodes(treeView1.Nodes, searchTerm);

                if (searchResults.Any())
                {
                    currentSearchIndex = 0;
                    SelectNode(searchResults[currentSearchIndex]);
                }
                else
                {
                    MessageBox.Show("File not found.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void FindNextButton_Click(object sender, EventArgs e)
        {
            if (searchResults.Any() && currentSearchIndex >= 0)
            {
                currentSearchIndex++;
                if (currentSearchIndex >= searchResults.Count)
                {
                    MessageBox.Show("No more results.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    currentSearchIndex = 0; // Resetting to the first result if needed, or adjust logic as per requirement
                }
                SelectNode(searchResults[currentSearchIndex]);
            }
        }

        private void FindNodes(TreeNodeCollection nodes, string searchTerm)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Text.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    searchResults.Add(node);
                }
                FindNodes(node.Nodes, searchTerm);
            }
        }

        private void SelectNode(TreeNode node)
        {
            treeView1.SelectedNode = node;
            node.EnsureVisible();
            treeView1.Focus();
        }
    }
}
