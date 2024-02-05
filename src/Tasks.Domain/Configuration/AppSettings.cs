namespace Tasks.API.Configuration;

public class AppSettings
{
    public string Secret { get; set; }
    public int ExpireInHours { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }

}