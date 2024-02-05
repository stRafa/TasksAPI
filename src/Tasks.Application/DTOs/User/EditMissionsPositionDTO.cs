namespace Tasks.Application.DTOs.User;

public class EditMissionsPositionDTO
{
    public Guid MissionId { get; set; }
    public short newPosition { get; set; }
}