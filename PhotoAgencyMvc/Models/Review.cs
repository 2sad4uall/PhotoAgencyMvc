using System;
using System.Collections.Generic;

namespace PhotoAgencyMvc.Models;

public partial class Review
{
    public int Id { get; set; }

    public int ClientId { get; set; }

    public int PhotographerId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime ReviewDate { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Photographer Photographer { get; set; } = null!;
}
