using System.Security.Claims;

namespace Tasks.Domain.Passport;

public class Passport : Entity
{
    public Passport()
    {
        
    }
    public Passport(string name, string username, string passwordHash, string passwordSalt)
    {
        Name = name;
        Username = username;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
        Status = true;
        CreatedAt = DateTime.UtcNow;
        Claims = new List<Claim>();
        Roles = new List<Role>();
    }

    public string Name { get; set;  }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public List<Claim> Claims { get; set; }
    public List<Role> Roles { get; set; }
}