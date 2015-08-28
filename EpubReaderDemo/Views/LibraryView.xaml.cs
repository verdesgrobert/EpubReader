using EpubReaderDemo.Models;
using EpubReaderDemo.Utils;
using EpubReaderDemo.ViewModels;
using EpubReaderDemo.WpfEnvironment;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EpubReaderDemo.Views
{
    public partial class LibraryView : ViewModel
    {
        public LibraryView()
        {
            InitializeComponent();
            libraryModel = new LibraryModel();
            addBookCommand = null;
            removeFromLibraryCommand = null;
            openBookCommand = null;
            RefreshLibrary();
        }
        private readonly LibraryModel libraryModel;

        private Command addBookCommand;
        private Command removeFromLibraryCommand;
        private Command openBookCommand;


        public ObservableCollection<LibraryItemViewModel> Books { get; private set; }

        public ICommand AddBookCommand
        {
            get
            {
                if (addBookCommand == null)
                    addBookCommand = new Command(AddBook);
                return addBookCommand;
            }
        }

        public ICommand RemoveFromLibraryCommand
        {
            get
            {
                if (removeFromLibraryCommand == null)
                    removeFromLibraryCommand = new Command(param => RemoveBookFromLibrary(param as LibraryItemViewModel));
                return removeFromLibraryCommand;
            }
        }

        public ICommand OpenBookCommand
        {
            get
            {
                if (openBookCommand == null)
                    openBookCommand = new Command(param => OpenBook(param as LibraryItemViewModel));
                return openBookCommand;
            }
        }

        private void RefreshLibrary()
        {
            Books = new ObservableCollection<LibraryItemViewModel>(libraryModel.GetLibraryItems());
            OnPropertyChanged(() => Books);
        }

        private void AddBook()
        {
            OpenFileDialogParameters openFileDialogParameters = new OpenFileDialogParameters
            {
                Filter = "EPUB files (*.epub)|*.epub|All files (*.*)|*.*",
                Multiselect = true
            };
            OpenFileDialogResult openDialogResult = ShowOpenFileDialog(openFileDialogParameters);
            if (openDialogResult.DialogResult)
            {
                foreach (string selectedFilePath in openDialogResult.SelectedFilePaths)
                    libraryModel.AddBookToLibrary(selectedFilePath);
                RefreshLibrary();
            }
        }
        private OpenFileDialogResult ShowOpenFileDialog(OpenFileDialogParameters openFileDialogParameters)
        {
            if (openFileDialogParameters == null)
                throw new ArgumentNullException("openFileDialogParameters");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (!String.IsNullOrEmpty(openFileDialogParameters.Filter))
                openFileDialog.Filter = openFileDialogParameters.Filter;
            openFileDialog.Multiselect = openFileDialogParameters.Multiselect;
            if (!String.IsNullOrEmpty(openFileDialogParameters.InitialDirectory))
                openFileDialog.InitialDirectory = openFileDialogParameters.InitialDirectory;
            OpenFileDialogResult result = new OpenFileDialogResult();
            result.DialogResult = openFileDialog.ShowDialog() == true;
            result.SelectedFilePaths = result.DialogResult ? openFileDialog.FileNames.ToList() : new List<string>();
            return result;
        }

        private void RemoveBookFromLibrary(LibraryItemViewModel book)
        {
            libraryModel.RemoveBookFromLibrary(book);
            RefreshLibrary();
        }

        private void OpenBook(LibraryItemViewModel book)
        {
            //return;
            //BookViewModel bookViewModel = new BookViewModel(book.Id);
            //IWindowContext bookWindowContext = windowManager.CreateWindow(bookViewModel);
            //bookWindowContext.Show();
        }
    }
}
