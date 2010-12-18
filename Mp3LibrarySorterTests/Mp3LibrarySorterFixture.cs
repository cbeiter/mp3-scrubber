using System.Collections.Generic;
using System.IO;
using Mp3LibrarySorter;
using NUnit.Framework;
using Rhino.Mocks;

namespace Mp3LibrarySorterTests
{
    [TestFixture]
    public class Mp3LibrarySorterTests
    {
        private Mp3LibrarySorter.Mp3LibrarySorter _mp3LibrarySorter;
        private IFileSystem _mockFileSystem;
        const string SomeStartDirectory = "someDirectory";
        List<IMp3Representation> _mp3Representations = new List<IMp3Representation>();
        private IMp3Representation _mockMp3Representation;
        private IMp3TagsHierarchy _mockMp3TagHierarchy;
        private IMp3FileReader _mockMp3FileReader;
        private readonly string _albumFolder = SomeStartDirectory + Path.DirectorySeparatorChar + SomeArtistName + Path.DirectorySeparatorChar + SomeAlbumName;
        const string SomeFileName = "SomeFileName";
        const string SomeAlbumName = "SomeAlbumName";
        const string SomeArtistName = "SomeArtistName";

        [SetUp]
        public void Setup()
        {
            _mockMp3Representation = MockRepository.GenerateStub<IMp3Representation>();
            _mockMp3Representation.AlbumName = SomeAlbumName;
            _mockMp3Representation.ArtistName = SomeArtistName;
            _mockMp3Representation.FileName = SomeFileName;

            _mockFileSystem = MockRepository.GenerateStub<IFileSystem>();
            _mockFileSystem.Stub(system => system.GetAllMp3Files(SomeStartDirectory)).Return(new List<string> { SomeFileName });

            _mockMp3TagHierarchy = MockRepository.GenerateStub<IMp3TagsHierarchy>();
            _mockMp3TagHierarchy.Stub(hierarchy => hierarchy.Artists).Return(new List<string> {SomeArtistName});
            _mockMp3TagHierarchy.Stub(tagsHierarchy => tagsHierarchy.GetAlbumsForArtist(SomeArtistName)).Return(
                new List<string> {SomeAlbumName});

            _mockMp3TagHierarchy.Stub(
                mp3TagsHierarchy => mp3TagsHierarchy.GetSongsForAlbumOfArtist(SomeAlbumName, SomeArtistName)).Return(
                    new List<string> {SomeFileName});
            _mockMp3FileReader = MockRepository.GenerateStub<IMp3FileReader>();
            _mockMp3FileReader.Stub(reader => reader.RetrieveTagsFromMp3Files(new List<string> {SomeFileName})).Return(
                new List<IMp3Representation>
                    {
                        new Mp3Representation
                            {AlbumName = SomeAlbumName, ArtistName = SomeArtistName, FileName = SomeFileName}
                    });

            _mp3LibrarySorter = new Mp3LibrarySorter.Mp3LibrarySorter(_mockFileSystem, SomeStartDirectory, _mockMp3TagHierarchy, _mockMp3FileReader);
            _mp3LibrarySorter.CreateFoldersForArtists();
        }

        [Test]
        public void ShouldRetrieveAllMp3FilesFromStartingDirectory()
        {
            _mockFileSystem.AssertWasCalled(system => system.GetAllMp3Files(SomeStartDirectory));
        }

        [Test]
        public void ShouldCreateDirectorySomeArtistName()
        {
            _mockFileSystem.AssertWasCalled(system => system.CreateDirectory(SomeStartDirectory + Path.DirectorySeparatorChar + SomeArtistName));
        }

        [Test]
        public void ShouldCreateDirectorySomeAlbumName()
        {
            _mockFileSystem.AssertWasCalled(system => system.CreateDirectory(_albumFolder));
        }

        [Test]
        public void ShouldMoveValidMp3FilesToAlbumFolder()
        {
            _mockFileSystem.AssertWasCalled(system => system.Move(SomeFileName, _albumFolder + Path.DirectorySeparatorChar + SomeFileName));
        }
    }
}
