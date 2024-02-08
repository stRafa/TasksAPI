using Microsoft.AspNetCore.Identity;
using Tasks.Application.DTOs.User;
using Tasks.Application.DTOs.User.Validators;
using Tasks.Application.Interfaces;
using Tasks.Domain;
using Tasks.Domain.Mission;
using Tasks.Domain.User;

namespace Tasks.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMissionRepository _missionRepository;

    public UserService(IUserRepository userRepository, IMissionRepository missionRepository)
    {
        _userRepository = userRepository;
        _missionRepository = missionRepository;
    }

    public async Task<ResultViewModel<List<User>>> GetAll()
        =>  new ResultViewModel<List<User>>(await _userRepository.GetAll());

    public async Task<ResultViewModel<User>> GetById(Guid id)
    {
        var user = await _userRepository.GetById(id);

        if (user is null)
            return new ResultViewModel<User>("User not found");

        return new ResultViewModel<User>(user);
    }

    public async Task<ResultViewModel<User>> AddMission(CreateMissionDTO missionDto)
    {
        var validationResult = await new CreateMissionDTOValidator().ValidateAsync(missionDto);
        
        if (!validationResult.IsValid)
            return new ResultViewModel<User>(validationResult);
        
        var user = await _userRepository.GetById(missionDto.UserId);

        if (user is null)
            return new ResultViewModel<User>("User do not exist");

        var mission = new Mission(missionDto.Title, missionDto.Description, missionDto.UserId);
        user.AddMission(mission);
        
        _missionRepository.Create(user.Missions.FirstOrDefault(m => m.Id == mission.Id));

        return new ResultViewModel<User>(user);
    }

    public async Task<ResultViewModel<User>> UpdateMissionsPosition(EditMissionsPositionDTO missionDto)
    {
        var validationResult = await new EditMissionsPositionDTOValidator().ValidateAsync(missionDto);
        
        if (!validationResult.IsValid)
            return new ResultViewModel<User>(validationResult);
        
        var mission = await _missionRepository.GetById(missionDto.MissionId);
        
        if (mission is null)
            return new ResultViewModel<User>("Mission do not exist");

        var user = await _userRepository.GetById(mission.UserId);

        user.UpdateMissionPosition(missionDto.MissionId, missionDto.NewPosition);
        
        _missionRepository.UpdateMany(user.Missions);

        return new ResultViewModel<User>(user);
    }

    public async Task<ResultViewModel<Mission>> UpdateMission(EditMissionDTO missionDto)
    {
        var validationResult = await new EditMissionDTOValidator().ValidateAsync(missionDto);
        
        if (!validationResult.IsValid)
            return new ResultViewModel<Mission>(validationResult);
        
        var mission = await _missionRepository.GetById(missionDto.Id);

        mission.Title = missionDto.Title;
        mission.Description = missionDto.Description;
        mission.Status = missionDto.Status;
        
        _missionRepository.Update(mission);

        return new ResultViewModel<Mission>(mission);
    }

    public async Task<ResultViewModel<User>> UpdateUserInfo(EditUserDTO userDto)
    {
        var validationResult = await new EditUserDTOValidator().ValidateAsync(userDto);
        
        if (!validationResult.IsValid)
            return new ResultViewModel<User>(validationResult);

        var user = new User(userDto.Name, userDto.Email);

        user.Id = userDto.Id;
        
        _userRepository.Update(user);

        return new ResultViewModel<User>(user);
    }

    public async Task<ResultViewModel<User>> Delete(Guid id)
    {
        var user = await _userRepository.GetById(id);

        if (user is null)
            return new ResultViewModel<User>("User not found");
        
        _userRepository.Delete(id);

        return new ResultViewModel<User>(user);
    }

    public async Task<ResultViewModel<User>> DeleteMission(Guid missionId)
    {
        var mission = await _missionRepository.GetById(missionId);
        
        if (mission is null)
            return new ResultViewModel<User>("Mission not found");

        var user = await _userRepository.GetById(mission.UserId);
        
        user.RemoveMission(missionId);
        
        _missionRepository.Delete(missionId);
        
        _missionRepository.UpdateMany(user.Missions);

        return new ResultViewModel<User>(user);
    }
}