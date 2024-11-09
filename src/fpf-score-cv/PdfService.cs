public class PdfService
{
    private readonly string cvPath;

    public PdfService(string cvPath)
    {
        this.cvPath = cvPath;
    }

    internal void GeneratePdf(Cv cv)
    {
        throw new NotImplementedException();
    }
}