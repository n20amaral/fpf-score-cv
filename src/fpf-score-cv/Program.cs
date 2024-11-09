using System.Net;

var DEBUG_MODE = args.LastOrDefault() == "DEBUG";
var CV_PATH = args.Any() ? args[0] : "CV.pdf";

var cookieContainer = new CookieContainer();
var handler = new SocketsHttpHandler
{
    CookieContainer = cookieContainer,
    UseCookies = true,
    AllowAutoRedirect = true,
};

var httpClient = new HttpClient(handler, disposeHandler: false);
httpClient.BaseAddress = new Uri("https://score.fpf.pt/");

var authService = new AuthService(httpClient);
var cvService = new CvService(httpClient);
var pdfService = new PdfService(CV_PATH);


try
{
    await HandleAuthentication();

    var staffList = await cvService.GetAllStaff();
    var cvList = await Task.WhenAll(staffList.Select(s => cvService.GetCv(s.PersonEntityId)));

    foreach (var cv in cvList)
    {
        pdfService.GeneratePdf(cv);
    }
}
catch (Exception ex)
{
    if (DEBUG_MODE)
    {
        Console.Write(ex.Message);
    }

    var errorMessage = ex.Message.StartsWith("FSCV") ?
        ex.Message.Replace("FSCV", "ERROR") :
        "ERROR: Something went wrong..";

    Console.WriteLine(errorMessage);
}

async Task HandleAuthentication()
{
    var username = GetStringInput("Username");
    var password = GetStringInput("Password");

    var (userId, token) = await authService.Authenticate(username, password);

    if (DEBUG_MODE)
    {
        Console.WriteLine($"userId: {userId}\ntoken: {token}");
    }

    if (string.IsNullOrWhiteSpace(token))
    {
        throw new Exception("FSCV: Authentication failed. Username or Password are wrong.");
    }

    var confirmationCode = GetStringInput("Confirmation Code");

    var isConfirmed = await authService.ConfirmCode(confirmationCode, userId, token);

    if (!isConfirmed)
    {
        throw new Exception("FSCV: Authentication failed. Confirmation Code is wrong");
    }
}

string GetStringInput(string prompt)
{
    var input = string.Empty;

    while (string.IsNullOrWhiteSpace(input))
    {
        Console.Write($"{prompt}: ");
        input = Console.ReadLine();
    }

    return input;
}