using System.Net.Http.Json;

public class CvService
{
    private const string CLUB_ENTITY_ID = "751";
    private const string ASSOCIATION_ENTITY_ID = "227";
    private const string SEASON_ID = "104";
    public const string STAFF_URL = "InscricoesDirigentes/Process";
    private const string STAFF_LIST_URL = "InscricoesDirigentes/Process/GetProcessList?token=undefined";
    private const string STAFF_DETAILS_URL = "InscricoesDirigentes/Process/Detail";
    private HttpClient _httpClient;
    private string _token;
    private Header _header;

    public CvService(HttpClient httpClient)
    {
        this._httpClient = httpClient;
        _header = new Header("FCF", "AFH", "2024", "2025");
    }

    internal async Task<IEnumerable<StaffMember>> GetAllStaff()
    {
        var staffPayload = new FormUrlEncodedContent(new Dictionary<string, string> {
            {"draw","2"},
            {"columns[0][data]","ClubEntityId"},
            {"columns[0][name]",""},
            {"columns[0][searchable]","true"},
            {"columns[0][orderable]","true"},
            {"columns[0][search][value]",CLUB_ENTITY_ID},
            {"columns[0][search][regex]","false"},
            {"columns[1][data]","AssociationEntityId"},
            {"columns[1][name]",""},
            {"columns[1][searchable]","true"},
            {"columns[1][orderable]","true"},
            {"columns[1][search][value]",ASSOCIATION_ENTITY_ID},
            {"columns[1][search][regex]","false"},
            {"start","0"},
            {"length","20"},
            {"search[value]",""},
            {"search[regex]","false"},
            {"seasonId",SEASON_ID},
            {"statusid","4"},
            {"exportGrid","false"},
            {"printGrid","false"},
        });

        var postStaff = await PostWithToken(STAFF_LIST_URL, staffPayload);
        var result = await postStaff.Content.ReadFromJsonAsync<StaffResult>();

        _ = result ?? throw new Exception($"FSCV: Cant load staff list. ({postStaff.StatusCode})");

        return result.Data;
    }

    internal async Task<Cv> GetCv(int personEntityId, int registrationId)
    {
        var staffDetails = await GetStaffDetails(registrationId);

        staffDetails.TryGetValue("Name", out var name);
        staffDetails.TryGetValue("DateOfBirth", out var birth);
        staffDetails.TryGetValue("RegistrationDate", out var registration);
        staffDetails.TryGetValue("CellPhone", out var cellPhont);
        staffDetails.TryGetValue("Email", out var email);
        staffDetails.TryGetValue("Address", out var address);
        staffDetails.TryGetValue("Location", out var location);
        staffDetails.TryGetValue("ZipCodeId", out var zip);
        staffDetails.TryGetValue("SubEntityTypeId", out var subEntity);
        staffDetails.TryGetValue("FunctionName", out var functionName);


        var hasDateOfBirth = DateOnly.TryParseExact(birth, "dd-MM-yyyy", out var dateOfBirth);
        var hasRegistrationDate = DateOnly.TryParseExact(registration, "dd-MM-yyyy", out var registrationDate);



        var personalData = new PersonalData
        (
            Name: name ?? string.Empty,
            DayOfBirth: hasDateOfBirth ? dateOfBirth.Day.ToString() : string.Empty,
            MonthOfBirth: hasDateOfBirth ? dateOfBirth.Month.ToString() : string.Empty,
            YearOfBirth: hasDateOfBirth ? dateOfBirth.Year.ToString() : string.Empty,
            Nationality: "Portuguesa" ?? string.Empty,
            Phone: cellPhont ?? string.Empty,
            Email: email ?? string.Empty,
            Address: $"{address}, {location} {zip}"
        );

        var staffAssignment = new StaffAssignment
        (
            Department: subEntity ?? string.Empty,
            StartDate: hasRegistrationDate ? registrationDate : null,
            Roles: new string[6] { functionName ?? string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty }
        );

        return new Cv
        {
            Header = _header,
            PersonalData = personalData,
            StaffAssignment = staffAssignment
        };

    }

    private async Task<Dictionary<string, string>> GetStaffDetails(int registrationId)
    {
        var getStaffDetails = await _httpClient.GetAsync($"{STAFF_DETAILS_URL}/{registrationId}");
        var staffDetailsPage = await getStaffDetails.Content.ReadAsStringAsync();

        return HtmlHelper.GetAttributeAndTextValuesFromNodeCollection(staffDetailsPage, "//div[contains(@class, 'form-field-row') and contains(@class, 'ffr-four-columns')]", ".//label", "for");
    }

    private async Task<HttpResponseMessage> PostWithToken(string requestUri, HttpContent content)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, STAFF_LIST_URL);
        request.Headers.Add("__RequestVerificationToken", await GetToken());
        request.Content = content;

        return await _httpClient.SendAsync(request);
    }

    private async Task<string> GetToken()
    {
        if (!string.IsNullOrWhiteSpace(_token))
        {
            return _token;
        }

        var getStaff = await _httpClient.GetAsync(STAFF_URL);
        var staffPage = await getStaff.Content.ReadAsStringAsync();

        return HtmlHelper.GetSingleNodeAttributeValue(staffPage, "//*[@id=\"logoutForm\"]/input");
    }
}