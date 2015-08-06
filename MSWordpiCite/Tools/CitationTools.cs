using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MSWordpiCite.Entities;
using MSWordpiCite.Enums;
using MSWordpiCite.Classes;
using MSWordpiCite.CitationGenerator;
using System.Web;
using System.Runtime.InteropServices;
using System.Drawing;

namespace MSWordpiCite.Tools
{
    class CitationTools
    {
        /// <summary>
        /// Return number of authors with provided NameType
        /// </summary>
        /// <param name="authors">Author list</param>
        /// <param name="nametype">NameType</param>
        /// <returns></returns>
        public static int CountAuthor(List<NameMasterRow> authors, NameTypes nametype)
        {
            int num = 0;
            foreach (NameMasterRow name in authors)
            {
                if (name.NameTypeID == nametype)
                    num++;
            }
            return num;
        }

        /// <summary>
        /// Return Author delimitor in String
        /// </summary>
        /// <param name="lim">AuthorDelimitor delimitorType</param>
        /// <returns></returns>
        public static string GetAuthorDelimitor(AuthorDelimitor lim)
        {
            if (lim == AuthorDelimitor.Comma)
                return ", ";            
            else if (lim == AuthorDelimitor.Semicolon)
                return "; ";
            else
                return "";
        }

        /// <summary>
        /// Return Author name in desired format
        /// </summary>
        /// <param name="name">NameMasterRow authorname</param>
        /// <param name="format">AuthorFormat format</param>
        /// <returns></returns>
        public static string FormatName(NameMasterRow name, AuthorFormat format)
        {
            string formatname = string.Empty;
            string[] temp = Regex.Split(name.ForeName, @"[\s|-]+");
            name.Initials = string.Empty;
            for (int i = 0; i < temp.Length; i++)
            {
                if(Regex.Replace(temp[i], @"\s+", "").Length > 0)
                    name.Initials += temp[i].Substring(0, 1) + "|";
            }
            name.Initials = name.Initials.ToUpper();
            switch(format)
            {
                //William Jefferson Clinton
                case AuthorFormat.FullNameFirstNameFirst:
                    formatname = name.ForeName;
                    if (formatname.Length > 0)
                        formatname += " " + name.LastName;
                    else
                        formatname = name.LastName;
                    break;

                //W. J. Clinton
                case AuthorFormat.InitialWithDot:
                    formatname = name.Initials;
                    if (formatname.Length > 0)
                        formatname = Regex.Replace(formatname, @"\|", ". ");
                    if (formatname.Length > 0)
                        formatname += name.LastName;
                    else
                        formatname = name.LastName;
                    break;

                //W.J. Clinton
                case AuthorFormat.InitialWithDotWithoutSpace:
                    formatname = name.Initials;
                    if (formatname.Length > 0)
                        formatname = Regex.Replace(formatname, @"\|", ".");
                    if (formatname.Length > 0)
                        formatname += " " + name.LastName;
                    else
                        formatname = name.LastName;
                    break;

                //W J Clinton
                case AuthorFormat.InitialWithoutDot:
                    formatname = name.Initials;
                    if (formatname.Length > 0)
                        formatname = Regex.Replace(formatname, @"\|", " ");
                    if (formatname.Length > 0)
                        formatname += name.LastName;
                    else
                        formatname = name.LastName;
                    break;

                //WJ Clinton
                case AuthorFormat.InitialWithoutDotWithoutSpace:
                    formatname = name.Initials;                   
                    if (formatname.Length > 0)
                        formatname = Regex.Replace(formatname, @"\|", "") + " " + name.LastName;
                    else
                        formatname = name.LastName;
                    break;

                //Clinton, William Jefferson
                case AuthorFormat.FullNameLastNameFirstWithComma:
                    formatname = name.LastName;
                    if (name.ForeName.Length > 0)
                        formatname += ", " + name.ForeName;
                    break;

                //Clinton, W. J.
                case AuthorFormat.InitialLastNameFirstWithCommaWithDot:
                    formatname = name.Initials;
                    if (formatname.Length > 0)
                        formatname = Regex.Replace(formatname, @"\|", ". ");
                    if (formatname.Length > 0)
                        formatname = name.LastName + ", " + Regex.Replace(formatname, @"^\s+|\s+$", "");
                    else
                        formatname = name.LastName;
                    break;

                //Clinton, W.J.
                case AuthorFormat.InitialLastNameFirstWithCommaWithDotWithoutSpace:
                    formatname = name.Initials;
                    if (formatname.Length > 0)
                        formatname = Regex.Replace(formatname, @"\|", ".");
                    if (formatname.Length > 0)
                        formatname = name.LastName + ", " + formatname;
                    else
                        formatname = name.LastName;
                    break;

                //Clinton, W J
                case AuthorFormat.InitialLastNameFirstWithCommaWithoutDot:
                    formatname = name.Initials;
                    if (formatname.Length > 0)
                        formatname = Regex.Replace(formatname, @"\|", " ");
                    if (formatname.Length > 0)
                        formatname = name.LastName + ", " + Regex.Replace(formatname, @"^\s+|\s+$", "");
                    else
                        formatname = name.LastName;
                    break;

                //Clinton, WJ
                case AuthorFormat.InitialLastNameFirstWithCommaWithoutDotWithoutSpace:
                    formatname = name.Initials;
                    if (formatname.Length > 0)
                        formatname = name.LastName + ", " + Regex.Replace(formatname, @"\|", "");
                    else
                        formatname = name.LastName;
                    break;

                //Clinton
                case AuthorFormat.LastNameOnly:
                    formatname = name.LastName;
                    break;

                //Clinton W. J.
                case AuthorFormat.InitialLastNameFirstWithoutCommaWithDot:
                    formatname = name.Initials;
                    if (formatname.Length > 0)
                        formatname = Regex.Replace(formatname, @"\|", ". ");
                    if (formatname.Length > 0)
                        formatname = name.LastName + " " + Regex.Replace(formatname, @"^\s+|\s+$", "");
                    else
                        formatname = name.LastName;
                    break;

                //Clinton W.J.
                case AuthorFormat.InitialLastNameFirstWithoutCommaWithDotWithoutSpace:
                    formatname = name.Initials;
                    if (formatname.Length > 0)
                        formatname = Regex.Replace(formatname, @"\|", ".");
                    if (formatname.Length > 0)
                        formatname = name.LastName + " " + formatname;
                    else
                        formatname = name.LastName;
                    break;

                //Clinton W J
                case AuthorFormat.InitialLastNameFirstWithoutCommaWithoutDot:
                    formatname = name.Initials;
                    if (formatname.Length > 0)
                        formatname = Regex.Replace(formatname, @"\|", " ");
                    if (formatname.Length > 0)
                        formatname = name.LastName + " " + Regex.Replace(formatname, @"^\s+|\s+$", "");
                    else
                        formatname = name.LastName;
                    break;

                //Clinton WJ
                case AuthorFormat.InitialLastNameFirstWithoutCommaWithoutDotWithoutSpace:
                    formatname = name.Initials;
                    if (formatname.Length > 0)
                        formatname = name.LastName + " " + Regex.Replace(formatname, @"\|", "");
                    else
                        formatname = name.LastName;
                    break;

                //William J. Clinton
                case AuthorFormat.FirstNameFirstMidNameInitialWithDot:
                    formatname = name.ForeName + " ";
                    if(name.Initials.Length > 0)
                    {
                        formatname += name.Initials.Substring(0, 1).ToUpper();
                    }
                    if (formatname.Length > 0)
                        formatname += ". ";
                    formatname += name.LastName;
                    break;

                //Clinton,W. J.
                case AuthorFormat.InitialLastNameFirstWithCommaWithDotWithoutSpaceAfterComma:
                    formatname = name.Initials;
                    if (formatname.Length > 0)
                        formatname += Regex.Replace(formatname, @"\|", ". ");
                    if (formatname.Length > 0)
                        formatname = name.LastName + "," + Regex.Replace(formatname, @"^\s+|\s+$", "");
                    else
                        formatname = name.LastName;
                    break;

                //Clinton,W.J.
                case AuthorFormat.InitialLastNameFirstWithCommaWithDotWithoutAnySpace:
                    formatname = name.Initials;
                    if (formatname.Length > 0)
                        formatname += Regex.Replace(formatname, @"\|", ".");
                    if (formatname.Length > 0)
                        formatname = name.LastName + "," + formatname;
                    else
                        formatname = name.LastName;
                    break;
            }
            return formatname;
        }

