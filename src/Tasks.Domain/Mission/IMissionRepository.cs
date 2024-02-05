namespace Tasks.Domain.Mission;

public interface IMissionRepository
{
    Task<List<Mission>> GetAll();
    Task<List<Mission>> GetByUserId(Guid userId);
    Task<Mission> GetById(Guid id);
    void Create(Mission mission);
    void Update(Mission mission);
    void UpdateMany(List<Mission> missions);
    void Delete(Guid id);
}