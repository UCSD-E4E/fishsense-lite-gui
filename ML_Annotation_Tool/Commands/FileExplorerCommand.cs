using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using ML_Annotation_Tool.Models;
using ML_Annotation_Tool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ML_Annotation_Tool.Commands
{
    public class FileExplorerCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public async void Execute(object? parameter)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.AllowMultiple = true; // for now, force them to choose all files in a directory rather than only
            openFileDialog.Filters.Add(new FileDialogFilter() { Name = "Images", Extensions = { "png", "jpeg", "tiff", "bmp", "gif" } });
            var result = await openFileDialog.ShowAsync(new Window());

            bool imageAdded = true;
            if (result != null)
            {
                for (int i = 0; i < result.Length; i++)
                {
                    if (result[i] != null)
                    {
                        source.fileNames.Add(result[i]);
                        source.NumImages += 1;
                        try
                        {
                            var k = new Bitmap(result[i]);
                        } catch (Exception ex)
                        {
                            var w = new ErrorMessageBox("Attempted to add corrupted image named " + result[i]);
                            source.fileNames.Clear();
                            source.NumImages = 0;
                            imageAdded = false;
                            break;
                        }
                    } 
                }
                if (imageAdded)
                {
                    source.ImageToShow = new Bitmap(result[0]);
                    source.selectedTabIndex += 1;
                    source.selectedTabIndex %= 3;
                    source.secondPageEnabled = true;
                } 
            }
        }

        private MainWindowViewModel source;
        public FileExplorerCommand(MainWindowViewModel source)
        {
            this.source = source;
        }
    }
}
