using AppUI.Areas.Manager.Models.ViewModels;
using AppUI.Models.CustomIdentity;
using AppUI.Validators;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppUI.Models.Entities;
[Table("ShopCart")]
[PrimaryKey(nameof(CustomerId), nameof(VideoGameId))]
public class ShopCart
{
    public int CustomerId { get; set; }
    [ForeignKey(nameof(CustomerId))]
    public AppUser? Customer { get; set; }
    public int VideoGameId { get; set; }
    [ForeignKey(nameof(VideoGameId))]
    public VideoGame? VideoGame { get; set; }
    [Quantity]
    public int Quantity { get; set; }
}
