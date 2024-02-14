using System.ComponentModel.DataAnnotations;
namespace AppUI.Areas.Admin.Models.DTOs;
public class ExistentPublisherManagerDto
{
    public int ManagerId { get; set; }
    public int PublisherId { get; set; }
}