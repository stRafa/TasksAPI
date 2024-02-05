namespace Tasks.Application.ViewModels;

public class LoginViewModel
{
    public LoginViewModel(string accessToken)
    {
        access_token = accessToken;
    }

    public string access_token { get; set; }
}