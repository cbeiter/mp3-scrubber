using System.Collections.Generic;
using Mp3LibrarySorter;
using NUnit.Framework;

namespace Mp3LibrarySorterTests
{
    [TestFixture]
    public class Mp3TagsHierachyFixture
    {
        private Mp3TagsHierarchy _mp3ClassHierarchy;
        private List<string> _expectedAlbumsForArtist;

        [SetUp]
        public void Setup()
        {
            _mp3ClassHierarchy = new Mp3TagsHierarchy();
            _mp3ClassHierarchy.AddInformation(new Mp3Representation { AlbumName = "Album1", ArtistName = "Artist", FileName = "fileName1" });
            _mp3ClassHierarchy.AddInformation(new Mp3Representation { AlbumName = "Album2", ArtistName = "Artist", FileName = "fileName2" });
            _expectedAlbumsForArtist = new List<string> {"Album1", "Album2"};
        }

        [Test]
        public void ShouldContain1ItemForArtists()
        {
            Assert.That(_mp3ClassHierarchy.Artists[0], Is.EqualTo("Artist"));
        }

        [Test]
        public void ShouldContainTwoAlbumsForTheArtist()
        {
            Assert.That(_mp3ClassHierarchy.GetAlbumsForArtist("Artist"), Is.EqualTo(_expectedAlbumsForArtist));
        }

        [Test]
        public void Album1ShouldContainFileName1()
        {
            var expectedSongsForAlbum1 = new List<string> {"fileName1"};
            Assert.That(_mp3ClassHierarchy.GetSongsForAlbumOfArtist("Album1", "Artist"), Is.EqualTo(expectedSongsForAlbum1));
        }

        [Test]
        public void WhenAddingSongToAlbum()
        {
            var expectedSongsForAlbum2 = new List<string> { "fileName2", "fileName3" };
            _mp3ClassHierarchy.AddInformation(new Mp3Representation { AlbumName = "Album2", ArtistName = "Artist", FileName = "fileName3" });
            Assert.That(_mp3ClassHierarchy.GetSongsForAlbumOfArtist("Album2", "Artist"), Is.EqualTo(expectedSongsForAlbum2));
        }

        [Test]
        public void WhenTheSameAlbumExistsForTwoArtists()
        {
            _mp3ClassHierarchy.AddInformation(new Mp3Representation { AlbumName = "Album2", ArtistName = "Artist", FileName = "fileName3" });
            _mp3ClassHierarchy.AddInformation(new Mp3Representation { AlbumName = "Album2", ArtistName = "Artist1", FileName = "fileName31" });

            var expectedSongsForArtistAlbum2 = new List<string> {"fileName2", "fileName3"};
            var expectedSongsForArtist1Album2 = new List<string> { "fileName31" };

            var actualSongsForAristAlbum2 = _mp3ClassHierarchy.GetSongsForAlbumOfArtist("Album2", "Artist");
            var actualSongsForArist1Album2 = _mp3ClassHierarchy.GetSongsForAlbumOfArtist("Album2", "Artist1");

            Assert.That(actualSongsForAristAlbum2, Is.EqualTo(expectedSongsForArtistAlbum2));
            Assert.That(actualSongsForArist1Album2, Is.EqualTo(expectedSongsForArtist1Album2));
        }
    }
}