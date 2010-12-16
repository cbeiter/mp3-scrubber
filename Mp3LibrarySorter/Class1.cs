using System;
using System.Collections.Generic;
using System.IO;

namespace Mp3LibrarySorter
{
    
    public interface IFileSystem
    {
        void CreateDirectory(string artistName);
        List<string> GetAllMp3Files(string someStartDirectory);
    }

}
