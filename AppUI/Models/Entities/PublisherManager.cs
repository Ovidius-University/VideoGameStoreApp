using AppUI.Models.CustomIdentity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppUI.Models.Entities;
[Table("Publisher_Manager")]
[PrimaryKey(nameof(ManagerId), nameof(PublisherId))]
public class PublisherManager
{
    [Column("UserId")]
    public int ManagerId { get; set; }
    public int PublisherId { get; set; }
    [ForeignKey(nameof(ManagerId))]
    public AppUser? Manager { get; set; }
    [ForeignKey(nameof(PublisherId))]
    public Publisher? Publisher { get; set; }
}