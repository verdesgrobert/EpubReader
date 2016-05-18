using EpubReaderDemo.Models;
using EpubReaderDemo.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using VersFx.Formats.Text.Epub;
using VersFx.Formats.Text.Epub.Entities;

namespace EpubReaderDemo.Views
{
    public partial class TranslationView : Window, INotifyPropertyChanged
    {
        public TranslationView()
        {
            InitializeComponent();
            View = this;
            ShowWatermark = true;
            txtSearch.GotFocus += TxtSearch_GotFocus;
            txtSearch.LostFocus += TxtSearch_LostFocus;
            book1HTML.Scrolled += BookHTML_Scrolled;
            book2HTML.Scrolled += BookHTML_Scrolled;
            book3HTML.Scrolled += BookHTML_Scrolled;
        }

        private void BookHTML_Scrolled(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            book1HTML.ScrollVertically(e.NewValue);
            book2HTML.ScrollVertically(e.NewValue);
            book3HTML.ScrollVertically(e.NewValue);
        }

        private void TxtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            ShowWatermark = string.IsNullOrEmpty(txtSearch.Text);
        }

        private void TxtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            ShowWatermark = false;// string.IsNullOrEmpty(txtSearch.Text);
        }


        public static TranslationView View
        {
            get; set;
        }
        public void ScrollTo(string elemId)
        {
            string status = "";
            List<string> availableIds = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(elemId))
                {
                    bookHTML.ScrollToTop();
                    book1HTML.ScrollToTop();
                    book2HTML.ScrollToTop();
                    book3HTML.ScrollToTop();
                }
                else
                {
                    bookHTML.ScrollToElement(elemId, out status, out availableIds);
                    book1HTML.ScrollToElement(elemId, out status, out availableIds);
                    book2HTML.ScrollToElement(elemId, out status, out availableIds);
                    book3HTML.ScrollToElement(elemId, out status, out availableIds);
                }
                if (!string.IsNullOrEmpty(status))
                {
                    MessageBox.Show(status + string.Join(Environment.NewLine, availableIds));
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(status + string.Join(Environment.NewLine, availableIds));
            }
        }
        public bool _showWatermark;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool ShowWatermark
        {
            get { return _showWatermark; }
            set
            {
                _showWatermark = value;
                Title = value ? "ShowWatermark" : "HideWatermark";
                OnPropertyChanged();
            }
        }
        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private System.Windows.Forms.Timer SearchTextBoxTimer;
        private string filter = "";
        private void TxtSearch_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (SearchTextBoxTimer != null)
            {
                Console.WriteLine("The user is currently typing.");
                if (SearchTextBoxTimer.Interval < 750)
                {
                    SearchTextBoxTimer.Interval += 750;
                    Console.WriteLine("Delaying...");
                }
            }
            else
            {
                Console.WriteLine("The user just started typing.");
                SearchTextBoxTimer = new System.Windows.Forms.Timer();
                SearchTextBoxTimer.Tick += new EventHandler(SearchTextBoxTimer_Tick);
                SearchTextBoxTimer.Interval = 1000;
                SearchTextBoxTimer.Start();
            }
        }
        List<SearchResult> SearchResults;
        private async void SearchTextBoxTimer_Tick(object sender, EventArgs e)
        {
            SearchTextBoxTimer.Stop();
            SearchTextBoxTimer.Dispose();
            SearchTextBoxTimer = null;

            filter = txtSearch.Text;
            txtSearch.IsEnabled = false;
            await Search();
            txtSearch.IsEnabled = true;
        }

        private async Task Search()
        {
            SearchResults = new List<SearchResult>();
            LibraryViewModel vm = new LibraryViewModel();
            foreach (var book in vm.Books)
            {
                var bk = new BookModel().OpenBook(book.Id);
                foreach (var chapter in bk.Chapters)
                {
                    SearchInChapter(chapter);
                }
            }
            lstResults.ItemsSource = SearchResults;
        }

        private void SearchInChapter(EpubChapter chapter)
        {
            if (chapter.SubChapters.Count > 0)
            {
                foreach (var item in chapter.SubChapters)
                {
                    SearchInChapter(item);
                }
            }
            else
            {
                try
                {
                    int results = 0;
                    var str = chapter.HtmlContent;
                    int index = str.ToLower().IndexOf(filter.ToLower(), StringComparison.InvariantCulture);
                    int lastResult = index + filter.Length;
                    while (index >= 0)
                    {
                        string before = "";
                        string originalFound = "";
                        string after = "";
                        if (index > 0)
                            before = str.Substring(0, index);
                        if (index + filter.Length < str.Length)
                            after = str.Substring(index + filter.Length);
                        originalFound = str.Substring(index, filter.Length);
                        SearchResults.Add(new SearchResult()
                        {
                            HtmlId = "res" + results,
                            BeforeResult = before.Substring(Math.Max(0, before.Length - 60)),
                            Result = originalFound,
                            AfterResult = after.Substring(0, Math.Min(60, after.Length)),
                            Reference = chapter
                        });
                        originalFound = "<result id=\"res" + results + "\">" + originalFound + "</result>";
                        lastResult = index + originalFound.Length;

                        str = before + originalFound + after;
                        results++;
                        index = str.ToLower().IndexOf(filter.ToLower(), lastResult, StringComparison.InvariantCulture);
                    }
                    chapter.HtmlContent = str;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }


        public class SearchResult : INotifyPropertyChanged
        {
            private string _beforeResult;
            private string _result;
            private string _afterResult;
            private EpubChapter _reference;

            public string BeforeResult
            {
                get { return _beforeResult; }
                set
                {
                    _beforeResult = value;
                    OnPropertyChanged();
                }
            }

            public string Result
            {
                get { return _result; }
                set
                {
                    _result = value;
                    OnPropertyChanged();
                }
            }

            public string AfterResult
            {
                get { return _afterResult; }
                set
                {
                    _afterResult = value;
                    OnPropertyChanged();
                }
            }

            public EpubChapter Reference
            {
                get { return _reference; }
                set
                {
                    _reference = value;
                    OnPropertyChanged();
                }
            }

            public string HtmlId { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }


    }
}
