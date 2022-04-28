using Core.Data;
using Core.Data.Dto;
using Core.Data.Exceptions;
using Core.IServices;
using Mapster;
using Microsoft.Extensions.Configuration;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class ElasticBookService : IElasticBookService
    {
        private readonly IConfiguration configuration;
        private readonly IElasticClient elasticClient;
        public ElasticBookService(IConfiguration _configuration)
        {
            configuration = _configuration;
            elasticClient = GetElasticClient();
        }


        public ElasticClient GetElasticClient()
        {
            //var host = configuration["ElasticBook:Host"];
            var host = configuration.GetSection("ElasticBook:Host").Value;
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ElasticException("Elastic host is not empty");
            }
            var constring = new ConnectionSettings(new Uri(host)).SniffOnConnectionFault(false).SniffOnStartup(false);
            constring.OnRequestCompleted(apiCallDetails =>
            {
                if (apiCallDetails.HttpStatusCode == 418)
                {
                    throw new ElasticException(apiCallDetails.OriginalException.Message);
                }
            }).DefaultIndex("books3");
            return new ElasticClient(constring);
        }
        public virtual async Task<BookDto> GetAllBookNoFilteredAsync(int page, int quantityPerPage)
        {
            var response = await elasticClient.SearchAsync<Book>(b =>
                b.Query(q => q.MatchAll()).Index("books3")
                .Skip((page - 1) * quantityPerPage)
                .Size(quantityPerPage)
                .Sort(s => s.Field(f => f.Price, SortOrder.Ascending))
                .Aggregations(p => p.Average("Total", a => a.Field(f => f.Price)))
                );
            if (response.ServerError is not null) throw new ElasticException(response.ServerError.Error.Reason);
            var responseDto = new BookDto();
            responseDto.Books = response.Documents.Adapt<List<Book>>();
            responseDto.Total = response.Total;
            return responseDto;
        }
        public virtual async Task<BookDto> GetByOrderFilterAsync(int page, int quantityPerPage, string selectedOrderBook)
        {
            QueryContainer _query = new MatchAllQuery();

            var sort = FillSelectedOrderBook(selectedOrderBook);

            SearchRequest searchRequest = new SearchRequest<Book>()
            {
                From = (page - 1) * quantityPerPage,
                Size = quantityPerPage,
                Query = _query,
                Sort = sort
            };

            var searchRequestResponse = await elasticClient.SearchAsync<Book>(searchRequest);
            var bookDto = new BookDto();
            if (searchRequestResponse.IsValid)
            {
                bookDto.Books = searchRequestResponse.Documents.ToList();
                bookDto.Total = searchRequestResponse.Total;
                return bookDto;
            }
            throw new ElasticException(searchRequestResponse.ServerError.Error.Reason);
        }

        public virtual async Task<List<CategoriesDto>> GetAllCategoriesAsync()
        {
            var categoriesDtos = new List<CategoriesDto>();

            var requestOfCategoriBookCount = await elasticClient.SearchAsync<Book>(b =>
            b.Index("books3")
            .Aggregations(a => a.Terms("categoryBookCount", t => t.Field("categories.keyword").Size(1000))));

            if (requestOfCategoriBookCount.ServerError is not null) throw new ElasticException(requestOfCategoriBookCount.ServerError.Error.Reason);

            var aggrRes = requestOfCategoriBookCount.Aggregations.Terms("categoryBookCount").Buckets;
            foreach (var bucket in aggrRes)
            {
                var categoriesDto = new CategoriesDto(String.IsNullOrEmpty(bucket.Key) ? "Kategorisiz" : bucket.Key, bucket.DocCount ?? 0);
                categoriesDtos.Add(categoriesDto);
            }

            return categoriesDtos;
        }
        public async Task<List<RatingDto>> GetRatingsAndBookCounts()
        {
            var request = await elasticClient.SearchAsync<Book>(b =>
                b.Index("books3")
                .Aggregations(a => a.Terms("ratingStars", t => t.Field("stars"))));
            if (request.ServerError is not null) throw new ElasticException(request.ServerError.Error.Reason);
            var aggRes = request.Aggregations.Terms("ratingStars").Buckets;

            var ratingDtos = aggRes.Select(s => new RatingDto(s.Key.Adapt<int>(),s.DocCount ?? 0)).OrderBy(o => o.RatingStar).ToList();
            return ratingDtos;
        }
        public async Task<List<CityDto>> GetAllCityNamesAsync()
        {
            var request = await elasticClient.SearchAsync<Book>(b => b.Index("books3").Aggregations(a => a.Terms("cities", t => t.Field("author.address.city.keyword").Size(100))));
            if (request.ServerError is not null) throw new ElasticException(request.ServerError.Error.Reason);
            var aggRes = request.Aggregations.Terms("cities").Buckets;

            return aggRes.Select(s => new CityDto(s.Key, s.DocCount ?? 0)).OrderBy(o => o.Name).ToList();
        }
        public async Task<BookDto> GetWithSearchFilterForm(int page, int quantityPerPage, string selectedOrderBook, SearchFormModel searchFormModel)
        {
            var query = FillSearchFormModel(searchFormModel);

            var sort = FillSelectedOrderBook(selectedOrderBook);

            SearchRequest searchRequest = new SearchRequest<Book>()
            {
                From = (page - 1) * quantityPerPage,
                Size = quantityPerPage,
                Query = query,
                Sort = sort
            };

            var searchRequestResponse = await elasticClient.SearchAsync<Book>(searchRequest);
            var bookDto = new BookDto();
            if (searchRequestResponse.IsValid)
            {
                bookDto.Books = searchRequestResponse.Documents.ToList();
                bookDto.Total = searchRequestResponse.Total;
                return bookDto;
            }
            throw new ElasticException(searchRequestResponse.ServerError.Error.Reason);
        }
        public List<ISort> FillSelectedOrderBook(string selectedOrderBook)
        {
            var sort = new List<ISort>();

            if (selectedOrderBook == SelectableOrderBookTypes.PriceLowToHigh)
            {
                sort.Add(new FieldSort { Field = "price", Order = SortOrder.Ascending });
            }
            if (selectedOrderBook == SelectableOrderBookTypes.PriceHighToLow)
            {
                sort.Add(new FieldSort { Field = "price", Order = SortOrder.Descending });
            }
            if (selectedOrderBook == SelectableOrderBookTypes.LikeCountLowToHigh)
            {
                sort.Add(new FieldSort { Field = "likeCount", Order = SortOrder.Ascending });
            }
            if (selectedOrderBook == SelectableOrderBookTypes.LikeCountHighToLow)
            {
                sort.Add(new FieldSort { Field = "likeCount", Order = SortOrder.Descending });
            }
            return sort;
        }
        public QueryContainer FillSearchFormModel(SearchFormModel searchFormModel)
        {
            QueryContainer? _query = null;
            
            if (searchFormModel.MinPrice > 0)
            {
                var rangeQuery = new NumericRangeQuery
                {
                    Field = new Field("price"),
                    GreaterThanOrEqualTo = searchFormModel.MinPrice
                };
                _query = _query == null ? rangeQuery : _query && rangeQuery;
            }
            if (searchFormModel.MaxPrice > 0)
            {
                var rangeQuery = new NumericRangeQuery
                {
                    Field = new Field("price"),
                    LessThanOrEqualTo = searchFormModel.MaxPrice
                };
                _query = _query == null ? rangeQuery : _query && rangeQuery;
            }
            if (searchFormModel.SelectedCategory.Count > 0)
            {
                QueryContainer? categoryQuery = null;
                foreach (var category in searchFormModel.SelectedCategory)
                {
                    var qry = new MatchQuery()
                    {
                        Field = new Field("categories.keyword"),
                        Query = category
                    };
                    categoryQuery = categoryQuery == null ? qry : categoryQuery || qry;
                }
                _query = _query == null ? categoryQuery : _query && categoryQuery;
            }
            if (searchFormModel.SelectedStar.Count > 0)
            {
                QueryContainer? starQuery = null;
                foreach (var star in searchFormModel.SelectedStar)
                {
                    var qry = new MatchQuery()
                    {
                        Field = new Field("stars"),
                        Query = star.ToString()
                    };
                    starQuery = starQuery == null ? qry : starQuery || qry;
                }
                _query = _query == null ? starQuery : _query && starQuery;
            }
            if (searchFormModel.SelectedCity.Count > 0)
            {
                QueryContainer? cityQuery = null;
                foreach (var city in searchFormModel.SelectedCity)
                {
                    var qry = new MatchQuery()
                    {
                        Field = new Field("author.address.city.keyword"),
                        Query = city
                    };
                    cityQuery = cityQuery == null ? qry : cityQuery || qry;
                }
                _query = _query == null ? cityQuery : _query && cityQuery;
            }
            return _query is null ? new QueryContainer() : _query;
        }
        public async Task<List<BookIdNameDto>> GetWithSearchBarFilterText(string filterText)
        {
            var searchRequest = await elasticClient.SearchAsync<Book>(p =>
            p.Index("books3")
            .Query(q =>
                q.Wildcard(w => w.Field(f => f.Title).Value($"{filterText}*"))
                    ).Highlight(h => h.Fields(fields => fields.Field(field => field.Title).
                    PreTags(new[] { "<b style=\"border-radius:15%;color:black;background-color:dodgerblue;\">" }).PostTags(new[] { "</b>" })))
                );
            if (searchRequest.ServerError is not null) throw new ElasticException(searchRequest.ServerError.Error.Reason);
            return searchRequest.Hits.Select(s => new BookIdNameDto(s.Id, s.Highlight.Values.First().First())).ToList();

        }
    }
}
