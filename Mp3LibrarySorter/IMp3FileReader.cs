using System.Collections.Generic;

namespace Mp3LibrarySorter
{
    public interface IMp3FileReader
    {
        IList<IMp3Representation> RetrieveTagsFromMp3Files(IList<string> mp3Files);
        IList<string> FilesWithNoTags { get;}
    }
}