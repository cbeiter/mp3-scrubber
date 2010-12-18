using System.Collections.Generic;

namespace Mp3LibrarySorter
{
    public interface IFileSystem
    {
        void CreateDirectory(string artistName);
        List<string> GetAllMp3Files(string someStartDirectory);
        void Move(string source, string destination);
    }

}
