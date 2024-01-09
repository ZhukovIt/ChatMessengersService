using System;
using Xunit;
using BulkMessagesWebServer.DataModel.FileSystem.Images;

namespace BulkMessagesWebServer.Tests.Unit
{
    public sealed class FileSystemImagesTests
    {
        [Fact]
        public void New_Image_Is_Created()
        {
            ImagesManager sut = new ImagesManager(7);
            FileImageContent[] _Files = new FileImageContent[]
            {
                new FileImageContent("Image1.jpg", new DateTime(2023, 9, 1, 12, 0, 0)),
                new FileImageContent("Image168.jpg", new DateTime(2023, 9, 1, 13, 0, 0))
            };

            FileImageAction add = sut.AddImage(_Files, "");

            Assert.Equal("Image169.jpg", add.FileName);
            Assert.Equal(Convert.FromBase64String(""), add.FileData);
        }
        //-------------------------------------------------------------------------------
        [Fact]
        public void Images_Is_Delete_When_They_Got_Old()
        {
            ImagesManager sut = new ImagesManager(7);
            FileImageContent[] _Files = new FileImageContent[]
            {
                new FileImageContent("Image1.jpg", new DateTime(2023, 9, 1, 12, 0, 0)),
                new FileImageContent("Image2.jpg", new DateTime(2023, 9, 1, 13, 0, 0)),
                new FileImageContent("Image20.jpg", new DateTime(2023, 9, 2, 12, 0, 0)),
                new FileImageContent("Image100.jpg", new DateTime(2023, 9, 3, 12, 0, 0)),
                new FileImageContent("Image112.jpg", new DateTime(2023, 9, 4, 12, 0, 0)),
            };

            FileImageAction[] deletedFiles = sut.DeleteOldFiles(_Files, new DateTime(2023, 9, 9));

            Assert.Equal(2, deletedFiles.Length);
            Assert.Equal("Image1.jpg", deletedFiles[0].FileName);
            Assert.Equal("Image2.jpg", deletedFiles[1].FileName);
        }
        //-------------------------------------------------------------------------------
    }
}
