namespace Tasks.Domain.User;

public interface IUserRepository
{
    void Create(User user);

    Task<User> GetById(Guid id);

    Task<List<User>> GetAll();

    void Update(User user);

    void Delete(Guid id);
}