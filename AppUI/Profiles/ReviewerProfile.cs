using AppUI.Areas.Admin.Models.DTOs;
using AppUI.Areas.Admin.Models.ViewModels;
using AppUI.Models.ViewModels;
namespace AppUI.Profiles;
public class ReviewerProfile : Profile
{
    public ReviewerProfile()
    {
        CreateMap<NewReviewerDto, Reviewer>();
        CreateMap<Reviewer, ExistentReviewerDto>();
    }
}