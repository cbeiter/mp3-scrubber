using System.Collections.Generic;

namespace Mp3LibrarySorter
{
    public interface IMp3TagsHierarchy
    {
        void AddInformation(IMp3Representation mp3Representation);
        IList<string> Artists { get; }
        IList<string> GetAlbumsForArtist(string artistName);
        IList<string> GetSongsForAlbumOfArtist(string albumName, string artistName);
    }
}