        /// <summary>
        /// Return Enclosure string
        /// </summary>
        /// <param name="enclosure">The enclosure.</param>
        /// <returns></returns>
        public static string GetEnclosure(EnclosureType enclosure)
        {
            string text = string.Empty;
            switch(enclosure)
            {
                case EnclosureType.AngleBracket:
                    text = "{}";
                    break;
                case EnclosureType.RoundBracket:
                    text = "()";
                    break;
                case EnclosureType.SquareBracket:
                    text = "[]";
                    break;
                default:
                    text = "";
                    break;
            }
            return text;
        }

        public static string ApplyStandardFormat(BaseClass field, string text, bool bInHtmlFormat)
        {
            return ApplyStandardFormat(field, text, bInHtmlFormat, string.Empty, true);
        }

        /// <summary>
        /// Applies the standard format.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string ApplyStandardFormat(BaseClass field, string text, bool bInHtmlFormat, string strFollowedBy, bool bTextPurify)
        {
            if (field.Prefix.Length > 0)
                text = field.Prefix + text;

            string _suffix = string.Empty;

            if(field.Suffix.Length > 0)
            {
                string temp = field.Suffix;
                if(Regex.Match(text, @"[\?|!]$").Success)
                    temp = Regex.Replace(temp, @"\.", "");
                if(strFollowedBy.Length > 0)
                    _suffix = temp;
                else
                    text = text + temp;
            }

            if (bTextPurify)
            {
                text = text.Replace("<", "&lt;").Replace(">", "&gt;");
            }            

            if(bInHtmlFormat)
            {
                if (field.Bold || field.Italic || field.Underline)
                {
                    if (field.Bold)
                        text = GetHtmlFormatString(text, TextFormat.Bold);
                    if (field.Italic)
                        text = GetHtmlFormatString(text, TextFormat.Italic);
                    if (field.Underline)
                        text = GetHtmlFormatString(text, TextFormat.Underline);
                }
                else
                    text = GetHtmlFormatString(text, TextFormat.None);
            }
            else
            {
                if (field.Bold || field.Italic || field.Underline)
                {
                    if (field.Bold)
                        text = GetWordMLFormatString(text, TextFormat.Bold);
                    if (field.Italic)
                        text = GetWordMLFormatString(text, TextFormat.Italic);
                    if (field.Underline)
                        text = GetWordMLFormatString(text, TextFormat.Underline);
                    if (strFollowedBy.Length > 0)
                    {
                        text += strFollowedBy;
                        if (field.Bold)
                            _suffix = GetWordMLFormatString(_suffix, TextFormat.Bold);
                        if (field.Italic)
                            _suffix = GetWordMLFormatString(_suffix, TextFormat.Italic);
                        if (field.Underline)
                            _suffix = GetWordMLFormatString(_suffix, TextFormat.Underline);
                        text += _suffix;
                    }
                }
                else
                {
                    text = GetWordMLFormatString(text, TextFormat.None);
                    if(strFollowedBy.Length > 0)
                    {
                        text += strFollowedBy + GetWordMLFormatString(_suffix, TextFormat.None);
                    }
                }
            }            
            text = purifyWordString(text);
            return text;
        }

