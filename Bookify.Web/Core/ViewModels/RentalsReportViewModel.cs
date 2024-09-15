﻿
namespace Bookify.Web.Core.ViewModels;

public class RentalsReportViewModel
{

    public string Duration { get; set; } = null!;
    public PaginatedList<RentalCopy>? Data { get; set; }

}
