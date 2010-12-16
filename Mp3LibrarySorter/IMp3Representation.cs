namespace Mp3LibrarySorter
{
    public interface IMp3Representation
    {
        string FileName { get; set; }
        string ArtistName { get; set; }
        string AlbumName { get; set; }
    }

    public class Mp3Representation : IMp3Representation
    {
        public string FileName { get; set; }
        public string ArtistName { get; set; }
        public string AlbumName { get; set; }
    }
}