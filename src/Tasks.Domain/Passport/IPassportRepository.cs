namespace Tasks.Domain.Passport;

public interface IPassportRepository
{
    Task<Passport> GetPassportByUsername(string username);
    void CreatePassport(Passport passport);
}