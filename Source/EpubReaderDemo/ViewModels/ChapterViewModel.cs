using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpubReaderDemo.ViewModels
{
    public class ChapterViewModel : ViewModel
    {
        private bool isTreeItemExpanded;
        private bool isSelected;

        public ChapterViewModel(string title, IEnumerable<ChapterViewModel> subChapters, string htmlContent, string htmlId, IEnumerable<ChapterViewModel> relatedChapters = null)
        {
            Title = title;
            foreach (var chap in subChapters)
            {
                chap.ParentChapter = this;
            }
            SubChapters = new ObservableCollection<ChapterViewModel>(subChapters);
            HtmlContent = htmlContent;
            HtmlId = htmlId;
            isTreeItemExpanded = false;
            isSelected = false;
            if (relatedChapters != null)
                RelatedChapters = new ObservableCollection<ChapterViewModel>(relatedChapters);
        }

        public string Title { get; private set; }
        public ObservableCollection<ChapterViewModel> SubChapters { get; private set; }
        public ObservableCollection<ChapterViewModel> RelatedChapters { get; private set; }
        public string HtmlContent { get; private set; }

        public bool IsTreeItemExpanded
        {
            get
            {
                return isTreeItemExpanded;
            }
            set
            {
                isTreeItemExpanded = value;
                OnPropertyChanged(() => IsTreeItemExpanded);
            }
        }

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
                OnPropertyChanged(() => IsSelected);
            }
        }

        public string HtmlId { get; set; }
        public ChapterViewModel ParentChapter { get; internal set; }
        
    }
}
