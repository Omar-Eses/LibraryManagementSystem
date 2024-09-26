using AutoMapper;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services.Commands.UserCommandsHandlers;


namespace LibraryManagementSystem.Background;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, CreateUserCommand>()
            .ForMember(dest => dest.UserPermissionsList, opt => opt.MapFrom(src => src.UserPermissions.Select(up => up.PermissionId).ToList()));

        CreateMap<User, UpdateUserCommand>()
            .ForMember(dest => dest.UserPermissionsList, opt => opt.MapFrom(src => src.UserPermissions.Select(up => up.PermissionId).ToList()));

        CreateMap<User, DeleteUserCommand>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
    }
}