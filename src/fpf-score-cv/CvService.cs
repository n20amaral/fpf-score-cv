public class CvService
{
    private HttpClient _httpClient;

    public CvService(HttpClient httpClient)
    {
        this._httpClient = httpClient;
    }

    internal Task<IEnumerable<StaffMember>> GetAllStaff()
    {
        throw new NotImplementedException();
    }

    internal Task<Cv> GetCv(int personEntityId)
    {
        throw new NotImplementedException();
    }
}