        /// <summary>
        /// Purifies the word string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private static string purifyWordString(string text)
        {
            text = text.Replace("&amp;", "&");
            text = text.Replace("&", "&amp;");
            return text;
        }

        /// <summary>
        /// Gets the raw text.
        /// </summary>
        /// <param name="formatstring">The formatstring.</param>
        /// <returns></returns>
        public static string GetRawText(string formatstring)
        {
            string text = Regex.Replace(formatstring, @"<.*?>", "");
            text = Regex.Replace(text, @"&amp;", "&");
            return text;
        }

        /// <summary>
        /// Gets the format from formatted string.
        /// </summary>
        /// <param name="formatstring">The formatstring.</param>
        /// <param name="isBold">if set to <c>true</c> [is bold].</param>
        /// <param name="isItalic">if set to <c>true</c> [is italic].</param>
        /// <param name="isUnderline">if set to <c>true</c> [is underline].</param>
        /// <param name="isSuperscript">if set to <c>true</c> [is superscript].</param>
        public static void GetFormatFromFormattedString(string formatstring, ref bool isBold, ref bool isItalic, ref bool isUnderline, ref bool isSuperscript)
        {
            List<Dictionary<string, string>> TEXT = new List<Dictionary<string, string>>();
            MatchCollection matches = Regex.Matches(formatstring, @"<w:r>(.*?)</w:r>");
            if (Regex.Match(formatstring, @"<w:rPr><w:b\/></w:rPr>").Success)
                isBold = true;
            if (Regex.Match(formatstring, @"<w:rPr><w:i\/></w:rPr>").Success)
                isItalic = true;
            if (Regex.Match(formatstring, @"<w:rPr><w:u.*?\/></w:rPr>").Success)
                isUnderline = true;
            if (Regex.Match(formatstring, @"<w:rPr><w:vertAlign.*?/></w:rPr>").Success)
                isSuperscript = true;
        }

