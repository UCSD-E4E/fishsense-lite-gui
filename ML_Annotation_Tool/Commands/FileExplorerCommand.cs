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
    public class FileExplorerCommand : ICommand
    {
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public async void Execute(object? parameter)
        {
            var openFolderDialog = new OpenFolderDialog();
            var result = await openFolderDialog.ShowAsync(new Window());

            if (result != null)
            {
                List<string> paths = new List<string>(Directory.GetFiles(result));
                // Should probably double check if .webp is supported by Avalonia
                List<string> AcceptableExtensions = new List<string> { ".png", ".jpeg", ".jpg", ".bmp", ".gif", ".webp"};
                string badExts = "";
                foreach ( string path in paths )
                {
                    if ( AcceptableExtensions.Contains(Path.GetExtension(path)) )
                    {
                        try
                        {
                            var first_image = new Bitmap(path);
                            source.fileNames.Add(path);
                            source.NumImages += 1;
                        }
                        catch (Exception ex )
                        {
                            var w = new ErrorMessageBox("Attempted to add a corrupted file named " + path);
                        }
                    } 
                }
                var k = new ErrorMessageBox(badExts);

                if (source.fileNames.Count > 0)
                {
                    source.ImageToShow = new Bitmap(source.fileNames[0]);
                    source.selectedTabIndex += 1;
                    source.selectedTabIndex %= 3;
                    source.secondPageEnabled = true;
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
