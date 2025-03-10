﻿namespace Bookify.Domain.Entities;

public class Rental : BaseEntity
{
    public int Id { get; set; }
    public int SubscriperId { get; set; }
    public Subscriper? Subscriper { get; set; }
    public DateTime StartDate { get; set; }
    public bool PenaltyPaid { get; set; }
    public ICollection<RentalCopy> RentalCopies { get; set; } = new List<RentalCopy>();
}
