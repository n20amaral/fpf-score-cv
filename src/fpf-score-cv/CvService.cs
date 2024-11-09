using System.Net.Http.Json;

public class CvService
{
    private const string CLUB_ENTITY_ID = "751";
    private const string ASSOCIATION_ENTITY_ID = "227";
    private const string SEASON_ID = "104";
    public const string STAFF_URL = "InscricoesDirigentes/Process";
    private const string STAFF_LIST_URL = "InscricoesDirigentes/Process/GetProcessList?token=undefined";
    private const string STAFF_DETAILS_URL = "InscricoesDirigentes/Process/Detail";
    private const string STAFF_HISTORY_URL = "InscricoesDirigentes/Process/GetRegistrationHistory";
    private HttpClient _httpClient;
    private string _token = string.Empty;
    private Header _header;

    public CvService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _header = new Header("Futebol Clube dos Flamengos", "Associação de Futebol da Horta", "2024", "2025");
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
            {"length","50"},
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

    internal async Task<Cv> GetCv(int personEntityId, int registrationId, IEnumerable<string> roles)
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
            Roles: roles.Take(6)
        );

        var cv = new Cv
        {
            Header = _header,
            PersonalData = personalData,
            StaffAssignment = staffAssignment
        };

        var maxHistoryRecords = cv.SportsQualifications.Count();
        var staffHistory = await GetStaffHistory(personEntityId, maxHistoryRecords);
        cv.SportsQualifications = staffHistory;

        return cv;
    }

    private async Task<Dictionary<string, string>> GetStaffDetails(int registrationId)
    {
        var getStaffDetails = await _httpClient.GetAsync($"{STAFF_DETAILS_URL}/{registrationId}");
        var staffDetailsPage = await getStaffDetails.Content.ReadAsStringAsync();

        return HtmlHelper.GetAttributeAndTextValuesFromNodeCollection(staffDetailsPage, "//div[contains(@class, 'form-field-row') and contains(@class, 'ffr-four-columns')]", ".//label", "for");
    }

    private async Task<IEnumerable<ProfessionalExperience>> GetStaffHistory(int personEntityId, int maxHistoryRecords)
    {
        var historyPayload = new FormUrlEncodedContent(new Dictionary<string, string> {
            {"draw","2"},
            {"order[0][column]", "1"},
            {"order[0][dir]", "desc"},
            {"start", "0"},
            {"length", "10"},
            {"search[value]", ""},
            {"search[regex]", "false"}
        });
        var postStaffHistory = await PostWithToken($"{STAFF_HISTORY_URL}?personEntityId={personEntityId}", historyPayload);
        var staffHistoryResult = await postStaffHistory.Content.ReadFromJsonAsync<StaffHistoryResult>();

        return staffHistoryResult?.Data?.Data.Select(h =>
            {
                var seasons = h.SeasonName.Split('-');
                var starDate = int.TryParse(seasons.FirstOrDefault(), out var starYear) ? new DateOnly(starYear, 8, 1) : DateOnly.MinValue;
                var endDate = int.TryParse(seasons.LastOrDefault(), out var endYear) ? new DateOnly(endYear, 6, 30) : DateOnly.MaxValue;

                return new ProfessionalExperience(h.EntityName, h.FunctionName, starDate, endDate);
            })
        .OrderByDescending(h => h.EndDate)
        .Take(maxHistoryRecords) ?? [];
    }

    private async Task<HttpResponseMessage> PostWithToken(string requestUri, HttpContent content, string tokenUrl = STAFF_URL)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
        request.Headers.Add("__RequestVerificationToken", await GetToken(tokenUrl));
        request.Content = content;

        return await _httpClient.SendAsync(request);
    }

    private async Task<string> GetToken(string tokenUrl = STAFF_URL)
    {
        var getStaff = await _httpClient.GetAsync(tokenUrl);
        var staffPage = await getStaff.Content.ReadAsStringAsync();

        return HtmlHelper.GetSingleNodeAttributeValue(staffPage, "//*[@id=\"logoutForm\"]/input");
    }
}