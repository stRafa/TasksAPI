namespace Tasks.Domain.Passport;

public class Role : Entity
{
    public Role()
    {
        
    }
    public Role(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
    public List<Passport>? Passports { get; set; }
}