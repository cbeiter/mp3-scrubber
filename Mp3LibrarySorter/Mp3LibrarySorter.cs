using System.Collections.Generic;
using System.IO;

namespace Mp3LibrarySorter
{
    public class Mp3LibrarySorter
    {
        private readonly IFileSystem _fileSystem;
        private readonly string _startDirectory;
        private readonly IMp3TagsHierarchy _mp3TagsHierarchy;
        private readonly IMp3FileReader _mp3FileReader;
        private readonly IList<IMp3Representation> _mp3Files;

        public Mp3LibrarySorter(IFileSystem fileSystem, string startDirectory, IMp3TagsHierarchy mp3TagsHierarchy, IMp3FileReader mp3FileReader)
        {
            _fileSystem = fileSystem;
            _startDirectory = startDirectory;
            _mp3TagsHierarchy = mp3TagsHierarchy;
            _mp3FileReader = mp3FileReader;

            var files = _fileSystem.GetAllMp3Files(startDirectory);
            _mp3Files = _mp3FileReader.RetrieveTagsFromMp3Files(files);
            foreach (var mp3Representation in _mp3Files)
            {
                _mp3TagsHierarchy.AddInformation(mp3Representation);
            }
        }

        public void CreateFoldersForArtists()
        {
            foreach (var artist in _mp3TagsHierarchy.Artists)
            {
                string artistDirectoryName = _startDirectory + Path.DirectorySeparatorChar + artist;
                _fileSystem.CreateDirectory(artistDirectoryName);
                foreach (var album in _mp3TagsHierarchy.GetAlbumsForArtist(artist))
                {
                    var albumFolder = artistDirectoryName + Path.DirectorySeparatorChar + album;
                    _fileSystem.CreateDirectory(albumFolder);
                    var files = _mp3TagsHierarchy.GetSongsForAlbumOfArtist(album, artist);
                    foreach (var file in files)
                    {
                        var fileNameWithoutFullPath = Path.GetFileName(file);
                        _fileSystem.Move(file, albumFolder + Path.DirectorySeparatorChar + fileNameWithoutFullPath);
                    }
                }
            }
        }
    }
}