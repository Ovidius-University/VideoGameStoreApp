using AppUI.Areas.Critic.Models.DTOs;
using AppUI.Areas.Manager.Models.ViewModels;
using AppUI.Areas.Critic.Models.ViewModels;
using AppUI.Models.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace AppUI.Profiles;

public class ReviewProfile : Profile
{
    public ReviewProfile()
    {
        CreateMap<Review, ReviewVm>();

        CreateMap<NewReviewDto, Review>()
            .ForMember(d => d.IsFinal, s => s.MapFrom(src => false));

        CreateMap<Review, ExistentReviewDto>()
            .ForMember(d => d.VideoGame, s => s.MapFrom(src => src.VideoGame!.Title))
            .ForMember(d => d.Reviewer, s => s.MapFrom(src => src.Reviewer!.Name))
            .ForMember(d => d.VideoGameId, s => s.MapFrom(src => src.VideoGameId))
            .ForMember(d => d.ReviewerId, s => s.MapFrom(src => src.ReviewerId));

        CreateMap<Reviewer, ShortReviewerVm>()
            .ForMember(d => d.ReviewerId, s => s.MapFrom(src => src.Id));

        CreateMap<Review, ShortReviewerVm>()
            .ForMember(d => d.ReviewerId, s => s.MapFrom(src => src.ReviewerId))
            .ForMember(d => d.Name, s => s.MapFrom(src => src.Reviewer!.Name));

        CreateMap<Review, ReviewDto>()
            .ForMember(d => d.VideoGameId, s => s.MapFrom(src => src.VideoGameId))
            .ForMember(d => d.ReviewerId, s => s.MapFrom(src => src.ReviewerId))
            .ForMember(d => d.Content, s => s.MapFrom(src => $"{src.Content}"));

       CreateMap<Review, Areas.Critic.Models.ViewModels.ReviewDetailsVm>()
            .ForMember(d => d.VideoGame, s => s.MapFrom(src => src.VideoGame!.Title))
            .ForMember(d => d.Reviewer, s => s.MapFrom(src => src.Reviewer!.Name));

        CreateMap<Review, CardReviewVm>()
            .ForMember(d => d.VideoGame, s => s.MapFrom(src => src.VideoGame!.Title))
            .ForMember(d => d.Reviewer, s => s.MapFrom(src => src.Reviewer!.Name))
            .ForMember(d => d.VideoGameId, s => s.MapFrom(src => src.VideoGameId))
            .ForMember(d => d.ReviewerId, s => s.MapFrom(src => src.ReviewerId));

        CreateMap<Review, Models.ViewModels.ReviewDetailsVm>()
            .ForMember(d => d.VideoGame, s => s.MapFrom(src => src.VideoGame!.Title))
            .ForMember(d => d.Reviewer, s => s.MapFrom(src => src.Reviewer!.Name))
            .ForMember(d => d.VideoGameId, s => s.MapFrom(src => src.VideoGameId))
            .ForMember(d => d.ReviewerId, s => s.MapFrom(src => src.ReviewerId));
    }
}