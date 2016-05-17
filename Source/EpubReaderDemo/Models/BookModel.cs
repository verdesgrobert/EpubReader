using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EpubReaderDemo.Entities;
using EpubReaderDemo.ViewModels;
using VersFx.Formats.Text.Epub;
using VersFx.Formats.Text.Epub.Entities;

namespace EpubReaderDemo.Models
{
    internal class BookModel
    {
        private readonly ApplicationContext applicationContext;
        private readonly Settings settings;

        public BookModel()
        {
            applicationContext = ApplicationContext.Instance;
            settings = applicationContext.Settings;
        }

        public EpubBook OpenBook(int bookId)
        {
            EpubBook epubBook = EpubReader.OpenBook(settings.Books.First(book => book.Id == bookId).FilePath);
            return epubBook;
        }

        public List<ChapterViewModel> GetChapters(EpubBook epubBook)
        {
            return GetChapters(epubBook.Chapters);
        }
        public List<ChapterViewModel> GetChapters(EpubBook epubBook, EpubBook epubBook1, EpubBook epubBook2)
        {
            return GetChapters(epubBook.Chapters, epubBook1.Chapters, epubBook2.Chapters);
        }

        private List<ChapterViewModel> GetChapters(List<EpubChapter> epubChapters, List<EpubChapter> epubChapters1, List<EpubChapter> epubChapters2)
        {
            List<ChapterViewModel> result = new List<ChapterViewModel>();
            for (int index = 0; index < epubChapters.Count; index++)
            {
                EpubChapter epubChapter1 = epubChapters1[index];
                List<ChapterViewModel> subChapters1 = GetChapters(epubChapter1.SubChapters);
                EpubChapter epubChapter2 = epubChapters2[index];
                List<ChapterViewModel> subChapters2 = GetChapters(epubChapter2.SubChapters);
                EpubChapter epubChapter0 = epubChapters[index];

                List<ChapterViewModel> subChapters0 = GetChapters(epubChapter0.SubChapters, epubChapter1.SubChapters, epubChapter2.SubChapters);

                ChapterViewModel chapterViewModel1 = new ChapterViewModel(epubChapter1.Title, subChapters1, epubChapter1.HtmlContent, epubChapter1.HtmlId);
                ChapterViewModel chapterViewModel2 = new ChapterViewModel(epubChapter2.Title, subChapters2, epubChapter2.HtmlContent, epubChapter2.HtmlId);
                ChapterViewModel chapterViewModel = new ChapterViewModel(epubChapter0.Title, subChapters0, epubChapter0.HtmlContent, epubChapter0.HtmlId, new List<ChapterViewModel> { chapterViewModel1, chapterViewModel2 });
                result.Add(chapterViewModel);
            }
            return result;
        }

        private List<ChapterViewModel> GetChapters(List<EpubChapter> epubChapters)
        {
            List<ChapterViewModel> result = new List<ChapterViewModel>();
            for (int index = 0; index < epubChapters.Count; index++)
            {
                EpubChapter epubChapter = epubChapters[index];
                List<ChapterViewModel> subChapters = GetChapters(epubChapter.SubChapters);
                ChapterViewModel chapterViewModel = new ChapterViewModel(epubChapter.Title, subChapters,
                    epubChapter.HtmlContent, epubChapter.ContentFileName);
                result.Add(chapterViewModel);
            }
            return result;
        }
    }
}
