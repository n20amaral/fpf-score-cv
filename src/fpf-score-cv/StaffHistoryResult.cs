public class StaffHistoryResult
{
    public bool IsSuccess { get; set; }
    public DrawData? Data { get; set; }
    public int RecordsTotal { get; set; }
    public int RecordsFiltered { get; set; }
    public class DrawData
    {
        public int Draw { get; set; }
        public IEnumerable<StaffHistoryRecord> Data { get; set; } = new List<StaffHistoryRecord>();
    }
}
