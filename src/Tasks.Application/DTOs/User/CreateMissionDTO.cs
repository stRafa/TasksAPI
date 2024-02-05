namespace Tasks.Application.DTOs.User;

public class CreateMissionDTO
{
    public string Title { get; set; }
    public string Description { get; set; }
    public Guid UserId { get; set; }
}