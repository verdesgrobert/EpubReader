using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using EpubReaderDemo.Models;
using EpubReaderDemo.Utils;
using VersFx.Formats.Text.Epub;
using EpubReaderDemo.Views;

namespace EpubReaderDemo.ViewModels
{
    public class BookViewModel : ViewModel
    {
        private readonly BookModel bookModel;

        private Dictionary<string, byte[]> images;
        private Dictionary<string, string> styleSheets;
        private Dictionary<string, byte[]> fonts;
        private ChapterViewModel selectedChapter;
        private ChapterContentViewModel selectedChapterContent;
        private Command selectChapterCommand;

        public BookViewModel()
        {
            bookModel = new BookModel();
        }
        public BookViewModel(int bookId) : this()
        {
            LoadBook(bookId);
        }

        public void LoadBook(int bookId, bool append = false)
        {
            var epubBook = bookModel.OpenBook(bookId);
            Contents = Contents.AddRange(bookModel.GetChapters(epubBook));
            images = images.AddRange(epubBook.Content.Images.ToDictionary(imageFile => imageFile.Key, imageFile => imageFile.Value.Content));
            styleSheets = styleSheets.AddRange(epubBook.Content.Css.ToDictionary(cssFile => cssFile.Key, cssFile => cssFile.Value.Content));
            fonts = fonts.AddRange(epubBook.Content.Fonts.ToDictionary(fontFile => fontFile.Key, fontFile => fontFile.Value.Content));

            selectChapterCommand = null;
            selectedChapter = null;
            selectedChapterContent = null;
            if (Contents.Any())
                SelectChapter(Contents.First());
        }

        public ObservableCollection<ChapterViewModel> Contents { get; private set; }

        public ChapterContentViewModel SelectedChapterContent
        {
            get
            {
                return selectedChapterContent;
            }
            set
            {
                selectedChapterContent = value;
                OnPropertyChanged(() => SelectedChapterContent);
            }
        }
        public string SelectedElement { get; set; }

        public ICommand SelectChapterCommand
        {
            get
            {
                if (selectChapterCommand == null)
                    selectChapterCommand = new Command(param => SelectChapter(param as ChapterViewModel));
                return selectChapterCommand;
            }
        }

        private void SelectChapter(ChapterViewModel chapterViewModel)
        {
            if (string.IsNullOrEmpty(chapterViewModel.HtmlId))
            {
                bool samePage = false;
                if (selectedChapter != null)
                {
                    selectedChapter.IsSelected = false;
                    if (selectedChapter == chapterViewModel || (!string.IsNullOrEmpty(selectedChapter.HtmlId) && chapterViewModel.ParentChapter == selectedChapter))
                    {
                        selectedChapter = null;
                        selectedChapterContent = null;
                        samePage = true;

                    }
                }
                selectedChapter = chapterViewModel;
                selectedChapter.IsTreeItemExpanded = true;
                selectedChapter.IsSelected = true;
                SelectedChapterContent = new ChapterContentViewModel(selectedChapter.HtmlContent, images, styleSheets, fonts);
                if (BookView.View != null && samePage)
                    BookView.View.ScrollTo("");
            }
            else
            {
                if (selectedChapter == null)
                {
                    selectedChapter = chapterViewModel.ParentChapter;
                }
                else
                    selectedChapter.IsSelected = false;
                selectedChapter = chapterViewModel;
                selectedChapter.IsTreeItemExpanded = true;
                selectedChapter.IsSelected = true;
                if (SelectedChapterContent.HtmlContent != chapterViewModel.ParentChapter.HtmlContent)
                    SelectedChapterContent = new ChapterContentViewModel(chapterViewModel.ParentChapter.HtmlContent, images, styleSheets, fonts);
                Task.Run(async () =>
                {
                    await Task.Delay(100);
                    if (BookView.View != null)
                        BookView.View.ScrollTo(chapterViewModel.HtmlId);
                });
                // SelectedElement = chapterViewModel.HtmlContent;
            }
        }
    }
}
