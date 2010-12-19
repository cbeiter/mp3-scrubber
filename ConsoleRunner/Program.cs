using System.Collections.Generic;
using System.IO;
using Mp3LibrarySorter;

namespace ConsoleRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            //var a = new Mp3LibrarySorter.Mp3LibrarySorter(new FileSystem(), @"c:\Viki\Music", new Mp3TagsHierarchy(), new Mp3FileReader());
            var a = new Mp3LibrarySorter.Mp3LibrarySorter(new FileSystem(), @"c:\testmp3", new Mp3TagsHierarchy(), new Mp3FileReader());
            a.CreateFoldersForArtists();
        }
    }

    public class FileSystem : IFileSystem
    {
        public void CreateDirectory(string artistName)
        {
            Directory.CreateDirectory(artistName);
        }

        public List<string> GetAllMp3Files(string someStartDirectory)
        {
            return new List<string>(Directory.GetFiles(someStartDirectory, "*.mp3", SearchOption.TopDirectoryOnly));
        }

        public void Move(string source, string destination)
        {
            File.Move(source, destination);
        }
    }
}
