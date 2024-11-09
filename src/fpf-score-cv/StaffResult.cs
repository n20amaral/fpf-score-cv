public class StaffResult
{
    public IEnumerable<StaffMember> Data { get; set; } = new List<StaffMember>();
    public string ExportId { get; set; } = string.Empty;
    public string PrintId { get; set; } = string.Empty;
    public int Draw { get; set; }
    public int RecordsTotal { get; set; }
    public int RecordsFiltered { get; set; }
}