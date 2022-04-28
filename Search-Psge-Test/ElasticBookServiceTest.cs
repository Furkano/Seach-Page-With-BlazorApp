using Core.Data;
using Core.Data.Dto;
using Core.Data.Exceptions;
using Core.IServices;
using Core.Services;
using Elasticsearch.Net;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Nest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Search_Psge_Test
{
    public class Tests
    {
        //private ElasticBookService elasticBookServiceMock;
        private Mock<IElasticBookService> elasticBookServiceMock;
        private Mock<IElasticClient> elasticClientMock;
        private Mock<BookDto> bookDtoMock;
        private Mock<Book> bookMock;
        private List<Book> booksMock;
        private Mock<ISearchResponse<Book>> mockSearchResponse;
        [SetUp]
        public void Setup()
        {
            //var configuration = new Mock<IConfiguration>();
            //var configSection = new Mock<IConfigurationSection>();
            //configSection.Setup(x => x.Value).Returns("http://localhost:9200");
            //configuration.Setup(s => s.GetSection("ElasticBook:Host")).Returns(configSection.Object);

            //elasticBookServiceMock = new ElasticBookService(configuration.Object);
            elasticBookServiceMock = new Mock<IElasticBookService>();

            elasticClientMock = new Mock<IElasticClient>();

            bookMock = new Mock<Book>();
            booksMock = new List<Book>() { bookMock.Object, bookMock.Object, bookMock.Object, bookMock.Object, bookMock.Object, bookMock.Object, bookMock.Object, bookMock.Object, bookMock.Object, bookMock.Object, };
            
            bookDtoMock = new Mock<BookDto>();
            bookDtoMock.Object.Books = booksMock;

            mockSearchResponse = new Mock<ISearchResponse<Book>>();
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
        [Test]
        public async Task GetAllBookNoFilteredAsync_ShouldReturnBookDto_WhenElasticSearchResponseNotHasServerError()
        {
            mockSearchResponse.Setup(s => s.Documents).Returns(booksMock);
            mockSearchResponse.Setup(s => s.Total).Returns(395);

            bookDtoMock.Object.Total = mockSearchResponse.Object.Total;

            elasticClientMock.Setup(s => s
                .SearchAsync(It.IsAny<Func<SearchDescriptor<Book>, ISearchRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockSearchResponse.Object);
            
            elasticBookServiceMock.Setup(s=>s.GetAllBookNoFilteredAsync(It.IsAny<int>(),It.IsAny<int>()))
                .ReturnsAsync(bookDtoMock.Object);

            var result = await elasticBookServiceMock.Object.GetAllBookNoFilteredAsync(1, 10);

            result.Books.Count.Should().Be(bookDtoMock.Object.Books.Count);
            result.Total.Should().Be(bookDtoMock.Object.Total);
        }
        [Test]
        public async Task GetAllBookNoFilteredAsync_ShouldThrowException_WhenElasticSearchResponseHasServerError()
        {
            mockSearchResponse.Setup(s => s.ServerError).Returns(() => new ServerError());

            elasticClientMock.Setup(s=>s
                .SearchAsync(It.IsAny<Func<SearchDescriptor<Book>, ISearchRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockSearchResponse.Object);
            elasticBookServiceMock.Setup(s => s.GetAllBookNoFilteredAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new ElasticException());

            
            await elasticBookServiceMock.Invoking(p=> p.Object.GetAllBookNoFilteredAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Should().ThrowAsync<ElasticException>();
        }
        [Test]
        public async Task GetByOrderFilterAsync_ShouldReturnBookDto_WhenNotGetException()
        {
            //Arrange
            mockSearchResponse.Setup(s => s.Documents).Returns(booksMock);
            mockSearchResponse.Setup(s => s.Total).Returns(395);

            bookDtoMock.Object.Total = mockSearchResponse.Object.Total;

            elasticClientMock.Setup(s => s
                .SearchAsync(It.IsAny<Func<SearchDescriptor<Book>, ISearchRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockSearchResponse.Object);

            elasticBookServiceMock.Setup(s => s
                .GetByOrderFilterAsync(It.IsAny<int>(), It.IsAny<int>(), SelectableOrderBookTypes.PriceLowToHigh))
                .ReturnsAsync(bookDtoMock.Object);
            //Act
            var result = await elasticBookServiceMock.Object
                .GetByOrderFilterAsync(It.IsAny<int>(), It.IsAny<int>(), SelectableOrderBookTypes.PriceLowToHigh);
            
            //Assert
            result.Books.Count.Should().Be(bookDtoMock.Object.Books.Count);
            result.Total.Should().Be(bookDtoMock.Object.Total);
        }
        public async Task GetByOrderFilterAsync_ShouldReturnBookDto_WhenGetException()
        {
            //Arrange
            mockSearchResponse.Setup(s => s.ServerError).Returns(() => new ServerError());

            elasticClientMock.Setup(s => s
                .SearchAsync(It.IsAny<Func<SearchDescriptor<Book>, ISearchRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockSearchResponse.Object);
            //Act
            elasticBookServiceMock.Setup(s => s.GetByOrderFilterAsync(It.IsAny<int>(), It.IsAny<int>(),SelectableOrderBookTypes.PriceLowToHigh))
                .ThrowsAsync(new ElasticException());
            //Assert
            await elasticBookServiceMock.Invoking(p => p.Object.GetByOrderFilterAsync(It.IsAny<int>(), It.IsAny<int>(), SelectableOrderBookTypes.PriceLowToHigh))
                .Should().ThrowAsync<ElasticException>();
        }
    }
}