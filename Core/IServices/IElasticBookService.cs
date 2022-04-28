using Core.Data;
using Core.Data.Dto;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.IServices
{
    public interface IElasticBookService
    {
        Task<BookDto> GetAllBookNoFilteredAsync(int page, int quantityPerPage);
        Task<BookDto> GetByOrderFilterAsync(int page, int quantityPerPage, string selectedOrderBook);
        Task<List<CategoriesDto>> GetAllCategoriesAsync();
        Task<List<RatingDto>> GetRatingsAndBookCounts();
        Task<List<CityDto>> GetAllCityNamesAsync();
        Task<BookDto> GetWithSearchFilterForm(int page, int quantityPerPage, string selectedOrderBook, SearchFormModel searchFormModel);
        Task<List<BookIdNameDto>> GetWithSearchBarFilterText(string filterText);

    }
}
