using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSWordpiCite.Classes
{
    public enum SortByType
    {
        Title = 0,
        Author = 1,
        Year = 2,
        ItemType = 3,
        JournalName = 4,
        None
    }

    public enum FilterFields
    {
        Title = 1,
        Title2 = 2,
        Authors = 4,
        PubDate = 8,
        Abstract = 16,
        Notes = 32,
        Tags = 64,
        Keywords = 128
    }

    public enum FormatType
    {
        Number = 0,
        Year,
        Author,
        AuthorYear
    }

    public enum EnclosureType
    {
        /// <summary>
        /// 
        /// </summary>
        None = 0,

        /// <summary>
        /// (...)
        /// </summary>
        RoundBracket,

        /// <summary>
        /// [...]
        /// </summary>
        SquareBracket,

        /// <summary>
        /// {...}
        /// </summary>
        AngleBracket
    }

    public enum AuthorFormat
    {
        /// <summary>
        /// William Jefferson Clinton (0)
        /// </summary>
        FullNameFirstNameFirst = 0,

        /// <summary>
        /// W. J. Clinton (1)
        /// </summary>
        InitialWithDot,

        /// <summary>
        /// W.J. Clinton (2)
        /// </summary>
        InitialWithDotWithoutSpace,

        /// <summary>
        /// W J Clinton (3)
        /// </summary>
        InitialWithoutDot,

        /// <summary>
        /// WJ Clinton (4)
        /// </summary>
        InitialWithoutDotWithoutSpace,

        /// <summary>
        /// Clinton, William Jefferson (5)
        /// </summary>
        FullNameLastNameFirstWithComma,

        /// <summary>
        /// Clinton, W. J. (6)
        /// </summary>
        InitialLastNameFirstWithCommaWithDot,

        /// <summary>
        /// Clinton, W.J. (7)
        /// </summary>
        InitialLastNameFirstWithCommaWithDotWithoutSpace,

        /// <summary>
        /// Clinton, W J (8)
        /// </summary>
        InitialLastNameFirstWithCommaWithoutDot,

        /// <summary>
        /// Clinton, WJ (9)
        /// </summary>
        InitialLastNameFirstWithCommaWithoutDotWithoutSpace,

        /// <summary>
        /// Clinton (0)
        /// </summary>
        LastNameOnly,

        /// <summary>
        /// Clinton W. J. (10)
        /// </summary>
        InitialLastNameFirstWithoutCommaWithDot,

        /// <summary>
        /// Clinton W.J. (11)
        /// </summary>
        InitialLastNameFirstWithoutCommaWithDotWithoutSpace,

        /// <summary>
        /// Clinton W J (12)
        /// </summary>
        InitialLastNameFirstWithoutCommaWithoutDot,

        /// <summary>
        /// Clinton WJ (13)
        /// </summary>
        InitialLastNameFirstWithoutCommaWithoutDotWithoutSpace,

        /// <summary>
        /// William J. Clinton (14)
        /// </summary>
        FirstNameFirstMidNameInitialWithDot,

        /// <summary>
        /// Clinton,W. J. (15)
        /// </summary>
        InitialLastNameFirstWithCommaWithDotWithoutSpaceAfterComma,

        /// <summary>
        /// Clinton,W.J. (16)
        /// </summary>
        InitialLastNameFirstWithCommaWithDotWithoutAnySpace
    }

    public enum AuthorDelimitor
    {
        None = 0,
        Semicolon,
        Comma
    }

    public enum Capitalization
    {
        AsIs = 0,
        FirstIsCapital,
        AllCapital,
        Sup
    }

    public enum PageNumberFormat
    {
        AsIs = 0,
        FirstPageOnly,
        AbbrLastPageOneDigit,
        AbbrLastPageTwoDigits,         
        Full,
        FirstPageOnlyForJournals,
        LastPageOnly
    }

    public enum JournalNameFormat
    {
        AsIs = 0,
        Abbreviation
    }

    public enum YearFormats
    {
        FourDigits = 0,
        TwoDigits,
        ApostropheTwoDigits,
        AsIs
    }

    public enum SortOrders
    {
        Occurence = 0,
        AuthorYearTitle,
        AuthorTitle
    }

    public enum TextFormat
    {
        Bold = 0,
        Italic,
        Underline,
        Paragraph,
        ParagraphWithIndent,
        LineBreak,
        Superscript,
        InLineText,
        Document,
        None
    }

    public enum StyleOwner
    {
        User = 0,
        Public
    }
}
