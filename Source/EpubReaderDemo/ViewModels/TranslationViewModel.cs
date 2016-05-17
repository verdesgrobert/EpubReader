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
    public class TranslationViewModel : ViewModel
    {
        private BookModel bookModel;
        private EpubBook epubBook;
        private EpubBook epubBook1;
        private EpubBook epubBook2;

        private Dictionary<string, byte[]> images;
        private Dictionary<string, string> styleSheets;
        private Dictionary<string, byte[]> fonts;
        private ChapterViewModel selectedChapter;
        private ChapterContentViewModel selectedChapterContent;
        private ChapterContentViewModel selectedChapterContent1;
        private ChapterContentViewModel selectedChapterContent2;
        private Command selectChapterCommand;

        public TranslationViewModel() : this(1)
        {

        }
        public TranslationViewModel(int bookId)
        {
            selectChapterCommand = null;
            selectedChapter = null;
            selectedChapterContent = null;
            LoadBook(bookId);
        }

        private void LoadBook(int bookId)
        {
            bookModel = new BookModel();
            epubBook = bookModel.OpenBook(bookId);
            epubBook1 = bookModel.OpenBook(bookId + 1);
            epubBook2 = bookModel.OpenBook(bookId + 2);
            Contents = new ObservableCollection<ChapterViewModel>(bookModel.GetChapters(epubBook, epubBook1, epubBook2));
            images = epubBook.Content.Images.ToDictionary(imageFile => imageFile.Key, imageFile => imageFile.Value.Content);
            styleSheets = epubBook.Content.Css.ToDictionary(cssFile => cssFile.Key, cssFile => cssFile.Value.Content);
            fonts = epubBook.Content.Fonts.ToDictionary(fontFile => fontFile.Key, fontFile => fontFile.Value.Content);
            if (Contents.Any())
                SelectChapter(Contents.First());
        }

        public ObservableCollection<ChapterViewModel> Contents { get; private set; }

        public ChapterContentViewModel SelectedChapterContent
        {
            get { return selectedChapterContent; }
            set { selectedChapterContent = value; OnPropertyChanged(() => SelectedChapterContent); }
        }
        public ChapterContentViewModel SelectedChapterContent1
        {
            get { return selectedChapterContent1; }
            set { selectedChapterContent1 = value; OnPropertyChanged(() => SelectedChapterContent1); }
        }
        public ChapterContentViewModel SelectedChapterContent2
        {
            get { return selectedChapterContent2; }
            set { selectedChapterContent2 = value; OnPropertyChanged(() => SelectedChapterContent2); }
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
                SelectedChapterContent1 = new ChapterContentViewModel(selectedChapter.RelatedChapters[0].HtmlContent, images, styleSheets, fonts);
                SelectedChapterContent2 = new ChapterContentViewModel(selectedChapter.RelatedChapters[1].HtmlContent, images, styleSheets, fonts);
                if (TranslationView.View != null && samePage)
                    TranslationView.View.ScrollTo("");
            }
            else
            {
                if (selectedChapter == null)
                {
                    selectedChapter = chapterViewModel.ParentChapter;
                }
                else
                {
                    selectedChapter.IsSelected = false;
                }
                selectedChapter = chapterViewModel;
                selectedChapter.IsTreeItemExpanded = true;
                selectedChapter.IsSelected = true;
                if (SelectedChapterContent.HtmlContent != chapterViewModel.ParentChapter.HtmlContent)
                    SelectedChapterContent = new ChapterContentViewModel(chapterViewModel.ParentChapter.HtmlContent, images, styleSheets, fonts);

                if (SelectedChapterContent1.HtmlContent != chapterViewModel.RelatedChapters[0].ParentChapter.HtmlContent)
                    SelectedChapterContent1 = new ChapterContentViewModel(chapterViewModel.RelatedChapters[0].ParentChapter.HtmlContent, images, styleSheets, fonts);
                if (SelectedChapterContent2.HtmlContent != chapterViewModel.RelatedChapters[1].ParentChapter.HtmlContent)
                    SelectedChapterContent2 = new ChapterContentViewModel(chapterViewModel.RelatedChapters[1].ParentChapter.HtmlContent, images, styleSheets, fonts);
                Task.Run(async () =>
                {
                    await Task.Delay(100);
                    if (TranslationView.View != null)
                        TranslationView.View.ScrollTo(chapterViewModel.HtmlId);
                });
            }
        }
    }
}
