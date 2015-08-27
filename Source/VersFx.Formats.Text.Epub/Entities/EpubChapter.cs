using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VersFx.Formats.Text.Epub.Entities
{
    public class EpubChapter
    {
        public string Title { get; set; }
        public string ContentFileName { get; set; }
        public string Anchor { get; set; }
        public string HtmlContent { get; set; }
        public string HtmlId { get; set; }
        public List<EpubChapter> SubChapters { get; set; }
        public EpubBook Book { get; internal set; }
        public string Reference { get { return Book.Title + ":" + Title; } }
        public override string ToString()
        {
            return String.Format("Title: {0}, Subchapter count: {1}", Title, SubChapters.Count);
        }
    }


}
