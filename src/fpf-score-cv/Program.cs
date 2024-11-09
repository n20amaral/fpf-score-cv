using System.Net;
using System.Text.Json;

var DEBUG_MODE = args.LastOrDefault() == "DEBUG";
var CV_PATH = args.Any() ? args[0] : "CV.pdf";
var JSON_PATH = "collection.json";

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


// try
// {
var cvList = new List<Cv>();
if (!File.Exists(JSON_PATH))
{
    await HandleAuthentication();
    var staffList = await cvService.GetAllStaff();


    foreach (var staff in staffList
        .GroupBy(s => s.PersonEntityId)
        .Select(g => new { PersonId = g.Key, LastRegistrationId = g.Max(s => s.RegistrationId), Roles = g.Select(s => s.FunctionName) }))
    {
        Console.WriteLine($"Getting CV data for: {staff.PersonId}");
        await Task.Delay(1000);
        var cv = await cvService.GetCv(staff.PersonId, staff.LastRegistrationId, staff.Roles);
        cvList.Add(cv);
        Console.WriteLine($"{cv.PersonalData.Name} is done: {string.Join(',', cv.StaffAssignment.Roles)} with {cv.SportsQualifications.Count()} experiences");
    }

    await SaveToFile(cvList);
    Console.WriteLine($"{cvList.Count} cvs saved");
}
else
{
    cvList = await LoadFromFile();
}

foreach (var cv in cvList)
{
    pdfService.GeneratePdf(cv);
}
//}
// catch (Exception ex)
// {
//     if (DEBUG_MODE)
//     {
//         Console.Write(ex.Message);
//     }

//     var errorMessage = ex.Message.StartsWith("FSCV") ?
//         ex.Message.Replace("FSCV", "ERROR") :
//         "ERROR: Something went wrong..";

//     Console.WriteLine(errorMessage);
//     throw;
// }

async Task SaveToFile(List<Cv> cvs)
{
    var options = new JsonSerializerOptions { WriteIndented = true };
    string jsonString = JsonSerializer.Serialize(cvs, options);
    await File.WriteAllTextAsync(JSON_PATH, jsonString);
}

async Task<List<Cv>> LoadFromFile()
{
    string jsonString = await File.ReadAllTextAsync(JSON_PATH);
    return JsonSerializer.Deserialize<List<Cv>>(jsonString) ?? [];
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