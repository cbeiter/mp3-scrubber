using System;
using System.Collections.Generic;
using System.IO;
using Raize.CodeSiteLogging;
using File = TagLib.File;

namespace Mp3LibrarySorter
{
    public class Mp3FileReader : IMp3FileReader
    {
        private readonly IList<string> _filesWithMissingTags = new List<string>(); 

        public IList<IMp3Representation> RetrieveTagsFromMp3Files(IList<string> mp3Files)
        {
            var result = new List<IMp3Representation>();
            var count = 0;
            foreach (var mp3File in mp3Files)
            {
                string extension = Path.GetExtension(mp3File);
                if ((extension != ".cue") && (extension != ".db"))
                try
                {
                    CodeSite.Send("Processing file " + count++ + " from " + mp3Files.Count);
                    File tagLibFile = null;
                    try
                    {
                        tagLibFile = File.Create(mp3File);
                        _filesWithMissingTags.Add(mp3File);
                    }
                    catch (Exception exception)
                    {
                        CodeSite.Send(mp3File);
                        CodeSite.SendException(exception);
                        tagLibFile = null;
                    }
                    if (tagLibFile != null)
                    {
                        string artist = "";
                        if (tagLibFile.Tag.AlbumArtists.Length > 0)
                            artist = tagLibFile.Tag.AlbumArtists[0];
                        else if (tagLibFile.Tag.Artists.Length > 0)
                            artist = tagLibFile.Tag.Artists[0];

                        string album = "unknown";
                        if ((tagLibFile.Tag != null) && (tagLibFile.Tag.Album != null))
                        {
                            if (tagLibFile.Tag.Album.Length > 0)
                                album = tagLibFile.Tag.Album;
                            else if (tagLibFile.Tag.AlbumArtists.Length > 0)
                                album = tagLibFile.Tag.AlbumArtists[0];
                            if (artist != string.Empty)
                            {
                                var mp3Representation1 = new Mp3Representation()
                                                             {
                                                                 AlbumName = album,
                                                                 ArtistName = artist,
                                                                 FileName = mp3File
                                                             };
                                result.Add(mp3Representation1);
                            }
                        } else {
                            _filesWithMissingTags.Add(mp3File);
                        }
                    }
                    else {
                        _filesWithMissingTags.Add(mp3File);
                    }
                } catch (Exception ex) {
                    CodeSite.Send(mp3File);
                    CodeSite.SendException(ex);
                    _filesWithMissingTags.Add(mp3File);
                }
               
            }
            return result;
        }

        public string GetArtistFromTagFile(TagLib.File mp3File)
        {
            string artist = "";
            if (mp3File.Tag.AlbumArtists.Length > 0)
                artist = mp3File.Tag.AlbumArtists[0];
            else if (mp3File.Tag.Artists.Length > 0)
                artist = mp3File.Tag.Artists[0];

            return artist;
        }

        public IList<string> FilesWithNoTags
        {
            get { return _filesWithMissingTags; }
        }
    }
}