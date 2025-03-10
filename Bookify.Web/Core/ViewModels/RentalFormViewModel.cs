﻿namespace Bookify.Web.Core.ViewModels;

public class RentalFormViewModel
{
    public int SubscriperId { get; set; }
    public string SubscriperKey { get; set; } = null!;
    public IList<int> SelectedCopies { get; set; } = new List<int>();
    public int MaxAllowedCopies { get; set; }
}
