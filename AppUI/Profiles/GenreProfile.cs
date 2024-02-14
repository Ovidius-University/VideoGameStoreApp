using AppUI.Areas.Manager.Models.DTOs;
using AppUI.Areas.Manager.Models.ViewModels;
using AppUI.Models.ViewModels;
namespace AppUI.Profiles;
public class GenreProfile : Profile
{
    public GenreProfile()
    {
        CreateMap<NewGenreDto, Genre>();
        CreateMap<Genre, ExistentGenreDto>();

        CreateMap<Genre, Areas.Manager.Models.DTOs.GenreDto>()
            .ForMember(d => d.GenreId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => $"{src.Name}"));

        CreateMap<Genre, CardGenreVm>()
            .ForMember(d => d.GenreId, s => s.MapFrom(src => src.Id));
    }
}