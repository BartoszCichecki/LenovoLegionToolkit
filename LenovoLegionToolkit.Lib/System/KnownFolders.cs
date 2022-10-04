using System;
using System.Collections.Generic;
using Vanara.PInvoke;

namespace LenovoLegionToolkit.Lib.System
{
    public static class KnownFolders
    {
        private static readonly Dictionary<KnownFolder, Guid> Guids = new()
        {
            [KnownFolder.Contacts] = new("56784854-C6CB-462B-8169-88E350ACB882"),
            [KnownFolder.Downloads] = new("374DE290-123F-4565-9164-39C4925E467B"),
            [KnownFolder.Favorites] = new("1777F761-68AD-4D8A-87BD-30B759FA33DD"),
            [KnownFolder.Links] = new("BFB9D5E0-C6A9-404C-B2B2-AE6DB6AF4968"),
            [KnownFolder.SavedGames] = new("4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4"),
            [KnownFolder.SavedSearches] = new("7D1D3A04-DEBB-4115-95CF-2F29DA2920DA")
        };

        public static string GetPath(KnownFolder knownFolder)
        {
            return Shell32.SHGetKnownFolderPath(Guids[knownFolder], Shell32.KNOWN_FOLDER_FLAG.KF_FLAG_DEFAULT, HTOKEN.NULL, out var path) == HRESULT.S_OK ? path : string.Empty;
        }
    }
}
