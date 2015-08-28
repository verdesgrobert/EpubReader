﻿using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using VersFx.Formats.Text.Epub.Entities;
using VersFx.Formats.Text.Epub.Readers;
using VersFx.Formats.Text.Epub.Schema.Navigation;
using VersFx.Formats.Text.Epub.Schema.Opf;
using VersFx.Formats.Text.Epub.Utils;

namespace VersFx.Formats.Text.Epub
{
    public static class EpubReader
    {
        public static EpubBook OpenBook(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Specified epub file not found.", filePath);
            EpubBook book = new EpubBook();
            book.FilePath = filePath;
            using (ZipArchive epubArchive = ZipFile.OpenRead(filePath))
            {
                book.Schema = SchemaReader.ReadSchema(epubArchive);
                book.Title = book.Schema.Package.Metadata.Titles.FirstOrDefault() ?? String.Empty;
                book.AuthorList = book.Schema.Package.Metadata.Creators.Select(creator => creator.Creator).ToList();
                book.Author = String.Join(", ", book.AuthorList);
                book.Content = ContentReader.ReadContentFiles(epubArchive, book);
                book.CoverImage = LoadCoverImage(book);
                book.Chapters = LoadChapters(book, epubArchive);
            }
            return book;
        }

        private static Image LoadCoverImage(EpubBook book)
        {
            List<EpubMetadataMeta> metaItems = book.Schema.Package.Metadata.MetaItems;
            if (metaItems == null || !metaItems.Any())
                return null;
            EpubMetadataMeta coverMetaItem = metaItems.FirstOrDefault(metaItem => String.Compare(metaItem.Name, "cover", StringComparison.OrdinalIgnoreCase) == 0);
            if (coverMetaItem == null)
                return null;
            if (String.IsNullOrEmpty(coverMetaItem.Content))
                throw new Exception("Incorrect EPUB metadata: cover item content is missing");
            EpubManifestItem coverManifestItem = book.Schema.Package.Manifest.FirstOrDefault(manifestItem => String.Compare(manifestItem.Id, coverMetaItem.Content, StringComparison.OrdinalIgnoreCase) == 0);
            if (coverManifestItem == null)
                throw new Exception(String.Format("Incorrect EPUB manifest: item with ID = \"{0}\" is missing", coverMetaItem.Content));
            EpubByteContentFile coverImageContentFile;
            if (!book.Content.Images.TryGetValue(coverManifestItem.Href, out coverImageContentFile))
                throw new Exception(String.Format("Incorrect EPUB manifest: item with href = \"{0}\" is missing", coverManifestItem.Href));
            using (MemoryStream coverImageStream = new MemoryStream(coverImageContentFile.Content))
                return Image.FromStream(coverImageStream);
        }

        private static List<EpubChapter> LoadChapters(EpubBook book, ZipArchive epubArchive)
        {
            return LoadChapters(book, book.Schema.Navigation.NavMap, epubArchive);
        }

        private static List<EpubChapter> LoadChapters(EpubBook book, List<EpubNavigationPoint> navigationPoints, ZipArchive epubArchive, EpubChapter parentChapter = null)
        {
            List<EpubChapter> result = new List<EpubChapter>();
            foreach (EpubNavigationPoint navigationPoint in navigationPoints)
            {
                EpubChapter chapter = new EpubChapter();
                chapter.Book = book;
                chapter.Title = navigationPoint.NavigationLabels.First().Text;
                int contentSourceAnchorCharIndex = navigationPoint.Content.Source.IndexOf('#');
                if (contentSourceAnchorCharIndex == -1)
                    chapter.ContentFileName = navigationPoint.Content.Source;
                else
                {
                    chapter.ContentFileName = navigationPoint.Content.Source.Substring(0, contentSourceAnchorCharIndex);
                    chapter.Anchor = navigationPoint.Content.Source.Substring(contentSourceAnchorCharIndex + 1);
                }
                EpubTextContentFile htmlContentFile;
                if (!book.Content.Html.TryGetValue(chapter.ContentFileName, out htmlContentFile))
                    throw new Exception(String.Format("Incorrect EPUB manifest: item with href = \"{0}\" is missing", chapter.ContentFileName));
                chapter.HtmlContent = htmlContentFile.Content;
                chapter.SubChapters = LoadChapters(book, navigationPoint.ChildNavigationPoints, epubArchive, chapter);
                result.Add(chapter);
            }
            //if (result.Count == 0 && parentChapter != null)
            //{
            //    var html = parentChapter.HtmlContent;
            //    HtmlDocument doc = new HtmlDocument();
            //    doc.LoadHtml(html);
            //    int count = 0;
            //    foreach (var node in doc.DocumentNode.QuerySelectorAll("p"))
            //    {
            //        EpubChapter chapter = new EpubChapter();
            //        chapter.Title = node.InnerText.Substring(0, Math.Min(node.InnerText.Length, 80));
            //        if (chapter.Title.Length < node.InnerText.Length)
            //            chapter.Title = chapter.Title.Substring(0, chapter.Title.LastIndexOf(" "));
            //        node.SetAttributeValue("id", "p" + count);
            //        count++;
            //        chapter.HtmlId = node.Id;
            //        chapter.HtmlContent = node.InnerHtml;
            //        chapter.SubChapters = LoadChapters(book, new List<EpubNavigationPoint>(), epubArchive);
            //        result.Add(chapter);
            //    }
            //    parentChapter.HtmlContent = doc.DocumentNode.InnerHtml;

            //}
            return result;
        }
    }
}