        /// <summary>
        /// Parses the query string.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public static Dictionary<string, object> ParseQueryString(string url)
        {
            url = Regex.Replace(url, @"#.*?$", "");
            url = Regex.Replace(url, @"^\s+|[&|\s]+$", "");

            Dictionary<string, object> obj = new Dictionary<string, object>();
            if (Regex.Match(url, @"\?(.*?)(#|$)").Success)
            {
                MatchCollection match = Regex.Matches(url, @"\?(.*?)(#|$)");
                string[] split = Regex.Split(match[0].Value, @"=|&amp;|&");
                for (int i = 0; i < split.Length; i = i + 2)
                {
                    obj[split[i]] = HttpUtility.UrlDecode(split[i + 1]);
                }
            }
            return obj;
        }

        /// <summary>
        /// Gets the item hash.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static string GetItemHash(ItemMasterRow item)
        {
            return item.UserID + "_" + item.ItemID;
        }

        /// <summary>
        /// Gets the word ML format string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static string GetWordMLFormatString(string text, TextFormat format)
        {
            string formattedtext = string.Empty;
            switch (format)
            {
                case TextFormat.Bold:
                    formattedtext = "<w:r><w:rPr><w:b/></w:rPr><w:t>" + text + "</w:t></w:r>";
                    break;
                case TextFormat.Italic:
                    formattedtext = "<w:r><w:rPr><w:i/></w:rPr><w:t>" + text + "</w:t></w:r>";
                    break;
                case TextFormat.Underline:
                    formattedtext = "<w:r><w:rPr><w:u w:val=\"single\" /></w:rPr><w:t>" + text + "</w:t></w:r>";
                    break;
                case TextFormat.LineBreak:
                    formattedtext = "<w:r><w:br /></w:r>";
                    break;
                case TextFormat.Superscript:
                    formattedtext = "<w:r><w:rPr><w:vertAlign w:val=\"superscript\" /></w:rPr><w:t>" + text + "</w:t></w:r>";
                    break;
                case TextFormat.Paragraph:
                    formattedtext = "<w:p>" + text + "</w:p>";
                    break;
                case TextFormat.ParagraphWithIndent:
                    formattedtext = "<w:p><w:pPr><w:ind w:left=\"720\" w:first-line=\"1080\" w:hanging=\"720\" /></w:pPr>" + text + "</w:p>";
                    break;                    
                case TextFormat.Document:
                    formattedtext = "<?xml version=\"1.0\" encoding=\"utf-16\" ?><w:wordDocument  xml:space='preserve' xmlns:w=\"http://schemas.microsoft.com/office/word/2003/wordml\"><w:body>" + text + "</w:body></w:wordDocument>";
                    break;
                case TextFormat.InLineText:
                    formattedtext = "<?xml version=\"1.0\" encoding=\"utf-16\" ?><w:wordDocument  xml:space='preserve' xmlns:w=\"http://schemas.microsoft.com/office/word/2003/wordml\">" + text + "</w:wordDocument>";
                    break;
                case TextFormat.None:
                    formattedtext = "<w:r><w:t>" + text + "</w:t></w:r>";
                    break;
            }
            return formattedtext;
        }

