# BG3UnPAKer Application

## Overview
BG3UnPAKer is a Windows Forms application designed to browse and extract files from `.pak` and `.lsv` archive files used in the game Baldur's Gate 3. The application provides a graphical interface to navigate the contents of these archives, extract individual files, and perform searches within the file tree.

## Features
- **Browse Archives**: Load and display the contents of `.pak` and `.lsv` files in a tree view structure.
- **Extract Files**: Extract individual files from the archive to a specified directory.
- **Search Functionality**: Search for files within the archive and navigate through search results.
- **File Type Support**: Supports decompression of files using LZ4, ZSTD, and zlib algorithms.

## Getting Started

### Prerequisites
- The application requires the following DLLs to be present in the application's directory:
  - `msys-lz4-1.dll`
  - `libzstd.dll`
  - `libz.dll`

### Installation
1. Download the BG3UnPAKer application and ensure the required DLL files are placed in the same directory as the executable.
2. Run the `BG3UnPAKer.exe` application.

### Usage

#### Loading an Archive
1. Click the "Path" button to open a folder browser dialog.
2. Select the folder containing the `.pak` and/or `.lsv` files.
3. The application will list the archive files in the selected folder. Select an archive from the list to load its contents.

#### Browsing Files
- Right-Click on an archive and the contents of the selected archive will be displayed in a tree view. Expand the nodes to navigate through the directories and files.

#### Extracting Files
1. Select a file in the tree view.
2. Click the "Extract" button to extract the selected file to the specified output directory.

#### Searching for Files
1. Enter a search term in the search text box.
2. Click the "Search" button to find all occurrences of the search term in the file names.
3. Use the "Find Next" button to navigate through the search results.

### Error Handling
- The application will display error messages in pop-up windows if required DLLs are missing or if other issues occur during file extraction or decompression.

### Advanced Options
- **Background Extraction**: The extraction process runs in a background worker to ensure the UI remains responsive. The progress of the extraction is displayed in a progress bar.

## Code Structure

### `BrowseFiles` Form
- **Purpose**: Manages the file browsing and search functionality.
- **Key Methods**:
  - `PopulateTreeView()`: Loads the file entries into the tree view.
  - `AddFileToTree()`: Adds individual files to the tree structure.
  - `SearchButton_Click()`: Handles the search operation.
  - `ExtractButton_Click()`: Extracts the selected file.

### `BG3UnPAKer` Form
- **Purpose**: Manages the main application window and archive extraction process.
- **Key Methods**:
  - `ParseArchive()`: Parses the selected archive to extract file entries.
  - `ExtractionWorker_DoWork()`: Handles the background extraction process.
  - `ExtractFileOrPart()`: Extracts files or parts of files from the archive.
  - `UncompressData()`: Decompresses the file data using the appropriate algorithm.

### Decompression Algorithms
- **LZ4**: `LZ4_decompress_safe()`
- **ZSTD**: `ZSTD_decompressDCtx()`
- **zlib**: `uncompress()`

## Conclusion
BG3UnPAKer is a user-friendly tool for managing and extracting files from Baldur's Gate 3 archives. With its intuitive interface and robust search and extraction capabilities, it simplifies the process of handling game data.
