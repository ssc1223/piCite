using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSWordpiCite.Enums
{
    [Serializable]
    public enum ItemTypes
    {
        Document = 1,
        BookWhole,
        BookChapter,
        JournalArticle,
        Proceeding,
        Thesis,
        LectureNote,
        Patent,
        Audio,
        Video,
        WebPage
    }

    [Serializable]
    public enum NameTypes
    {
        Author = 1,
        Editor,
        Translator,
        Publisher,
        Conference,
        Institution
    }

    [Serializable]
    public enum Flags
    {
        Clear = 0,
        Red,
        Blue,
        Yellow,
        Green,
        Orange,
        Purple
    }

    public enum AccountType
    {
        Free = 0,
        Personal = 1,
        Academic = 2
    }

    public enum FolderType
    {
        Normal = 1,
        ALL = 2,
        MyPublication = 4,
        Trash = 8,
        Shared = 16,
        Search = 32,
        PublicCollections = 64,
        Collection = 128,
        SubscribedCollection = 256,
        SubscribedProfile = 512,
        SubscribedCollectionCategory = 1024,
        PubmedNotification = 2048
    }

    public enum DeviceTypes
    {
        Iphone = 0,
        Ipad,
        PC,
        MSWordWin
    }

    public enum SearchOptions
    {
        Filter = 0,
        PubMed,
        ScholarsPortal,
        CiteULike,
        WorldCat,
        GoogleScholar,
        WizFolioSearch
    }
}