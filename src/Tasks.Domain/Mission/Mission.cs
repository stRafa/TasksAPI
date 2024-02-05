namespace Tasks.Domain.Mission;

public class Mission : Entity
{
    public Mission() { }

    public Mission(string title, string description, Guid userId)
    {
        Title = title;
        Description = description;
        Status = EMissionStatus.OnGoing;
        UserId = userId;
    }

    public string Title { get; set; }
    public string Description { get; set; }
    public short Position { get; set; }
    public EMissionStatus Status { get; set; }
    public Guid UserId { get; set; }
}