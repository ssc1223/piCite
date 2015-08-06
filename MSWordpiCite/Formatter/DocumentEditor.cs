using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Word = Microsoft.Office.Interop.Word;
using MSWordpiCite.Classes;
using System.Text.RegularExpressions;
using MSWordpiCite.Tools;
using System.Windows;

namespace MSWordpiCite.Formatter
{
    class DocumentEditor
    {
        #region Variables

        private Word.Document document = Globals.ThisAddIn.Application.ActiveDocument;
        private Word.Window window = Globals.ThisAddIn.Application.ActiveWindow;
        private Word.Selection selection = Globals.ThisAddIn.Application.ActiveWindow.Selection;
        private Logger log = Globals.ThisAddIn.log;

        #endregion

        public DocumentEditor()
        { }

        #region Public Functions

        /// <summary>
        /// Get number of Links in the document
        /// </summary>
        /// <returns>Number of Links</returns>
        public int GetLinkCount()
        {
            return document.Hyperlinks.Count;
        }

        /// <summary>
        /// Gets the WordHyperLink
        /// </summary>
        /// <param name="index">index of the hyperlink</param>
        /// <returns>WordHyperLink</returns>
        public WordHyperLink GetLinkAt(int index)
        {
            WordHyperLink link = null;
            try
            {                
                Word.Hyperlinks listLinks = document.Hyperlinks;
                if (index > 0 && index <= listLinks.Count)
                {
                    object oindex = index;
                    Word.Hyperlink wlink = listLinks.get_Item(ref oindex);
                    link = new WordHyperLink(wlink.Range.Start, wlink.Range.End, wlink.Address);
                }                
            }
            catch(Exception ex)
            {
                log.WriteLine(LogType.Error, "DocumentEditor::GetLinkAt", ex.ToString());
            }
            return link;
        }

        /// <summary>
        /// Gets the text in the range of this Word document
        /// </summary>
        /// <param name="end">End position</param>
        /// <param name="start">Start position</param>
        /// <returns>Text in provided range</returns>
        public string GetTextInRange(long end, long start)
        {            
            object oend = end;
            object ostart = start;
            return document.Range(ref oend, ref ostart).Text;
        }

        /// <summary>
        /// Removes the hyper link.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveHyperLink(int index)
        {
            try
            {
                Word.Hyperlinks listLinks = document.Hyperlinks;
                if (index > 0 && index <= listLinks.Count)
                {
                    object oindex = index;
                    Word.Hyperlink link = listLinks.get_Item(ref oindex);
                    link.Delete();
                }
            }
            catch(Exception ex)
            {
                log.WriteLine(LogType.Error, "DocumentEditor::RemoveHyperLink", ex.ToString());
            }
        }

        /// <summary>
        /// Removes the citation hyperlink
        /// </summary>
        /// <param name="index">Citation hyperlink index</param>
        public void RemoveCitation(int index)
        {
            try
            {
                Word.Hyperlinks listLinks = document.Hyperlinks;
                object missing = System.Reflection.Missing.Value;
                if (index > 0 && index <= listLinks.Count)
                {
                    object oindex = index;
                    Word.Hyperlink link = listLinks.get_Item(ref oindex);
                    link.Range.Delete(ref missing, ref missing);
                }
            }
            catch (Exception ex)
            {
                log.WriteLine(LogType.Error, "DocumentEditor::RemoveCitation", ex.ToString());
            }
        }

        /// <summary>
        /// Jumps to next matching citation
        /// </summary>
        /// <param name="index">Citation link index</param>
        public void JumpToNextMatchingItem(int index)
        {
            try
            {
                Word.Hyperlinks listLinks = document.Hyperlinks;
                object missing = System.Reflection.Missing.Value;
                if (index > 0 && index <= listLinks.Count)
                {
                    object oindex = index;
                    Word.Hyperlink link = listLinks.get_Item(ref oindex);
                    object collapseStart = Word.WdCollapseDirection.wdCollapseStart;
                    if (link.Range.Font.Hidden == 0)
                    {
                        link.Range.Select();
                        link.Range.Collapse(ref collapseStart);
                    }
                    else
                    {
                        object ostart = link.Range.Start;
                        object oend = link.Range.End + 1;
                        Word.Range target_range = document.Range(ref ostart, ref oend);
                        while (target_range.Font.Hidden != 0)
                        {
                            ostart = (int)ostart + 1;
                            oend = (int)oend + 1;
                            target_range = document.Range(ref ostart, ref oend);
                        }
                        target_range.Select();
                        target_range.Collapse(ref collapseStart);
                    }
                }
            }
            catch(Exception ex)
            {
                log.WriteLine(LogType.Error, "DocumentEditor::JumpToNextMatchingItem", ex.ToString());
            }
        }
        
