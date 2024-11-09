public class AuthService
{
    private const string LOGIN_URL = "Account/Login";
    private const string CONFIRMATION_URL = "Account/Auth";
    private HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        this._httpClient = httpClient;
    }

    internal async Task<(string userId, string token)> Authenticate(string username, string password)
    {
        var getLogin = await _httpClient.GetAsync(LOGIN_URL);
        var loginPage = await getLogin.Content.ReadAsStringAsync();
        var token = HtmlHelper.GetSingleNodeAttributeValue(loginPage, "//*[@id=\"loginForm\"]/form/input");

        var postLoginPayload = new FormUrlEncodedContent(new Dictionary<string, string> {
            {"__RequestVerificationToken",token},
            {"Username.Txt",username},
            {"Username",username},
            {"Password",password},
            {"submitLogin",""}
        });

        var postLogin = await _httpClient.PostAsync(LOGIN_URL, postLoginPayload);
        var confirmationPage = await postLogin.Content.ReadAsStringAsync();

        var confirmationHtml = HtmlHelper.LoadHtmlDocument(confirmationPage);
        var userId = HtmlHelper.GetSingleNodeAttributeValue(confirmationHtml, "//*[@id=\"UserId\"]", "value");
        token = HtmlHelper.GetSingleNodeAttributeValue(confirmationHtml, "//*[@id=\"formAuth\"]/input[4]", "value");

        return (userId, token);
    }

    internal async Task<bool> ConfirmCode(string confirmationCode, string userId, string token)
    {
        var confirmantionPayload = new FormUrlEncodedContent(new Dictionary<string, string>{
            {"UserId", userId},
            {"UserNane", ""},
            {"Count", "0"},
            {"__RequestVerificationToken", token},
            {"Token", confirmationCode}
        });

        var confirmationPost = await _httpClient.PostAsync(CONFIRMATION_URL, confirmantionPayload);
        var confirmationResult = await confirmationPost.Content.ReadAsStringAsync();

        return confirmationResult.StartsWith("{\"Success\":true");
    }
}