using System.Collections.Generic;
using System.IO;
using Mp3LibrarySorter;

namespace ConsoleRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = new Mp3LibrarySorter.Mp3LibrarySorter(new FileSystem(), @"C:\Development\Mp3LibrarySorter\Data", new Mp3TagsHierarchy(), new Mp3FileReader() );
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
            return new List<string>(Directory.GetFiles(someStartDirectory, "*.mp3", SearchOption.AllDirectories));
        }
    }
}
