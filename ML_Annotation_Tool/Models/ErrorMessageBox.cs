using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ML_Annotation_Tool.Models
{
    public class ErrorMessageBox : Window
    {
        Window ErrorWindow;
        public ErrorMessageBox(string message)
        {
            ErrorWindow = new Window();
            TextBlock txt = new TextBlock();
            txt.Text = message;
            txt.TextWrapping = TextWrapping.Wrap;

            Button CloseButton = new Button();
            CloseButton.Content = "Close Window";
            CloseButton.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            CloseButton.Margin = new Thickness(12);
            CloseCommand Close = new CloseCommand(ErrorWindow);
            CloseButton.Command = Close;

            StackPanel TextButtonStackPanel = new StackPanel();
            TextButtonStackPanel.Children.Add(txt);
            TextButtonStackPanel.Children.Add(CloseButton);

            ErrorWindow.Content = TextButtonStackPanel;
            ErrorWindow.Width = 426;
            ErrorWindow.SizeToContent = SizeToContent.Height;
            ErrorWindow.Show();
        }

    }

    public class CloseCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            this.source.Close();
        }
        Window source;
        public CloseCommand(Window source)
        {
            this.source = source;
        } 
    }
}
