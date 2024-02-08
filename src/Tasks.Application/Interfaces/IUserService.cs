using Tasks.Application.DTOs.User;
using Tasks.Domain.Mission;
using Tasks.Domain.User;

namespace Tasks.Application.Interfaces;

public interface IUserService
{
    Task<ResultViewModel<List<User>>> GetAll();
    Task<ResultViewModel<User>> GetById(Guid id);

    Task<ResultViewModel<User>> AddMission(CreateMissionDTO missionDto);
    Task<ResultViewModel<User>> UpdateMissionsPosition(EditMissionsPositionDTO missionDto);
    Task<ResultViewModel<Mission>> UpdateMission(EditMissionDTO missionDto);
    Task<ResultViewModel<User>> UpdateUserInfo(EditUserDTO userDto);
    Task<ResultViewModel<User>> Delete(Guid id);
    Task<ResultViewModel<User>> DeleteMission(Guid missionId);
}