        /// <summary>
        /// Mark the citation hyperlink which system cannot find the bibliographic data
        /// </summary>
        /// <param name="index">Index of the hyperlink</param>
        /// <param name="text">Text to warn the missing citation</param>
        public void MissingCitation(int index, string text)
        {
            try
            {
                if (index < 1 || index > document.Hyperlinks.Count)
                    return;
                object missing = System.Reflection.Missing.Value;
                object oindex = index;
                Word.Hyperlink link = document.Hyperlinks.get_Item(ref oindex);
                object ostart = link.Range.Start;
                string prevtext = link.TextToDisplay;
                link.Range.Delete(ref missing, ref missing);
                Word.Range range = document.Range(ref ostart, ref ostart);
                range.InsertAfter(prevtext + " " + text);
                range.Font.Color = Word.WdColor.wdColorRed;
                range.Font.Underline = Word.WdUnderline.wdUnderlineNone;
            }
            catch(Exception ex)
            {
                log.WriteLine(LogType.Error, "DocumentEditor::MissingCitation", ex.ToString());
            }            
        }

        /// <summary>
        /// Inserts the citation hyperlink as a place holder (Pre-formatted)
        /// </summary>
        /// <param name="text">Text to represent the citation</param>
        /// <param name="url">URL of the hyperlink</param>
        public void InsertCitation(string text, string url)
        {
            try
            {
                object missing = System.Reflection.Missing.Value;
                Word.Range range1 = Globals.ThisAddIn.Application.ActiveWindow.Selection.Range;
                object c1 = range1.End;
                range1 = document.Range(ref c1, ref c1);
                while (range1.Hyperlinks.Count > 0)
                {
                    object index_v1 = 1;
                    Word.Hyperlink link1 = range1.Hyperlinks.get_Item(ref index_v1);
                    object c2 = range1.End + 1;
                    range1 = document.Range(ref c2, ref c2);
                }
                range1.InsertAfter(text);                
                object c3 = url;
                Word.Hyperlink link = document.Hyperlinks.Add(range1, ref c3, ref missing, ref missing, ref missing, ref missing);
                range1.Font.Underline = Microsoft.Office.Interop.Word.WdUnderline.wdUnderlineNone;
                range1.Font.Color = selection.Font.Color;
                selection.SetRange(link.Range.End, link.Range.End);
                document.ActiveWindow.SetFocus();
            }
            catch(Exception ex)
            {
                log.WriteLine(LogType.Error, "DocumentEditor::InsertCitation", ex.ToString());
            }            
        }
                
