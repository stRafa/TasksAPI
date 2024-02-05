using Tasks.Domain.Exceptions;

namespace Tasks.Domain.User;

public class User : Entity
{
    public User()
    {
        
    }
    public User(string name, string email) 
    {
        Name = name;
        Email = email;
    }

    public string Name { get; set; }
    public string Email { get; set; }
    public List<Mission.Mission>? Missions { get; set; }

    public void AddMission(Mission.Mission mission)
    {
        if (Missions is null || Missions.Count == 0)
        {
            mission.Position = 1;
            Missions = new List<Mission.Mission>
            {
                mission
            };
            return;
        }
        
        var finalPosition = Missions.Max(p => p.Position);

        mission.Position = (short)(finalPosition + 1);
        
        Missions.Add(mission);
    }

    public void UpdateMissionPosition(Guid missionId, short newPosition)
    {
        if (newPosition <= 0 && newPosition > Missions.Max(p => p.Position))
            throw new DomainException("Position must exist");
        
        var mission = Missions.FirstOrDefault(m => m.Id == missionId);
        mission.Position = newPosition;
        
        if (mission is null)
            throw new DomainException("Mission not found for this user");
        
        Missions.Remove(mission);
        
        Missions.Insert((int)(newPosition - 1), mission);

        RearrangePositions();
    }

    public void RemoveMission(Guid missionId)
    {
        var mission = Missions.FirstOrDefault(m => m.Id == missionId);
        
        Missions.Remove(mission);

        RearrangePositions();
    }


    private void RearrangePositions()
    {
        for (int i = 0; i < Missions.Count; i++)
        {
            Missions[i].Position = (short)(i + 1);
        }
    }
}