        /// <summary>
        /// Gets the word ML format string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static string GetHtmlFormatString(string text, TextFormat format)
        {
            string formattedtext = string.Empty;
            switch (format)
            {
                case TextFormat.Bold:
                    formattedtext = "<b>" + text + "</b>";
                    break;
                case TextFormat.Italic:
                    formattedtext = "<i>" + text + "</i>";
                    break;
                case TextFormat.Underline:
                    formattedtext = "<u>" + text + "</u>";
                    break;
                case TextFormat.LineBreak:
                    formattedtext = "<br />";
                    break;
                case TextFormat.Superscript:
                    formattedtext = "<sup>" + text + "</sup>";
                    break;
                case TextFormat.Paragraph:
                    formattedtext = "<p>" + text + "</p>";
                    break;
                case TextFormat.None:
                    formattedtext = text;
                    break;
            }
            return formattedtext;
        }

        /// <summary>
        /// Gets the word ML format link.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="text">The text.</param>
        /// <param name="tooltip">The tooltip.</param>
        /// <returns></returns>
        public static string GetWordMLFormatLink(string url, string tooltip, string formatedstring)
        {
            string linkXML = "<w:hlink w:dest=\"{0}\" w:screenTip=\"{1}\"><w:r><w:rPr><w:rStyle w:val=\"Hyperlink\"/></w:rPr><w:t></w:t></w:r>{2}</w:hlink>";
            linkXML = string.Format(linkXML, purifyWordString(url), System.Security.SecurityElement.Escape(purifyWordString(tooltip)), formatedstring);
            return linkXML;
        }

        /// <summary>
        /// Formats the citation screen tip.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static string FormatCitationScreenTip(ItemMasterRow item)
        {
            string screentip = string.Empty;
            foreach (NameMasterRow name in item.Authors)
            {
                if (name.NameTypeID == NameTypes.Author)
                {
                    screentip += name.LastName;
                    break;
                }
            }
            if (item.PubYear.Length > 0 || item.PubDate.Length > 0 || item.IssueDate.Length > 0 || item.Date3.Length > 0)
            {
                string date = item.PubYear.Length > 0 ? item.PubYear : (item.PubDate.Length > 0 ? item.PubDate : (item.IssueDate.Length > 0 ? item.IssueDate : (item.Date3.Length > 0 ? item.Date3 : "")));
                screentip += date.Length > 0 ? ((screentip.Length > 0 ? ", " : "") + date) : "";
            }
            if (screentip.Length > 0)
                screentip = "(" + screentip + ")";

            if (item.Title.Length > 0)
                screentip = (item.Title.Length > Properties.Settings.Default.DEFAULT_SCREENTIP_TITLE_WORDLIMIT ? (item.Title.Substring(0, Properties.Settings.Default.DEFAULT_SCREENTIP_TITLE_WORDLIMIT) + "...") : item.Title) + "\r\n" + screentip;

            return screentip;
        }

        /// <summary>
        /// Gets the plain format.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="bInHtmlFormat">if set to <c>true</c> [b in HTML format].</param>
        /// <returns></returns>
        public static string GetPlainFormat(string text, bool bInHtmlFormat, bool bIsSuperscipt)
        {
            if (!bInHtmlFormat)
                text = GetWordMLFormatString(text, bIsSuperscipt ? TextFormat.Superscript : TextFormat.None);
            else if (bIsSuperscipt)
                text = GetHtmlFormatString(text, TextFormat.Superscript);

            return text;
        }

