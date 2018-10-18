using Moq;
using Softeq.XToolkit.Common.Interfaces;
using Xunit;

namespace Softeq.XToolkit.Tests.Core.Common
{
    public class FilesProviderTests
    {
        private Mock<IFilesProvider> _filesProvider;

        public FilesProviderTests()
        {
            _filesProvider = new Mock<IFilesProvider>();
        }

        [Fact]
        public async void Fix_inerface()
        {
            //arrange
            //act
            await _filesProvider.Object.ClearFolderAsync("test");
            await _filesProvider.Object.CopyFileFromAsync("test", "test1");
            await _filesProvider.Object.RemoveAsync("test");
            await _filesProvider.Object.ExistsAsync("test");
            await _filesProvider.Object.OpenStreamForWriteAsync("test");
            await _filesProvider.Object.GetFileContentAsync("test");


            //assert
            _filesProvider.Verify(mock => mock.ClearFolderAsync("test"), Times.Once);
            _filesProvider.Verify(mock => mock.CopyFileFromAsync("test", "test1"), Times.Once);
            _filesProvider.Verify(mock => mock.RemoveAsync("test"), Times.Once);
            _filesProvider.Verify(mock => mock.ExistsAsync("test"), Times.Once);
            _filesProvider.Verify(mock => mock.OpenStreamForWriteAsync("test"), Times.Once);
            _filesProvider.Verify(mock => mock.GetFileContentAsync("test"), Times.Once);
        }
    }
}
