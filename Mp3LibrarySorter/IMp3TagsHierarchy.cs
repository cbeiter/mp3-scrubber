using System.Collections.Generic;
using System.Linq;

namespace Mp3LibrarySorter
{
    public interface IMp3TagsHierarchy
    {
        void AddInformation(IMp3Representation mp3Representation);
        IList<string> Artists { get; }
        IList<string> GetAlbumsForArtist(string artistName);
        IList<string> GetSongsForAlbumOfArtist(string albumName, string artistName);
    }

    public class Mp3TagsHierarchy : IMp3TagsHierarchy
    {
        private readonly Dictionary<string, Dictionary<string, List<string>>> _artistAlbumSongs;

        public Mp3TagsHierarchy()
        {
            _artistAlbumSongs = new Dictionary<string, Dictionary<string, List<string>>>();
        }

        public void AddInformation(IMp3Representation mp3Representation)
        {
            var artist = mp3Representation.ArtistName;
            var albumName = mp3Representation.AlbumName;
            var song = mp3Representation.FileName;

            if (_artistAlbumSongs.ContainsKey(artist))
            {
                var albumSongs = _artistAlbumSongs[artist];
                if (albumSongs.ContainsKey(albumName))
                {
                    albumSongs[albumName].Add(song);
                }
                else
                {
                    albumSongs[albumName] = new List<string>{song};
                }
            }
            else
            {
                _artistAlbumSongs[artist] = new Dictionary<string, List<string>>();
                _artistAlbumSongs[artist][albumName] = new List<string>{song};
            }
        }
    
        public IList<string> Artists
        {
            get
            {
                return _artistAlbumSongs.Keys.ToList();
            }
        }

        public IList<string> GetAlbumsForArtist(string artistName)
        {
            var result = new List<string>();
            if (_artistAlbumSongs.ContainsKey(artistName))
            {
                var albumSongs = _artistAlbumSongs[artistName];
                result.AddRange(albumSongs.Keys);
            }
            return result;
        }

        public IList<string> GetSongsForAlbumOfArtist(string albumName, string artistName)
        {
            var albumSongs = _artistAlbumSongs[artistName];
            return albumSongs[albumName];
        }
    }
}