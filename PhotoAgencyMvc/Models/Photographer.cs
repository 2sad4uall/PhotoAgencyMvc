using System;
using System.Collections.Generic;

namespace PhotoAgencyMvc.Models;

public partial class Photographer
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Bio { get; set; }

    public byte[] Photo { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();

    public virtual User User { get; set; } = null!;
}
