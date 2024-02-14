using AppUI.Areas.Manager.Models.DTOs;
using AppUI.Areas.Manager.Models.ViewModels;
using AppUI.Areas.Cashier.Models.DTOs;
using AppUI.Areas.Cashier.Models.ViewModels;
using AppUI.Models.ViewModels;
using AppUI.Models.DTOs;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace AppUI.Profiles;

public class VideoGameProfile : Profile
{
    public VideoGameProfile()
    {
        CreateMap<VideoGame, Areas.Manager.Models.ViewModels.VideoGameVm>()
            .ForMember(d => d.Genre, s => s.MapFrom(src => src.Genre!.Name));

        CreateMap<VideoGame, Areas.Cashier.Models.ViewModels.VideoGameVm>()
            .ForMember(d => d.Price, s => s.MapFrom(src => src.Offer != null ? src.Offer.NewPrice : src.Price))
            .ForMember(d => d.Genre, s => s.MapFrom(src => src.Genre!.Name));

        CreateMap<NewVideoGameDto, VideoGame>()
            .ForMember(d => d.IsFinal, s => s.MapFrom(src => false));

        CreateMap<VideoGame, Areas.Manager.Models.DTOs.ExistentVideoGameDto>();

        CreateMap<VideoGame, Areas.Cashier.Models.DTOs.ExistentVideoGameDto>()
            .ForMember(d => d.Price, s => s.MapFrom(src => src.Offer != null ? src.Offer.NewPrice : src.Price));

        CreateMap<VideoGame, Areas.Critic.Models.ViewModels.ShortVideoGameVm>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => src.Title));

        CreateMap<VideoGame, Models.ViewModels.ShortVideoGameVm>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => src.Title))
            .ForMember(d => d.Stock, s => s.MapFrom(src => src.Stock))
            .ForMember(d => d.IsFinal, s => s.MapFrom(src => src.IsFinal));

        CreateMap<VideoGame, Areas.Admin.Models.ViewModels.ShortVideoGameVm>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => src.Title))
            .ForMember(d => d.Stock, s => s.MapFrom(src => src.Stock))
            .ForMember(d => d.IsFinal, s => s.MapFrom(src => src.IsFinal));

        CreateMap<VideoGame, Areas.Cashier.Models.ViewModels.ShortVideoGameVm>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => src.Title))
            .ForMember(d => d.Stock, s => s.MapFrom(src => src.Stock))
            .ForMember(d => d.IsFinal, s => s.MapFrom(src => src.IsFinal));

        CreateMap<VideoGame, Areas.Delivery.Models.ViewModels.ShortVideoGameVm>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => src.Title))
            .ForMember(d => d.Stock, s => s.MapFrom(src => src.Stock))
            .ForMember(d => d.IsFinal, s => s.MapFrom(src => src.IsFinal));

        CreateMap<VideoGame, Areas.Manager.Models.DTOs.VideoGameDto>()
            .ForMember(d => d.VideoGameId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => $"{src.Title}"));

        CreateMap<VideoGame, Areas.Admin.Models.DTOs.VideoGameDto>()
            .ForMember(d => d.VideoGameId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => $"{src.Title}"));

        CreateMap<VideoGame, Areas.Critic.Models.DTOs.VideoGameDto>()
            .ForMember(d => d.VideoGameId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => $"{src.Title}"));

        CreateMap<VideoGame, Areas.Cashier.Models.DTOs.VideoGameDto>()
            .ForMember(d => d.VideoGameId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => $"{src.Title}"));

        CreateMap<VideoGame, Models.DTOs.VideoGameDto>()
            .ForMember(d => d.VideoGameId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => $"{src.Title}"))
            .ForMember(d => d.Stock, s => s.MapFrom(src => src.Stock));

        CreateMap<VideoGame, Areas.Manager.Models.ViewModels.VideoGameDetailsVm>()
            .ForMember(d => d.Genre, s => s.MapFrom(src => src.Genre!.Name));

        CreateMap<VideoGame, Areas.Cashier.Models.ViewModels.VideoGameDetailsVm>()
            .ForMember(d => d.Price, s => s.MapFrom(src => src.Offer != null ? src.Offer.NewPrice : src.Price))
            .ForMember(d => d.Genre, s => s.MapFrom(src => src.Genre!.Name));

        CreateMap<VideoGameOffer, VideoGameOfferVm>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => src.VideoGame!.Title))
            .ForMember(d => d.Price, s => s.MapFrom(src => src.VideoGame!.Price))
            .ForMember(d => d.NewPrice, s => s.MapFrom(src => src.NewPrice));

        CreateMap<VideoGameOffer, VideoGameOfferDto>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => src.VideoGame!.Title))
            .ForMember(d => d.Developers, s => s.MapFrom(src => string.Join(", ", src.VideoGame!.Developers!.Select(a => $"{a.Developer!.FirstName} {a.Developer!.LastName}"))))
            .ForMember(d => d.Price, s => s.MapFrom(src => src.VideoGame!.Price));
        /**/
        CreateMap<VideoGame, VideoGameOfferDto>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => src.Title))
            .ForMember(d => d.Price, s => s.MapFrom(src => src.Price));
        /**/
        CreateMap<VideoGameOfferDto, VideoGameOffer>();

        CreateMap<AddEditVideoGameOfferDto, VideoGameOffer>();

        CreateMap<VideoGame, VideoGamePriceVm>();

        CreateMap<VideoGame, CardVideoGameVm>()
            .ForMember(d => d.NewPrice, s => s.MapFrom(src => src.Offer != null ? src.Offer.NewPrice : 0))
            .ForMember(d => d.PromoText, s => s.MapFrom(src => src.Offer != null ? src.Offer.PromoText : string.Empty))
            .ForMember(d => d.Publisher, s => s.MapFrom(src => src.Publisher!.Name))
            .ForMember(d => d.Developers, s => s.MapFrom(src => string.Join(", ", src.Developers!.Select(a => $"{a.Developer!.FirstName} {a.Developer!.LastName}"))))
            .ForMember(d => d.Genre, s => s.MapFrom(src => src.Genre!.Name));

        CreateMap<VideoGame, Models.ViewModels.VideoGameDetailsVm>()
            .ForMember(d => d.NewPrice, s => s.MapFrom(src => src.Offer != null ? src.Offer.NewPrice : 0))
            .ForMember(d => d.Publisher, s => s.MapFrom(src => src.Publisher!.Name))
            .ForMember(d => d.Genre, s => s.MapFrom(src => src.Genre!.Name))
            .ForMember(d => d.Developers, s => s.MapFrom(src => string.Join(", ", src.Developers!.Select(a => $"{a.Developer!.FirstName} {a.Developer!.LastName}"))));
        
        /*
        CreateMap<VideoGame, NewShopCartDto>()
            //.ForMember(d => d.VideoGameId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Stock, s => s.MapFrom(src => src.Stock));
        */
    }
}