        /// <summary>
        /// Formats the author.
        /// </summary>
        /// <param name="authors">The authors.</param>
        /// <param name="nametype">The nametype.</param>
        /// <returns></returns>
        public static string FormatAuthor(List<NameMasterRow> authors, Author authorTemplate, NameTypes nametype, bool bIsInHtmlFormat)
        {
            string text = string.Empty;
            string strFollowedBy = string.Empty;
            List<NameMasterRow> listAuthors = new List<NameMasterRow>();
            foreach (NameMasterRow author in authors)
            {
                if (author.NameTypeID == nametype)
                {
                    if(author.LastName.Length > 0)
                        listAuthors.Add(author);
                }
            }
            int count = listAuthors.Count;
            if (count >= authorTemplate.MaxAuthors)
            {
                if (count > authorTemplate.ListAuthors)
                    count = authorTemplate.ListAuthors;
            }
            for (int i = 0; i < count; i++)
            {
                AuthorFormat formatIndex = authorTemplate.OtherAuthors;
                if (i == 0)
                    formatIndex = authorTemplate.FirstAuthor;
                if (text.Length > 0)
                {
                    if (i < count - 1) 
                        text += CitationTools.GetAuthorDelimitor(authorTemplate.BetweenAuthors);
                    else
                        text += authorTemplate.BeforeLast;
                }
                string authorname = CitationTools.FormatName(listAuthors[i], formatIndex);
                switch (authorTemplate.Capitalization)
                {
                    case Capitalization.AsIs:
                        break;
                    case Capitalization.FirstIsCapital:
                        authorname = authorname.Substring(0, 1).ToUpper() + authorname.Substring(1).ToLower();
                        break;
                    case Capitalization.AllCapital:
                        authorname = authorname.ToUpper();
                        break;
                }

                text += authorname.Replace("<", "&lt;").Replace(">", "&gt;");                
            }
            if (listAuthors.Count >= authorTemplate.MaxAuthors)
            {
                if (authorTemplate.FollowedBy_Italic)
                {
                    if(bIsInHtmlFormat)
                        text += "<i>" + authorTemplate.FollowedBy + "</i>";
                    else
                        strFollowedBy = GetWordMLFormatString(authorTemplate.FollowedBy, TextFormat.Italic);
                }
                else
                    text += authorTemplate.FollowedBy;
            }
            text = ApplyStandardFormat(authorTemplate, text, bIsInHtmlFormat, strFollowedBy, false);
            return text;
        }

        /// <summary>
        /// Formats the author.
        /// </summary>
        /// <param name="authors">The authors.</param>
        /// <param name="authorTemplate">The author template.</param>
        /// <param name="nametype">The nametype.</param>
        /// <returns></returns>
        public static string FormatPublisher(List<NameMasterRow> authors, Publisher publisherTemplate, NameTypes nametype, bool bInHtmlFormat)
        {
            string text = string.Empty;
            List<NameMasterRow> listAuthors = new List<NameMasterRow>();
            foreach (NameMasterRow author in authors)
            {
                if (author.NameTypeID == nametype)
                    listAuthors.Add(author);
            }
            int count = listAuthors.Count;
            for (int i = 0; i < count; i++)
            {
                text += listAuthors[i].LastName;
            }
            text = ApplyStandardFormat(publisherTemplate, text, bInHtmlFormat);
            return text;
        }

        /// <summary>
        /// Formates the date.
        /// </summary>
        /// <param name="strDate">The STR date.</param>
        /// <param name="fieldDate">The field date.</param>
        /// <param name="bIsInHtmlFormat">if set to <c>true</c> [b is in HTML format].</param>
        /// <returns></returns>
        public static string FormateDate(string strDate, DateClass fieldDate, bool bIsInHtmlFormat)
        {
            string pubdate = strDate;
            string text = string.Empty;
            if (pubdate.Length > 0)
            {
                switch (fieldDate.YearFormat)
                {
                    default:
                    case YearFormats.FourDigits:
                        if (Regex.Match(pubdate, @"\d{4}").Success)
                        {
                            text = Regex.Match(pubdate, @"\d{4}").Value;
                        }
                        break;
                    case YearFormats.TwoDigits:
                        if (Regex.Match(pubdate, @"\d{4}").Success)
                        {
                            text = Regex.Match(pubdate, @"\d{4}").Value.Substring(2, 2);
                        }
                        break;
                    case YearFormats.ApostropheTwoDigits:
                        if (Regex.Match(pubdate, @"\d{4}").Success)
                        {
                            text = "'" + Regex.Match(pubdate, @"\d{4}").Value.Substring(2, 2);
                        }
                        break;
                }
                text = ApplyStandardFormat(fieldDate, text, bIsInHtmlFormat);
            }
            return text;
        }

