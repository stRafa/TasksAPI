namespace Tasks.Application.DTOs.User;

public class EditMissionsPositionDTO
{
    public Guid MissionId { get; set; }
    public short NewPosition { get; set; }
}