using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mp3LibrarySorter
{
    public class Mp3FileReader : IMp3FileReader
    {
        private readonly IList<string> _filesWithMissingTags = new List<string>(); 

        public IList<IMp3Representation> RetrieveTagsFromMp3Files(IList<string> mp3Files)
        {
            var result = new List<IMp3Representation>();
            foreach (var file in mp3Files)
            {
                using (var fs = File.OpenRead(file))
                {
                    var br = new BinaryReader(fs);
                    if (fs.Length >= 128)
                    {
                        var tag = new MusicID3Tag();
                        fs.Seek(-128, SeekOrigin.End);
                        fs.Read(tag.TAGID, 0, tag.TAGID.Length);
                        fs.Read(tag.Title, 0, tag.Title.Length);
                        fs.Read(tag.Artist, 0, tag.Artist.Length);
                        fs.Read(tag.Album, 0, tag.Album.Length);
                        fs.Read(tag.Year, 0, tag.Year.Length);
                        fs.Read(tag.Comment, 0, tag.Comment.Length);
                        fs.Read(tag.Genre, 0, tag.Genre.Length);
                        var theTAGID = Encoding.Default.GetString(tag.TAGID);

                        if (theTAGID.Equals("TAG"))
                        {
                            var artist = Encoding.Default.GetString(tag.Artist).Trim();
                            var album = Encoding.Default.GetString(tag.Album).Trim(new[]{'\0', ':', ';', '.', '?', '!', '"', '*', '/', '\\'});
                            var mp3Representation = new Mp3Representation()
                                                        {
                                                            AlbumName = album,
                                                            ArtistName = artist,
                                                            FileName = file
                                                        };
                            result.Add(mp3Representation);

                            //string Year = Encoding.Default.GetString(tag.Year);
                            //string Comment = Encoding.Default.GetString(tag.Comment);
                            //string Genre = Encoding.Default.GetString(tag.Genre);
                        }
                        else if (theTAGID.Equals("ID3"))
                        {
                            var b = true; 
                        }
                        else
                        {
                            _filesWithMissingTags.Add(file);
                        }
                    }
                }
            }
            return result;
        }

        public IList<string> FilesWithNoTags
        {
            get { return _filesWithMissingTags; }
        }
    }
}