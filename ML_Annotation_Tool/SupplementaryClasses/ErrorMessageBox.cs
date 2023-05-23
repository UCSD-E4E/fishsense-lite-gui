using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Windows.Input;

namespace FishSenseLiteGUI.SupplementaryClasses
{
    /// <summary>
    /// Purpose: Displays simple error box (similar to ErrorMessageBox in WPF).
    /// 
    /// Note: Has no constructor, just a static method whose usage is ErrorMessageBox.Show("Error Message").
    /// </summary>
    public class ErrorMessageBox : Window
    {
        private static Window ErrorWindow;
        private static TextBlock ErrorText;
        private static Button CloseButton;
        private static CloseCommand Close;
        private static StackPanel TextButtonStackPanel;
        public static void Show(string message)
        {
            ErrorWindow = new Window();

            ErrorText = new TextBlock();
            ErrorText.Text = message;
            ErrorText.TextWrapping = TextWrapping.Wrap;

            CloseButton = new Button();
            CloseButton.Content = "Close Window";
            CloseButton.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            CloseButton.Margin = new Thickness(12);

            Close = new CloseCommand(ErrorWindow);
            CloseButton.Command = Close;


            TextButtonStackPanel = new StackPanel();
            TextButtonStackPanel.Children.Add(ErrorText);
            TextButtonStackPanel.Children.Add(CloseButton);

            ErrorWindow.Content = TextButtonStackPanel;
            ErrorWindow.Width = 426;
            ErrorWindow.SizeToContent = SizeToContent.Height;
            ErrorWindow.Focus();
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
            source.Close();
        }
        Window source;
        public CloseCommand(Window source)
        {
            this.source = source;
        }
    }
}