        public static string FormateFullDate(string strDate, DateClass fieldDate, bool bIsInHtmlFormat)
        {
            string pubdate = strDate;
            string text = string.Empty;
            if (pubdate.Length > 0)
            {
                text = ApplyStandardFormat(fieldDate, pubdate, bIsInHtmlFormat);
            }
            return text;
        }

        /// <summary>
        /// Gets the item file icon.
        /// </summary>
        /// <param name="strItemFileName">Name of the STR item file.</param>
        /// <returns></returns>
        public static Image GetItemFileIcon(string strItemFileName)
        {
            Image img = null;
            if (Regex.Match(strItemFileName, @".(\w{0,5})$").Success)
            {
                string temp = Regex.Match(strItemFileName, @"\.(\w{0,5})$").Value;
                temp = Regex.Replace(temp, @"\.|_$", "");
                switch (temp.ToLower())
                {
                    case "pdf":
                        img = Properties.Resources.pdf;
                        break;
                    case "doc":
                    case "rtf":
                    case "docx":
                        img = Properties.Resources.word16x16;
                        break;
                    case "xls":
                    case "xlsx":
                        img = Properties.Resources.spreadsheet;
                        break;
                    case "ppt":
                    case "pptx":
                        img = Properties.Resources.powerpoint16x16;
                        break;
                    case "txt":
                        img = Properties.Resources.text16x16;
                        break;
                    case "htm":
                    case "mht":
                    case "html":
                    case "aspx":
                        img = Properties.Resources.html;
                        break;
                    case "mp2":
                    case "mp3":                   
                        img = Properties.Resources.midi;
                        break;
                    case "mp4":
                    case "avi":
                    case "mpeg":
                    case "mkv":
                        img = Properties.Resources.video;
                        break;
                    case "jpg":
                    case "gif":
                    case "png":
                    case "tiff":
                    case "jpge":
                        img = Properties.Resources.image;
                        break;
                    case "wfweb":
                        img = Properties.Resources.link_icon;
                        break;
                    default:
                        img = Properties.Resources.UnknownFile16x16;
                        break;
                }
            }
            return img;
        }

        /// <summary>
        /// Gets the authors string.
        /// </summary>
        /// <param name="authors">The authors.</param>
        /// <returns></returns>
        public static string GetAuthorsString(List<NameMasterRow> authors)
        {
            string strAuthors = string.Empty;
            int count = 0;
            foreach (NameMasterRow author in authors)
            {
                count++;
                if (author.LastName.Length > 0)
                {
                    string extra = string.Empty;
                    if (author.NameTypeID == NameTypes.Editor)
                        extra = " (Editor)";
                    else if (author.NameTypeID == NameTypes.Publisher)
                        extra = " (Publisher)";
                    strAuthors += (author.ForeName.Length > 0 ? (author.ForeName + " ") : "" ) + author.LastName + extra;
                    if (count < authors.Count)
                        strAuthors += ", ";
                }
            }
            return strAuthors;
        }
        public static string GetAuthorsSortableString(List<NameMasterRow> authors)
        {
            string strAuthors = string.Empty;
            foreach (NameMasterRow author in authors)
            {
                strAuthors += author.LastName + "," + author.ForeName + "|";
            }
            return strAuthors;
        }

        /// <summary>
        /// Executes the application.
        /// </summary>
        /// <param name="obj">The obj.</param>
        public static void ExecuteApplication(string obj)
        {
            System.Diagnostics.ProcessStartInfo procFormsBuilderStartInfo = new System.Diagnostics.ProcessStartInfo();
            procFormsBuilderStartInfo.FileName = obj;
            procFormsBuilderStartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            System.Diagnostics.Process procFormsBuilder = new System.Diagnostics.Process();
            procFormsBuilder.StartInfo = procFormsBuilderStartInfo;
            procFormsBuilder.Start();
        }
    }
}
