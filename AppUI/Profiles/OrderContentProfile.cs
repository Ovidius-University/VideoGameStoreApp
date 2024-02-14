using AppUI.Areas.Admin.Models.DTOs;
using AppUI.Areas.Admin.Models.ViewModels;
using AppUI.Models.DTOs;
using AppUI.Models.ViewModels;
namespace AppUI.Profiles;
public class OrderContentProfile : Profile
{
    public OrderContentProfile()
    {
        CreateMap<ShopCartVm, OrderContent>()
            .ForMember(d => d.VideoGameId, s => s.MapFrom(src => src.VideoGameId))
            .ForMember(d => d.UnitPrice, s => s.MapFrom(src => src.UnitPrice))
            .ForMember(d => d.Quantity, s => s.MapFrom(src => src.Quantity))
            .ForMember(d => d.VideoGame, s => s.Ignore());

        CreateMap<OrderContent, Models.ViewModels.OrderContentVm>()
            .ForMember(d => d.UnitPrice, s => s.MapFrom(src => src.UnitPrice))
            .ForMember(d => d.TotalPrice, s => s.MapFrom(src => src.UnitPrice * (decimal)src.Quantity));

        CreateMap<Models.DTOs.NewOrderContentDto, OrderContent>();

        CreateMap<OrderContent, Models.DTOs.ExistentOrderContentDto>()
            .ForMember(d => d.VideoGame, s => s.MapFrom(src => src.VideoGame!.Title))
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Order!.Customer!.UserName))
            .ForMember(d => d.VideoGameId, s => s.MapFrom(src => src.VideoGameId))
            .ForMember(d => d.CustomerId, s => s.MapFrom(src => src.Order!.CustomerId));

        CreateMap<OrderContent, Areas.Admin.Models.ViewModels.OrderContentVm>()
            .ForMember(d => d.UnitPrice, s => s.MapFrom(src => src.UnitPrice))
            .ForMember(d => d.TotalPrice, s => s.MapFrom(src => src.UnitPrice * (decimal)src.Quantity));

        CreateMap<OrderContent, Areas.Cashier.Models.ViewModels.OrderContentVm>()
            .ForMember(d => d.UnitPrice, s => s.MapFrom(src => src.UnitPrice))
            .ForMember(d => d.TotalPrice, s => s.MapFrom(src => src.UnitPrice * (decimal)src.Quantity));

        CreateMap<OrderContent, Areas.Delivery.Models.ViewModels.OrderContentVm>()
            .ForMember(d => d.UnitPrice, s => s.MapFrom(src => src.UnitPrice))
            .ForMember(d => d.TotalPrice, s => s.MapFrom(src => src.UnitPrice * (decimal)src.Quantity));

        CreateMap<Areas.Admin.Models.DTOs.NewOrderContentDto, OrderContent>();

        CreateMap<Areas.Cashier.Models.DTOs.NewOrderContentDto, OrderContent>();

        CreateMap<OrderContent, Areas.Admin.Models.DTOs.ExistentOrderContentDto>()
            .ForMember(d => d.VideoGame, s => s.MapFrom(src => src.VideoGame!.Title))
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Order!.Customer!.UserName))
            .ForMember(d => d.VideoGameId, s => s.MapFrom(src => src.VideoGameId))
            .ForMember(d => d.CustomerId, s => s.MapFrom(src => src.Order!.CustomerId));

        CreateMap<OrderContent, Areas.Cashier.Models.DTOs.ExistentOrderContentDto>()
            .ForMember(d => d.VideoGame, s => s.MapFrom(src => src.VideoGame!.Title))
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Order!.Customer!.UserName))
            .ForMember(d => d.VideoGameId, s => s.MapFrom(src => src.VideoGameId))
            .ForMember(d => d.CustomerId, s => s.MapFrom(src => src.Order!.CustomerId));

        CreateMap<OrderContent, Areas.Delivery.Models.DTOs.ExistentOrderContentDto>()
            .ForMember(d => d.VideoGame, s => s.MapFrom(src => src.VideoGame!.Title))
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Order!.Customer!.UserName))
            .ForMember(d => d.VideoGameId, s => s.MapFrom(src => src.VideoGameId))
            .ForMember(d => d.CustomerId, s => s.MapFrom(src => src.Order!.CustomerId));

        CreateMap<OrderContent, Models.ViewModels.OrderContentDetailsVm>()
            .ForMember(d => d.VideoGame, s => s.MapFrom(src => src.VideoGame!.Title))
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Order!.Customer!.UserName))
            .ForMember(d => d.UnitPrice, s => s.MapFrom(src => src.UnitPrice))
            .ForMember(d => d.VideoGameId, s => s.MapFrom(src => src.VideoGameId))
            .ForMember(d => d.CustomerId, s => s.MapFrom(src => src.Order!.CustomerId))
            .ForMember(d => d.TotalPrice, s => s.MapFrom(src => src.UnitPrice * (decimal)src.Quantity));

        CreateMap<OrderContent, Areas.Admin.Models.ViewModels.OrderContentDetailsVm>()
            .ForMember(d => d.VideoGame, s => s.MapFrom(src => src.VideoGame!.Title))
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Order!.Customer!.UserName))
            .ForMember(d => d.UnitPrice, s => s.MapFrom(src => src.UnitPrice))
            .ForMember(d => d.VideoGameId, s => s.MapFrom(src => src.VideoGameId))
            .ForMember(d => d.CustomerId, s => s.MapFrom(src => src.Order!.CustomerId))
            .ForMember(d => d.TotalPrice, s => s.MapFrom(src => src.UnitPrice * (decimal)src.Quantity));

        CreateMap<OrderContent, Areas.Cashier.Models.ViewModels.OrderContentDetailsVm>()
            .ForMember(d => d.VideoGame, s => s.MapFrom(src => src.VideoGame!.Title))
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Order!.Customer!.UserName))
            .ForMember(d => d.UnitPrice, s => s.MapFrom(src => src.UnitPrice))
            .ForMember(d => d.VideoGameId, s => s.MapFrom(src => src.VideoGameId))
            .ForMember(d => d.CustomerId, s => s.MapFrom(src => src.Order!.CustomerId))
            .ForMember(d => d.TotalPrice, s => s.MapFrom(src => src.UnitPrice * (decimal)src.Quantity));

        CreateMap<OrderContent, Areas.Delivery.Models.ViewModels.OrderContentDetailsVm>()
            .ForMember(d => d.VideoGame, s => s.MapFrom(src => src.VideoGame!.Title))
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Order!.Customer!.UserName))
            .ForMember(d => d.UnitPrice, s => s.MapFrom(src => src.UnitPrice))
            .ForMember(d => d.VideoGameId, s => s.MapFrom(src => src.VideoGameId))
            .ForMember(d => d.CustomerId, s => s.MapFrom(src => src.Order!.CustomerId))
            .ForMember(d => d.TotalPrice, s => s.MapFrom(src => src.UnitPrice * (decimal)src.Quantity));
    }
}
