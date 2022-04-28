using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data
{
    public class SearchFormModel
    {
        [Range(1, 10000, ErrorMessage = "Min fiyat yanlış. (1-10000).")]
        public int MinPrice { get; set; }
        [Range(1, 10000, ErrorMessage = "Max fiyat yanlış. (1-10000).")]
        public int MaxPrice { get; set; }
        public List<string> SelectedCategory { get; set; }
        public List<string> SelectedCity { get; set; }
        public List<int> SelectedStar { get; set; }


        public SearchFormModel()
        {
            SelectedCategory = new();
            SelectedCity = new();
            SelectedStar = new();
        }
        public bool IsAnyPropertyNotNull()
        {
            if (this.MinPrice > 0 | this.MaxPrice > 0 | this.SelectedCategory.Count > 0 | this.SelectedCity.Count > 0 | this.SelectedStar.Count > 0)
            {
                return true;
            }
            return false;
        }
    }
}
