using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

namespace BG3UnPAKer
{
    public partial class BG3UnPAKer : Form
    {
        // Constants defining file and table headers.
        private const string MAGIC_STRING = "LSPK";
        private const int FILE_VERSION = 18;
        private const int FILE_HDR_SIZE = 24;
        private const int TBL_HDR_SIZE = 8;
        private const int TBL_ENTRY_SIZE = 272;

        // External library imports for decompression functions via P/Invoke
        [DllImport("msys-lz4-1.dll")]
        private static extern int LZ4_decompress_safe(byte[] src, byte[] dst, int compressedSize, int maxDecompressedSize);

        [DllImport("libzstd.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ZSTD_createDCtx();

        [DllImport("libzstd.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD_decompressDCtx(IntPtr context, byte[] dst, UIntPtr maxDecompressedSize, byte[] src, UIntPtr compressedSize);

        [DllImport("libzstd.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ZSTD_freeDCtx(IntPtr context);

        [DllImport("libz.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int uncompress(byte[] dest, ref uint destLen, byte[] source, uint sourceLen);

        private readonly BackgroundWorker extractionWorker;
        private readonly ContextMenu contextMenu;

        public BG3UnPAKer()
        {
            InitializeComponent();
            ArchiveListBox.SelectedIndexChanged += ListBox1_SelectedIndexChanged;
            ArchiveListBox.MouseDown += ListBox1_MouseDown;

            contextMenu = new ContextMenu();
            MenuItem browseMenuItem = new MenuItem("Browse Files", BrowseMenuItem_Click);
            contextMenu.MenuItems.Add(browseMenuItem);

            // Initialize the BackgroundWorker
            extractionWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            extractionWorker.DoWork += ExtractionWorker_DoWork;
            extractionWorker.RunWorkerCompleted += ExtractionWorker_RunWorkerCompleted;
        }

        private void BrowseMenuItem_Click(object sender, EventArgs e)
        {
            string selectedFilePath = Path.Combine(selectedFolderPath, ArchiveListBox.SelectedItem.ToString());
            List<string> fileEntries = ParseArchive(selectedFilePath);
            BrowseFiles browseFilesForm = new BrowseFiles(fileEntries, this);
            browseFilesForm.ShowDialog();
        }

        private List<string> ParseArchive(string archivePath)
        {
            List<string> fileEntries = new List<string>();

            using (var bg3File = new FileStream(archivePath, FileMode.Open, FileAccess.Read))
            {
                byte[] fileHeader = new byte[FILE_HDR_SIZE];
                if (bg3File.Read(fileHeader, 0, FILE_HDR_SIZE) != FILE_HDR_SIZE)
                {
                    LogMessage("Unable to read file header.");
                    return fileEntries;
                }

                if (Encoding.ASCII.GetString(fileHeader, 0, 4) != MAGIC_STRING)
                {
                    LogMessage("Invalid file ID.");
                    return fileEntries;
                }

                uint version = BitConverter.ToUInt32(fileHeader, 4);
                if (version != FILE_VERSION)
                {
                    LogMessage("Unsupported package version.");
                    return fileEntries;
                }

                long tblOff = BitConverter.ToInt64(fileHeader, 8);
                bg3File.Seek(tblOff, SeekOrigin.Begin);
                byte[] tblHeader = new byte[TBL_HDR_SIZE];
                if (bg3File.Read(tblHeader, 0, TBL_HDR_SIZE) != TBL_HDR_SIZE)
                {
                    LogMessage("Unable to read table header.");
                    return fileEntries;
                }

                uint numFiles = BitConverter.ToUInt32(tblHeader, 0);
                uint tblCmpLen = BitConverter.ToUInt32(tblHeader, 4);
                byte[] cmpTblEntries = new byte[tblCmpLen];
                if (bg3File.Read(cmpTblEntries, 0, (int)tblCmpLen) != tblCmpLen)
                {
                    LogMessage("Unable to read table entries.");
                    return fileEntries;
                }

                byte[] tblEntries = UncompressTableEntries(cmpTblEntries, (int)(TBL_ENTRY_SIZE * numFiles));

                for (int i = 0; i < numFiles; i++)
                {
                    int entOff = i * TBL_ENTRY_SIZE;
                    string fName = Encoding.Default.GetString(tblEntries, entOff, 256).TrimEnd('\0');
                    fileEntries.Add(fName);
                }
            }

            return fileEntries;
        }

        private byte[] UncompressTableEntries(byte[] compressedData, int uncompressedSize)
        {
            byte[] decompressedData = new byte[uncompressedSize];
            LZ4_decompress_safe(compressedData, decompressedData, compressedData.Length, uncompressedSize);
            return decompressedData;
        }

        private void ListBox1_MouseDown(object sender, MouseEventArgs e)
        {
            int index = ArchiveListBox.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                Rectangle itemRect = ArchiveListBox.GetItemRectangle(index);
                if (itemRect.Contains(e.Location))
                {
                    ArchiveListBox.SelectedIndex = index;
                    if (e.Button == MouseButtons.Right)
                    {
                        contextMenu.Show(ArchiveListBox, e.Location);
                    }
                }
                else
                {
                    ArchiveListBox.ClearSelected();
                }
            }
            else
            {
                ArchiveListBox.ClearSelected();
            }
        }

        private void CheckDLLFiles()
        {
            string[] dllFiles = { "msys-lz4-1.dll", "libzstd.dll", "libz.dll" };

            foreach (string dllFile in dllFiles)
            {
                if (!File.Exists(dllFile))
                {
                    PathButton.Enabled = false;
                    UnpackButton.Enabled = false;
                    ExtractCheckBox.Enabled = false;
                    LogMessage($"Error: {dllFile} is missing.");
                }
                else
                {
                    UnpackButton.Enabled = false;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckDLLFiles();
        }

        private string selectedFolderPath;

        private void Button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();

            if (FBD.ShowDialog() == DialogResult.OK)
            {
                ArchiveListBox.Items.Clear();
                selectedFolderPath = FBD.SelectedPath; // Store the selected folder path

                // Get both .pak and .lsv files
                string[] pakFiles = Directory.GetFiles(selectedFolderPath, "*.pak");
                string[] lsvFiles = Directory.GetFiles(selectedFolderPath, "*.lsv");

                // Add .pak files to the list box, applying the regex filter
                foreach (string file in pakFiles)
                {
                    string fileName = Path.GetFileName(file);
                    if (!System.Text.RegularExpressions.Regex.IsMatch(fileName, @"_\d+\."))
                    {
                        ArchiveListBox.Items.Add(fileName);
                    }
                }

                // Add .lsv files to the list box without applying the regex filter
                foreach (string file in lsvFiles)
                {
                    string fileName = Path.GetFileName(file);
                    ArchiveListBox.Items.Add(fileName);
                }
            }
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ArchiveListBox.SelectedItem != null)
            {
                string curItem = ArchiveListBox.SelectedItem.ToString();
                UnpackButton.Enabled = true;
                LogMessage(curItem + " ready to extract?");
            }
            else
            {
                UnpackButton.Enabled = false;
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            string selectedFileName = ArchiveListBox.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(selectedFileName))
            {
                string selectedFilePath = Path.Combine(selectedFolderPath, selectedFileName);
                ArchiveListBox.Enabled = false;
                extractionWorker.RunWorkerAsync(selectedFilePath);
                ExtractCancelButton.Enabled = true;
                UnpackButton.Enabled = false;
                PathButton.Enabled = false;
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (extractionWorker.IsBusy)
            {
                extractionWorker.CancelAsync();
                ExtractCancelButton.Enabled = false;
            }
        }

        private void ExtractionWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string archivePath = e.Argument as string;
            try
            {
                ExtractFilesFromArchive(archivePath, extractionWorker, e);
            }
            catch (Exception ex)
            {
                e.Result = ex;
            }
        }

        private void ExtractionWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                LogMessage("Error: " + e.Error.Message);
            }
            else if (e.Cancelled)
            {
                LogMessage("Extraction process was cancelled.");
            }
            else if (e.Result != null && e.Result is Exception)
            {
                Exception ex = e.Result as Exception;
                LogMessage("Error during extraction: " + ex.Message);
            }
            else
            {
                LogMessage("Done" + Environment.NewLine);
                PathButton.Enabled = true;
                ExtractCancelButton.Enabled = false;
            }

            if (ArchiveListBox.InvokeRequired)
            {
                ArchiveListBox.Invoke((MethodInvoker)delegate
                {
                    ArchiveListBox.Enabled = true;
                    if (ArchiveListBox.SelectedItem != null)
                    {
                        ArchiveListBox.ClearSelected();
                    }
                    UnpackButton.Enabled = false;
                    UpdateProgressBarAndLabel(0);
                });
            }
            else
            {
                ArchiveListBox.Enabled = true;
                if (ArchiveListBox.SelectedItem != null)
                {
                    ArchiveListBox.ClearSelected();
                }
                UnpackButton.Enabled = false;
                UpdateProgressBarAndLabel(0);
            }
        }

        private void LogMessage(string message)
        {
            if (LogRichTextBox.InvokeRequired)
            {
                LogRichTextBox.Invoke((MethodInvoker)(() => LogMessage(message)));
                return;
            }

            LogRichTextBox.AppendText(message + Environment.NewLine);
            LogRichTextBox.ScrollToCaret();
        }

        private void ExtractFilesFromArchive(string archivePath, BackgroundWorker worker, DoWorkEventArgs e)
        {
            try
            {
                using (var bg3File = new FileStream(archivePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] fileHeader = new byte[FILE_HDR_SIZE];
                    if (bg3File.Read(fileHeader, 0, FILE_HDR_SIZE) != FILE_HDR_SIZE)
                    {
                        LogMessage("Unable to read file header.");
                        return;
                    }

                    if (Encoding.ASCII.GetString(fileHeader, 0, 4) != MAGIC_STRING)
                    {
                        LogMessage("Invalid file ID.");
                        return;
                    }

                    uint version = BitConverter.ToUInt32(fileHeader, 4);
                    if (version != FILE_VERSION)
                    {
                        LogMessage("Unsupported package version.");
                        return;
                    }

                    long tblOff = BitConverter.ToInt64(fileHeader, 8);
                    bg3File.Seek(tblOff, SeekOrigin.Begin);
                    byte[] tblHeader = new byte[TBL_HDR_SIZE];
                    if (bg3File.Read(tblHeader, 0, TBL_HDR_SIZE) != TBL_HDR_SIZE)
                    {
                        LogMessage("Unable to read table header.");
                        return;
                    }

                    uint numFiles = BitConverter.ToUInt32(tblHeader, 0);
                    LogMessage(string.Format("Package Version: {0}\nExtracting {1} files.", FILE_VERSION, numFiles));

                    uint tblCmpLen = BitConverter.ToUInt32(tblHeader, 4);
                    byte[] cmpTblEntries = new byte[tblCmpLen];
                    if (bg3File.Read(cmpTblEntries, 0, (int)tblCmpLen) != tblCmpLen)
                    {
                        LogMessage("Unable to read table entries.");
                        return;
                    }

                    byte[] tblEntries = UncompressTableEntries(cmpTblEntries, (int)(TBL_ENTRY_SIZE * numFiles));

                    string outputDir;
                    if (ExtractCheckBox.Checked == true)
                    {
                        outputDir = Path.Combine(selectedFolderPath, "unPAKed");
                        Directory.CreateDirectory(outputDir);
                    }
                    else
                    {
                        outputDir = Path.Combine(selectedFolderPath, Path.GetFileNameWithoutExtension(archivePath));
                        Directory.CreateDirectory(outputDir);
                    }

                    Stopwatch stopwatch = Stopwatch.StartNew();
                    int filesExtracted = 0;

                    for (int i = 0; i < numFiles; i++)
                    {
                        if (worker.CancellationPending)
                        {
                            e.Cancel = true;
                            PathButton.Enabled = true;
                            return;
                        }

                        double percentage = (double)i / numFiles * 100;

                        int entOff = i * TBL_ENTRY_SIZE;
                        string fName = Encoding.Default.GetString(tblEntries, entOff, 256).TrimEnd('\0');
                        entOff += 256;
                        long fOfst = BitConverter.ToInt64(tblEntries, entOff) & 0x00ffffffffffffff;

                        if (fOfst == 0xBEEFDEADBEEF)
                        {
                            continue;
                        }

                        uint fcLen = BitConverter.ToUInt32(tblEntries, entOff + 8);
                        uint fLen = BitConverter.ToUInt32(tblEntries, entOff + 12);

                        filesExtracted++;
                        double currentPercentage = (double)filesExtracted / numFiles * 100;
                        UpdateProgressBarAndLabel(currentPercentage);
                        ExtractFileFromArchive(bg3File, fName, fOfst, fcLen, fLen, outputDir, archivePath);
                    }

                    stopwatch.Stop();
                    LogMessage(string.Format("\nExtraction completed successfully in {0:F2} seconds.", stopwatch.Elapsed.TotalSeconds));

                    if (filesExtracted == numFiles)
                    {
                        if (ArchiveListBox.SelectedItem != null)
                        {
                            ArchiveListBox.ClearSelected();
                        }
                        UnpackButton.Enabled = false;
                        UpdateProgressBarAndLabel(0);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage("Error: " + ex.Message);
            }
        }

        private void UpdateProgressBarAndLabel(double currentPercentage)
        {
            if (ExtractProgressBar.InvokeRequired)
            {
                ExtractProgressBar.Invoke((MethodInvoker)(() => ExtractProgressBar.Value = (int)currentPercentage));
            }
            else
            {
                ExtractProgressBar.Value = (int)currentPercentage;
            }

            if (ProgressLabel.InvokeRequired)
            {
                ProgressLabel.Invoke((MethodInvoker)(() => ProgressLabel.Text = $"{(int)currentPercentage}%"));
            }
            else
            {
                ProgressLabel.Text = $"{(int)currentPercentage}%";
            }
        }

        private void ExtractFileFromArchive(FileStream bg3File, string fName, long fOfst, uint fcLen, uint fLen, string outputDir, string archivePath)
        {
            bool isCompressed = fcLen != fLen;
            ExtractFileOrPart(bg3File, fName, fOfst, fcLen, fLen, outputDir, archivePath, isCompressed);
        }

        private void ExtractFileOrPart(FileStream bg3File, string fName, long fOfst, uint fcLen, uint fLen, string outputDir, string archivePath, bool decompress = false)
        {
            long usableOffset = ((ulong)fOfst & 0xFFFF000000000000UL) != 0 ? fOfst & 0x00000000FFFFFFFF : fOfst;

            byte[] fileData;
            if (((ulong)fOfst & 0xFFFF000000000000UL) != 0 && File.Exists(archivePath))
            {
                string partName = Path.GetFileNameWithoutExtension(archivePath) + "_" + (((long)fOfst >> 48) & 0xFF) + Path.GetExtension(archivePath);
                string partPath = Path.Combine(Path.GetDirectoryName(archivePath), partName);

                if (!File.Exists(partPath))
                {
                    LogMessage("Part file does not exist: " + partPath);
                    extractionWorker.CancelAsync();
                    ExtractCancelButton.Enabled = false;
                    return;
                }

                using (var partFile = new FileStream(partPath, FileMode.Open, FileAccess.Read))
                {
                    partFile.Seek(usableOffset, SeekOrigin.Begin);
                    fileData = new byte[fcLen];
                    partFile.Read(fileData, 0, (int)fcLen);
                }
            }
            else
            {
                bg3File.Seek(fOfst, SeekOrigin.Begin);
                fileData = new byte[fcLen];
                bg3File.Read(fileData, 0, (int)fcLen);
            }

            if (decompress && fLen != 0)
                fileData = UncompressData(fileData, (int)fLen, archivePath);

            WriteExtractedFile(fileData, fName, outputDir);
        }

        private void WriteExtractedFile(byte[] fileData, string fName, string outputDir)
        {
            string filePath = Path.Combine(outputDir, fName);
            string directoryPath = Path.GetDirectoryName(filePath);

            if (!string.IsNullOrEmpty(directoryPath))
                Directory.CreateDirectory(directoryPath);

            using (var outFile = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                outFile.Write(fileData, 0, fileData.Length);
            }
        }

        public void ExtractSpecificFile(string filePath)
        {
            // Your logic to extract the specific file
            // Use the filePath to find the specific file in the archive and extract it
            string archivePath = Path.Combine(selectedFolderPath, ArchiveListBox.SelectedItem.ToString());
            using (var bg3File = new FileStream(archivePath, FileMode.Open, FileAccess.Read))
            {
                byte[] fileHeader = new byte[FILE_HDR_SIZE];
                if (bg3File.Read(fileHeader, 0, FILE_HDR_SIZE) != FILE_HDR_SIZE)
                {
                    LogMessage("Unable to read file header.");
                    return;
                }

                if (Encoding.ASCII.GetString(fileHeader, 0, 4) != MAGIC_STRING)
                {
                    LogMessage("Invalid file ID.");
                    return;
                }

                uint version = BitConverter.ToUInt32(fileHeader, 4);
                if (version != FILE_VERSION)
                {
                    LogMessage("Unsupported package version.");
                    return;
                }

                long tblOff = BitConverter.ToInt64(fileHeader, 8);
                bg3File.Seek(tblOff, SeekOrigin.Begin);
                byte[] tblHeader = new byte[TBL_HDR_SIZE];
                if (bg3File.Read(tblHeader, 0, TBL_HDR_SIZE) != TBL_HDR_SIZE)
                {
                    LogMessage("Unable to read table header.");
                    return;
                }

                uint numFiles = BitConverter.ToUInt32(tblHeader, 0);
                uint tblCmpLen = BitConverter.ToUInt32(tblHeader, 4);
                byte[] cmpTblEntries = new byte[tblCmpLen];
                if (bg3File.Read(cmpTblEntries, 0, (int)tblCmpLen) != tblCmpLen)
                {
                    LogMessage("Unable to read table entries.");
                    return;
                }

                byte[] tblEntries = UncompressTableEntries(cmpTblEntries, (int)(TBL_ENTRY_SIZE * numFiles));

                for (int i = 0; i < numFiles; i++)
                {
                    int entOff = i * TBL_ENTRY_SIZE;
                    string fName = Encoding.Default.GetString(tblEntries, entOff, 256).TrimEnd('\0');
                    entOff += 256;
                    long fOfst = BitConverter.ToInt64(tblEntries, entOff) & 0x00ffffffffffffff;

                    if (fName == filePath)
                    {
                        uint fcLen = BitConverter.ToUInt32(tblEntries, entOff + 8);
                        uint fLen = BitConverter.ToUInt32(tblEntries, entOff + 12);

                        string outputDir = Path.Combine(selectedFolderPath, "ExtractedFiles");
                        Directory.CreateDirectory(outputDir);

                        ExtractFileFromArchive(bg3File, fName, fOfst, fcLen, fLen, outputDir, archivePath);
                        LogMessage($"Extracted {fName} to {outputDir}");
                        break;
                    }
                }
            }
        }

        private byte[] UncompressData(byte[] compressedData, int maxDecompressedSize, string fileName)
        {
            if (Path.GetExtension(fileName).Equals(".lsv", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    return DecompressDataZSTD(compressedData, maxDecompressedSize);
                }
                catch
                {
                    return DecompressDataZlib(compressedData, maxDecompressedSize);
                }
            }
            else if (Path.GetExtension(fileName).Equals(".pak", StringComparison.OrdinalIgnoreCase))
            {
                return DecompressDataLZ4(compressedData, maxDecompressedSize);
            }
            else
            {
                throw new ArgumentException("Unsupported file type.");
            }
        }

        private byte[] DecompressDataLZ4(byte[] compressedData, int maxDecompressedSize)
        {
            byte[] decompressedData = new byte[maxDecompressedSize];
            LZ4_decompress_safe(compressedData, decompressedData, compressedData.Length, maxDecompressedSize);
            return decompressedData;
        }

        private byte[] DecompressDataZSTD(byte[] compressedData, int maxDecompressedSize)
        {
            byte[] decompressedData = new byte[maxDecompressedSize];
            IntPtr context = ZSTD_createDCtx();
            UIntPtr decompressedSize = ZSTD_decompressDCtx(context, decompressedData, (UIntPtr)maxDecompressedSize, compressedData, (UIntPtr)compressedData.Length);
            ZSTD_freeDCtx(context);
            Array.Resize(ref decompressedData, (int)decompressedSize.ToUInt32());
            return decompressedData;
        }

        private byte[] DecompressDataZlib(byte[] compressedData, int maxDecompressedSize)
        {
            uint destLen = (uint)maxDecompressedSize;
            byte[] decompressedData = new byte[maxDecompressedSize];
            int result = uncompress(decompressedData, ref destLen, compressedData, (uint)compressedData.Length);
            if (result != 0)
            {
                throw new Exception("Error decompressing zlib data. Error code: " + result);
            }
            byte[] actualDecompressedData = new byte[destLen];
            Array.Copy(decompressedData, actualDecompressedData, destLen);
            return actualDecompressedData;
        }
    }
}
