using Avalonia.Controls;
using Avalonia.Media.Imaging;
using FishSenseLiteGUI.SupplementaryClasses;
using FishSenseLiteGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;

namespace FishSenseLiteGUI.Commands
{
    /// <summary>
    /// Purpose: ICommand to open Asynchronously File Explorer and allow user to select a directory.
    /// 
    /// Notes:  Directory should contain images to be displayed for annotations.
    ///         Raises Error if directory contains corrupted images.
    ///         Automatically moves to next page.
    /// </summary>
    public class FileExplorerCommand : ICommand
    {
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public async void Execute(object? parameter)
        {
            // Dialog that allows user to open folder
            var openFolderDialog = new OpenFolderDialog();
            var result = await openFolderDialog.ShowAsync(new Window());

            // Stores if the program should allow the user to access page 2, which can only occur if valid images are added.
            bool imageAdded = false;
            Bitmap testImage;
            if (!String.IsNullOrEmpty(result))
            {
                // Initializes model layer. If model layer already exists, resets model layer for the most recently chosen directory.
                this.source.DirectoryChosen(result);

                List<string> paths = new List<string>(Directory.GetFiles(result));
                paths.Sort();

                List<string> AcceptableExtensions = new List<string> { ".png", ".jpeg", ".jpg", ".bmp", ".gif"};
                foreach ( string path in paths )
                {
                    if ( AcceptableExtensions.Contains(Path.GetExtension(path)) )
                    {
                        try
                        {
                            // If a bitmap can be created, assume image is not corrupted.
                            testImage = new Bitmap(path);
                            source.AddImage(path);
                            imageAdded = true;
                        }
                        catch 
                        {
                            ErrorMessageBox.Show("Attempted to add a corrupted file named " + path);
                        }
                    } 
                }

                if (imageAdded)
                {
                    source.SelectedTabIndex = 1;
                    source.SecondPageEnabled = true;
                } 
                else
                {
                    ErrorMessageBox.Show("No images added. This is the case if (a) the directory chosen had no images within it, or (b)" +
                        "the images within the directory were all incompatible with the program. Please ensure the images are of the types" +
                        " .png, .jpg, .jpeg, .bmp, or .gif.");
                }
            }
        }

        private MainWindowViewModel source;

        public event EventHandler? CanExecuteChanged;

        public FileExplorerCommand(MainWindowViewModel source)
        {
            this.source = source;
        }
    }
}
