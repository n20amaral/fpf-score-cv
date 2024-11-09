
using System.Net;

var DEBUG_MODE = args.LastOrDefault() == "DEBUG";
var CV_PATH = args[0];

var cookieContainer = new CookieContainer();
var handler = new SocketsHttpHandler
{
    CookieContainer = cookieContainer,
    UseCookies = true,
    AllowAutoRedirect = false,
};

var httpClient = new HttpClient(handler, disposeHandler: false);
httpClient.BaseAddress = new Uri("https://score.fpf.pt/");

var authService = new AuthService(httpClient);
var cvService = new CvService(httpClient);
var pdfService = new PdfService(CV_PATH);


try
{
    HandleAuthentication();

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
        return;
    }

    var errorMessage = ex.Message.StartsWith("FSCV") ?
        ex.Message.Replace("FSCV", "ERROR") :
        "ERROR: Something went wrong..";

    Console.WriteLine(errorMessage);
}

void HandleAuthentication()
{
    var username = GetStringInput("Username");
    var password = GetSecureStringInput("Password");

    var token = authService.Authenticate(username, password);

    if (string.IsNullOrWhiteSpace(token))
    {
        throw new Exception("FSCV: Authentication failed. Username or Password are wrong.");
    }

    var confirmationCode = GetStringInput("Confirmation Code");

    var isConfirmed = authService.ConfirmCode(confirmationCode, token);

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

string GetSecureStringInput(string prompt)
{
    Console.Write($"{prompt}: ");

    var password = string.Empty;
    ConsoleKeyInfo keyInfo;

    do
    {
        keyInfo = Console.ReadKey(intercept: true);
        if (keyInfo.Key == ConsoleKey.Enter)
        {
            Console.WriteLine();
            break;
        }
        else if (keyInfo.Key == ConsoleKey.Backspace)
        {
            if (password.Length > 0)
            {
                password = password[0..^1];
                Console.Write("\b \b");
            }
        }
        else
        {
            password += keyInfo.KeyChar;
            Console.Write("*");
        }
    } while (true);

    return password;
}