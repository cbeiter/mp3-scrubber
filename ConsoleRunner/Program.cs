using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mp3LibrarySorter;

namespace ConsoleRunner
{
    class Program
    {
        static void Main(string[] args)
        {
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
