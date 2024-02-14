using AppUI.Areas.Admin.Models.DTOs;
namespace AppUI.Profiles;

public class PublisherProfile : Profile
{
    public PublisherProfile()
    {
        CreateMap<NewPublisherDto, Publisher>();
        CreateMap<Publisher, ExistentPublisherDto>();
    }
}