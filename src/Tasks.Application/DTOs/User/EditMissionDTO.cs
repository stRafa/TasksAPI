using Tasks.Domain.Mission;

namespace Tasks.Application.DTOs.User;

public class EditMissionDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public EMissionStatus Status { get; set; }
}