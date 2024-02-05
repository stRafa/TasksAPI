using Tasks.Application.DTOs.Identity;
using Tasks.Application.DTOs.User;
using Tasks.Application.ViewModels;

namespace Tasks.Application.Interfaces;

public interface IAuthService
{
    Task<ResultViewModel<LoginViewModel>> Register(CreateUserDTO userDto);

    Task<ResultViewModel<LoginViewModel>> Login(LoginDTO loginDto);
}