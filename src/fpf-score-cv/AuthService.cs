public class AuthService
{
    private HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        this._httpClient = httpClient;
    }

    internal string Authenticate(string username, string password)
    {
        throw new NotImplementedException();
    }

    internal bool ConfirmCode(string confirmationCode, string token)
    {
        throw new NotImplementedException();
    }
}