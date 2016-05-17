﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpubReaderDemo.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public string Title { get; set; }
        public bool HasCover { get; set; }
    }
}
