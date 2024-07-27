﻿using Bookify.Web.Core.Consts;

namespace Bookify.Web.Core.ViewModels
{
    public class UpsertCategoryViewModel
    {
        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = Errors.MaxLength), Display(Name = "Category")]
        [Remote("IsCategoryAllowed", "Categories", AdditionalFields = "Id", HttpMethod = "Post", ErrorMessage = Errors.Dublicated)]
        public string Name { get; set; } = null!;
    }
}
