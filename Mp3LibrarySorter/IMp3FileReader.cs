using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mp3LibrarySorter
{
    internal class MusicID3Tag
    {
        public byte[] TAGID = new byte[3];      //  3
        public byte[] Title = new byte[30];     //  30
        public byte[] Artist = new byte[30];    //  30 
        public byte[] Album = new byte[30];     //  30 
        public byte[] Year = new byte[4];       //  4 
        public byte[] Comment = new byte[30];   //  30 
        public byte[] Genre = new byte[1];      //  1
    }

    public interface IMp3FileReader
    {
        IList<IMp3Representation> RetrieveTagsFromMp3Files(IList<string> mp3Files);
    }

    public class Mp3FileReader : IMp3FileReader
    {
        public IList<IMp3Representation> RetrieveTagsFromMp3Files(IList<string> mp3Files)
        {
            var result = new List<IMp3Representation>();
            foreach (var file in mp3Files)
            {
                using (var fs = File.OpenRead(file))
                {
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
                            string Artist = Encoding.Default.GetString(tag.Artist);
                            string Album = Encoding.Default.GetString(tag.Album);
                            var mp3Representation = new Mp3Representation()
                            {
                                AlbumName = Album,
                                ArtistName = Artist,
                                FileName = file
                            };
                            result.Add(mp3Representation);

                            //string Year = Encoding.Default.GetString(tag.Year);
                            //string Comment = Encoding.Default.GetString(tag.Comment);
                            //string Genre = Encoding.Default.GetString(tag.Genre);
                        }
                    }
                }
            }
            return result;
        }
    }

}