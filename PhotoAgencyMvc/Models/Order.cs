using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;

namespace PhotoAgencyMvc.Models;

public partial class Order
{
    public int Id { get; set; }

    public int ClientId { get; set; }

    public int ServiceId { get; set; }

    public DateTime OrderDate { get; set; }

    public string Status { get; set; } 

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public Client Client { get; set; }
    public Service Service { get; set; }
     
}
