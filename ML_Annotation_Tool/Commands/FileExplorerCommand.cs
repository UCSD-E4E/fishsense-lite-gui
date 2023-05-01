using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using ML_Annotation_Tool.Models;
using ML_Annotation_Tool.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace ML_Annotation_Tool.Commands
{
    /* Command to open file explorer to choose directory that contains images.
     * Checks to make sure images added aren't corrupted, and makes sure the image can make a Bitmap
     * Otherwise, notifies user that the image is corrupted.
     * Moves user to next page. 
     */
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

            // Boolean used to check whether program should move on to the next page, or force the user
            // to choose another folder that actually contains valid images.
            bool imageAdded = false;
            Bitmap testImage;
            if (result != null)
            {
                // Clear previous images. There can only be one database used at a time.
                if (source.accessor != null)
                {
                    source.accessor.ClearImages();
                }

                // Open connetion to SQLite database using the ViewModel's method.
                source.InitializeConnection(result);

                List<string> paths = new List<string>(Directory.GetFiles(result));
                // Should probably double check if .webp is supported by Avalonia
                List<string> AcceptableExtensions = new List<string> { ".png", ".jpeg", ".jpg", ".bmp", ".gif", ".webp"};
                
                foreach ( string path in paths )
                {
                    if ( AcceptableExtensions.Contains(Path.GetExtension(path)) )
                    {
                        try
                        {
                            // Tests whether the image can be turned into a bitmap. If this works,
                            // the code moving forward assumes the image is a valid one and can be added.
                            testImage = new Bitmap(path);
                            source.AddFileName(path);
                            imageAdded = true;
                        }
                        catch (Exception ex )
                        {
                            var w = new ErrorMessageBox("Attempted to add a corrupted file named " + path);
                        }
                    } 
                }

                if (imageAdded)
                {
                    // Moves to next page, and enables next page.
                    source.SelectedTabIndex += 1;
                    source.SecondPageEnabled = true;
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
