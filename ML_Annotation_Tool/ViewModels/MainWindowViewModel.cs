using Avalonia.Controls;
using Avalonia.Media.Imaging;
using ML_Annotation_Tool.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows.Input;

namespace ML_Annotation_Tool.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Please choose the directory that contains the images";
        public string FileExplorerButtonText => "Choose directory";

        private int _selectedIndex;
        public int selectedIndex
        {
            get { return _selectedIndex; }
            set { _selectedIndex = value;
                OnPropertyChanged(nameof(selectedIndex));
            }
        }

        private readonly string _lineOne = "This app is designed to allow you to annotate image features and annotate them in a YOLO format. Press";
        private readonly string _lineTwo = "the button below to input the directory that contains the files which you want to display. Later, you";
        private readonly string _lineThree = "will have an option to choose where you wish to start viewing the files, and which files to display";
        private readonly string _lineFour = "from the directory itself.";

        public string lineOne => _lineOne;
        public string lineTwo => _lineTwo;
        public string lineThree => _lineThree;
        public string lineFour => _lineFour;

        public ObservableCollection<string> fileNames { get; set; }

        public ICommand fileExplorer { get; }

        private bool _secondPageEnabled = false;
        public bool secondPageEnabled
        {
            get { return _secondPageEnabled; }
            set { 
                 _secondPageEnabled = value; 
                OnPropertyChanged(nameof(secondPageEnabled));
            }  
        }

        private bool _thirdPageEnabled = false;
        public bool thirdPageEnabled
        {
            get { return _thirdPageEnabled; }
            set
            {
                _thirdPageEnabled = value;
                OnPropertyChanged(nameof(thirdPageEnabled));
            }
        }

        private bool _displaySelectedImages = true;
        public bool displaySelectedImages
        {
            get { return _displaySelectedImages; }
            set
            {
                _displaySelectedImages = value;
                OnPropertyChanged(nameof(displaySelectedImages));
                OnPropertyChanged(nameof(removingSelectedImages));
            }
        }
        public bool removingSelectedImages
        {
            get { return !_displaySelectedImages; }
            set
            {
                _displaySelectedImages = value;
            }
        }
        public ICommand ImageSelection;
        public ICommand nextPage { get; }

        private Bitmap _imageToShow;
        public Bitmap ImageToShow 
        {
            get 
            {
                return _imageToShow; 
            }
            set 
            { 
                _imageToShow = value;
                OnPropertyChanged(nameof(ImageToShow));
            }
        }
        public MainWindowViewModel()
        {   
            fileNames = new ObservableCollection<string>();

            selectedIndex = 0;

            fileExplorer = new FileExplorerCommand(this);
            //ImageSelection = new ImageSelectionCommand(this);
            nextPage = new NextPageCommand(this);
        }
    }
}