using AppUI.Areas.Manager.Models.ViewModels;
using AppUI.Models.CustomIdentity;
using AppUI.Validators;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppUI.Models.Entities;
[Table("OrderContent")]
[PrimaryKey(nameof(OrderId), nameof(VideoGameId))]
public class OrderContent
{
    public int OrderId { get; set; }
    [ForeignKey(nameof(OrderId))]
    public Order? Order { get; set; }
    public int VideoGameId { get; set; }
    [ForeignKey(nameof(VideoGameId))]
    public VideoGame? VideoGame { get; set; }
    [Column(TypeName = "decimal(7,2)"), Price]
    public decimal UnitPrice { get; set; }
    [Quantity]
    public int Quantity { get; set; }
}
