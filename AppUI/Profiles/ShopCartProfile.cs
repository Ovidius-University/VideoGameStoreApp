using AppUI.Models.CustomIdentity;
using AppUI.Areas.Admin.Models.DTOs;
using AppUI.Areas.Manager.Models.DTOs;
using AppUI.Areas.Manager.Models.ViewModels;
using AppUI.Areas.Critic.Models.DTOs;
using AppUI.Areas.Critic.Models.ViewModels;
using AppUI.Models.ViewModels;
using AppUI.Models.DTOs;
using AppUI.Models.Entities;
namespace AppUI.Profiles;
public class ShopCartProfile : Profile
{
    public ShopCartProfile()
    {
        CreateMap<ShopCart, ShopCartVm>()
            .ForMember(d => d.UnitPrice, s => s.MapFrom(src => src.VideoGame!.Offer != null ? src.VideoGame!.Offer.NewPrice : src.VideoGame!.Price))
            .ForMember(d => d.TotalPrice, s => s.MapFrom(src => src.VideoGame!.Offer != null ? src.VideoGame!.Offer.NewPrice*(decimal)src.Quantity : src.VideoGame!.Price*(decimal)src.Quantity));

        CreateMap<NewShopCartDto, ShopCart>();

        CreateMap<ShopCart, ExistentShopCartDto>()
            .ForMember(d => d.VideoGame, s => s.MapFrom(src => src.VideoGame!.Title))
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Customer!.UserName))
            .ForMember(d => d.VideoGameId, s => s.MapFrom(src => src.VideoGameId))
            .ForMember(d => d.CustomerId, s => s.MapFrom(src => src.CustomerId));

        CreateMap<ShopCart, ShopCartDto>()
            .ForMember(d => d.VideoGameId, s => s.MapFrom(src => src.VideoGameId))
            .ForMember(d => d.CustomerId, s => s.MapFrom(src => src.CustomerId))
            .ForMember(d => d.Quantity, s => s.MapFrom(src => src.Quantity));

        CreateMap<ShopCart, Models.ViewModels.ShopCartDetailsVm>()
            .ForMember(d => d.VideoGame, s => s.MapFrom(src => src.VideoGame!.Title))
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Customer!.UserName))
            .ForMember(d => d.UnitPrice, s => s.MapFrom(src => src.VideoGame!.Offer != null ? src.VideoGame!.Offer.NewPrice : src.VideoGame!.Price))
            .ForMember(d => d.VideoGameId, s => s.MapFrom(src => src.VideoGameId))
            .ForMember(d => d.CustomerId, s => s.MapFrom(src => src.CustomerId))
            .ForMember(d => d.TotalPrice, s => s.MapFrom(src => src.VideoGame!.Offer != null ? src.VideoGame!.Offer.NewPrice*(decimal)src.Quantity : src.VideoGame!.Price*(decimal)src.Quantity));
    }
}