        /// <summary>
        /// Update the list of cited references
        /// </summary>
        /// <param name="index">Index of previous Citation Style Hyperlink (if avaiable)</param>
        /// <param name="formatstring">List of references in WordML format</param>
        /// <param name="url">the new Citation Style Hyperlink</param>
        public void UpdateReferences(int index, string formatstring, string url)
        {
            try
            {
                Word.Range currRange = selection.Range;
                formatstring = CitationTools.GetWordMLFormatString(formatstring, TextFormat.Document);
                object missing = System.Reflection.Missing.Value;
                object okey = Word.WdBreakType.wdLineBreak;
                Word.Window activeWindow = Globals.ThisAddIn.Application.ActiveWindow;
                Word.Range range = null;
                if (index < 0)
                {
                    document.Select();
                    object c1 = Word.WdUnits.wdCharacter;
                    object c2 = activeWindow.Selection.End;
                    activeWindow.Selection.MoveStart(ref c1, ref c2);
                    activeWindow.Selection.InsertBreak(ref okey);
                    activeWindow.Selection.InsertBreak(ref okey);
                    range = activeWindow.Selection.Range;
                    range.InsertBreak(ref okey);
                }
                else
                {
                    object oindex = index;
                    Word.Hyperlink link = document.Hyperlinks.get_Item(ref oindex);
                    document.Select();
                    object c1 = Word.WdUnits.wdCharacter;
                    object c2 = activeWindow.Selection.End;
                    activeWindow.Selection.MoveStart(ref c1, ref c2);
                    object c3 = link.Range.Start;
                    object c4 = selection.End;
                    document.Range(ref c3, ref c4).Delete(ref missing, ref missing);
                    activeWindow.Selection.InsertBreak(ref okey);
                    c3 = selection.End;
                    range = document.Range(ref c3, ref c3);
                }
                object oend = range.End;
                Word.Range range2 = document.Range(ref oend, ref oend);
                range.SetRange(range.Start, range.Start);
                range.InsertXML(formatstring, ref missing);
                range.SetRange(range.Start, range2.Start - 1);
                object olink = url;
                string linkXML = CitationTools.GetWordMLFormatString(CitationTools.GetWordMLFormatString(CitationTools.GetWordMLFormatLink(url, "", "References"), TextFormat.Paragraph), TextFormat.Document);
                range.InsertXML(linkXML, ref missing);
                Word.Hyperlink reflink = document.Hyperlinks.Add(range, ref olink, ref missing, ref missing, ref missing, ref missing);
                reflink.TextToDisplay = "References";
                reflink.Range.Font.Bold = 1;
                reflink.Range.Font.Underline = Microsoft.Office.Interop.Word.WdUnderline.wdUnderlineNone;
                reflink.Range.Font.Color = selection.Font.Color;
                selection.SetRange(currRange.Start, currRange.End);
                
                activeWindow.SetFocus();
            }
            catch(Exception ex)
            {
                log.WriteLine(LogType.Error, "DocumentEditor::UpdateReferences", ex.ToString());
            }            
        }
        
        /// <summary>
        /// Update hyperlink
        /// </summary>
        /// <param name="index">Index of the hyperlink</param>
        /// <param name="formatstring">[1], (1) or (Author, 2010) in WordML format</param>
        /// <param name="url">link to citation</param>
        /// <param name="screentip">Tooltip when mouseover the link -> SOMETIMES IT DOES NOT WORK !!??</param>
        public void UpdateLink(int index, string formatstring, string url, string screentip)
        {
            try
            {
                if (index < 1 || index > document.Hyperlinks.Count)
                    return;
                object missing = System.Reflection.Missing.Value;
                bool bInRange = false;
                
                object oindex = index;
                Word.Hyperlink link = document.Hyperlinks.get_Item(ref oindex);
                if (selection.Range.End == link.Range.End)
                    bInRange = true;
                string text = CitationTools.GetRawText(formatstring);
                if (text.Length == 0)
                {
                    link.TextToDisplay = @" ";
                    link.Range.Font.Hidden = 1;
                }
                else
                {
                    string linkXML = CitationTools.GetWordMLFormatString(CitationTools.GetWordMLFormatString(CitationTools.GetWordMLFormatLink(url, screentip, formatstring), TextFormat.Paragraph), TextFormat.Document);
                    object c1 = link.Range.Start;
                    object c2 = link.Range.End;
                    int end = link.Range.End;
                    Word.Range r1 = document.Range(ref c1, ref c2);
                    Word.Range r2 = document.Range(ref c2, ref c2);
                    r1.InsertXML(linkXML, ref missing);
                    if(bInRange)
                    {
                        object c3 = Word.WdUnits.wdCharacter;
                        object c4 = -1;
                        r2.MoveStart(ref c3, ref c4);
                        selection.SetRange(link.Range.End, link.Range.End);
                    }

                    //須加入判斷Office版本資訊，因insertxml function在2007,2010,2013版本會有\r動作。
                    r1.MoveUntil("\r");
                    r1.Delete(1, 1);
                    System.Diagnostics.Debug.WriteLine("Hello");

                    document.ActiveWindow.SetFocus();
                }
            }
            catch (Exception ex)
            {
                log.WriteLine(LogType.Error, "DocumentEditor::UpdateLink", ex.ToString());
            }
        }

        #endregion
    }
}
