using System.Text.RegularExpressions;
using Spire.Pdf;
using Spire.Pdf.Fields;
using Spire.Pdf.Widget;

public class PdfService
{
    private readonly string cvPath;

    public PdfService(string cvPath)
    {
        this.cvPath = cvPath;
    }

    internal void GeneratePdf(Cv cv)
    {
        PdfDocument pdf = new PdfDocument();
        pdf.LoadFromFile(cvPath);

        PdfFormWidget? formWidget = pdf.Form as PdfFormWidget;

        _ = formWidget ?? throw new Exception("FSCV: Pdf incorrect format. Does not have a form.");

        foreach (PdfField field in formWidget.FieldsWidget.List)
        {
            if (field is PdfTextBoxFieldWidget textBoxField)
            {
                string fieldName = textBoxField.Name;
                textBoxField.Text = GetFieldValue(cv, fieldName);
            }
        }

        var firstName = cv.PersonalData.Name.Split(' ').FirstOrDefault();
        var lastName = cv.PersonalData.Name.Split(' ').LastOrDefault();

        pdf.SaveToFile($"_FCF - {firstName} {lastName} - CV.pdf");
        pdf.Close();
    }

    private string GetFieldValue(Cv cv, string textFieldId)
    {
        if (!int.TryParse(Regex.Match(textFieldId, @"\d+").Value, out var id))
        {
            return string.Empty;
        }

        return id switch
        {
            2 => cv.Header.Entity,
            3 => cv.Header.Association,
            5 => cv.Header.SeasonEndYear,
            6 => cv.Header.SeasonStartYear,
            7 => cv.PersonalData.Name,
            8 => cv.PersonalData.DayOfBirth,
            9 => cv.PersonalData.MonthOfBirth,
            10 => cv.PersonalData.YearOfBirth,
            11 => cv.PersonalData.Nationality,
            12 => cv.PersonalData.Email,
            13 => cv.PersonalData.Phone,
            14 => cv.PersonalData.Address,
            15 => cv.StaffAssignment.Department,
            16 => cv.StaffAssignment.StartDate?.Month.ToString() ?? string.Empty,
            17 => cv.StaffAssignment.StartDate?.Year.ToString() ?? string.Empty,
            19 => cv.StaffAssignment.Roles.ElementAtOrDefault(0) ?? string.Empty,
            24 => cv.StaffAssignment.Roles.ElementAtOrDefault(1) ?? string.Empty,
            26 => cv.StaffAssignment.Roles.ElementAtOrDefault(2) ?? string.Empty,
            23 => cv.StaffAssignment.Roles.ElementAtOrDefault(3) ?? string.Empty,
            25 => cv.StaffAssignment.Roles.ElementAtOrDefault(4) ?? string.Empty,
            27 => cv.StaffAssignment.Roles.ElementAtOrDefault(5) ?? string.Empty,
            64 => cv.SportsQualifications.ElementAtOrDefault(0)?.Entity ?? string.Empty,
            66 => cv.SportsQualifications.ElementAtOrDefault(0)?.Role ?? string.Empty,
            88 => cv.SportsQualifications.ElementAtOrDefault(0)?.StartDate.Month.ToString() ?? string.Empty,
            89 => cv.SportsQualifications.ElementAtOrDefault(0)?.StartDate.Year.ToString() ?? string.Empty,
            90 => cv.SportsQualifications.ElementAtOrDefault(0)?.EndDate.Month.ToString() ?? string.Empty,
            91 => cv.SportsQualifications.ElementAtOrDefault(0)?.EndDate.Year.ToString() ?? string.Empty,
            92 => cv.SportsQualifications.ElementAtOrDefault(1)?.Entity ?? string.Empty,
            93 => cv.SportsQualifications.ElementAtOrDefault(1)?.Role ?? string.Empty,
            94 => cv.SportsQualifications.ElementAtOrDefault(1)?.StartDate.Month.ToString() ?? string.Empty,
            96 => cv.SportsQualifications.ElementAtOrDefault(1)?.StartDate.Year.ToString() ?? string.Empty,
            95 => cv.SportsQualifications.ElementAtOrDefault(1)?.EndDate.Month.ToString() ?? string.Empty,
            97 => cv.SportsQualifications.ElementAtOrDefault(1)?.EndDate.Year.ToString() ?? string.Empty,
            98 => cv.SportsQualifications.ElementAtOrDefault(2)?.Entity ?? string.Empty,
            99 => cv.SportsQualifications.ElementAtOrDefault(2)?.Role ?? string.Empty,
            100 => cv.SportsQualifications.ElementAtOrDefault(2)?.StartDate.Month.ToString() ?? string.Empty,
            102 => cv.SportsQualifications.ElementAtOrDefault(2)?.StartDate.Year.ToString() ?? string.Empty,
            101 => cv.SportsQualifications.ElementAtOrDefault(2)?.EndDate.Month.ToString() ?? string.Empty,
            103 => cv.SportsQualifications.ElementAtOrDefault(2)?.EndDate.Year.ToString() ?? string.Empty,
            104 => cv.SportsQualifications.ElementAtOrDefault(3)?.Entity ?? string.Empty,
            105 => cv.SportsQualifications.ElementAtOrDefault(3)?.Role ?? string.Empty,
            106 => cv.SportsQualifications.ElementAtOrDefault(3)?.StartDate.Month.ToString() ?? string.Empty,
            108 => cv.SportsQualifications.ElementAtOrDefault(3)?.StartDate.Year.ToString() ?? string.Empty,
            107 => cv.SportsQualifications.ElementAtOrDefault(3)?.EndDate.Month.ToString() ?? string.Empty,
            109 => cv.SportsQualifications.ElementAtOrDefault(3)?.EndDate.Year.ToString() ?? string.Empty,
            110 => cv.SportsQualifications.ElementAtOrDefault(4)?.Entity ?? string.Empty,
            111 => cv.SportsQualifications.ElementAtOrDefault(4)?.Role ?? string.Empty,
            112 => cv.SportsQualifications.ElementAtOrDefault(4)?.StartDate.Month.ToString() ?? string.Empty,
            114 => cv.SportsQualifications.ElementAtOrDefault(4)?.StartDate.Year.ToString() ?? string.Empty,
            113 => cv.SportsQualifications.ElementAtOrDefault(4)?.EndDate.Month.ToString() ?? string.Empty,
            115 => cv.SportsQualifications.ElementAtOrDefault(4)?.EndDate.Year.ToString() ?? string.Empty,
            116 => cv.SportsQualifications.ElementAtOrDefault(5)?.Entity ?? string.Empty,
            117 => cv.SportsQualifications.ElementAtOrDefault(5)?.Role ?? string.Empty,
            1010 => cv.SportsQualifications.ElementAtOrDefault(5)?.StartDate.Month.ToString() ?? string.Empty,
            1012 => cv.SportsQualifications.ElementAtOrDefault(5)?.StartDate.Year.ToString() ?? string.Empty,
            1011 => cv.SportsQualifications.ElementAtOrDefault(5)?.EndDate.Month.ToString() ?? string.Empty,
            1013 => cv.SportsQualifications.ElementAtOrDefault(5)?.EndDate.Year.ToString() ?? string.Empty,
            1014 => cv.SportsQualifications.ElementAtOrDefault(6)?.Entity ?? string.Empty,
            1015 => cv.SportsQualifications.ElementAtOrDefault(6)?.Role ?? string.Empty,
            1016 => cv.SportsQualifications.ElementAtOrDefault(6)?.StartDate.Month.ToString() ?? string.Empty,
            1018 => cv.SportsQualifications.ElementAtOrDefault(6)?.StartDate.Year.ToString() ?? string.Empty,
            1017 => cv.SportsQualifications.ElementAtOrDefault(6)?.EndDate.Month.ToString() ?? string.Empty,
            1019 => cv.SportsQualifications.ElementAtOrDefault(6)?.EndDate.Year.ToString() ?? string.Empty,
            118 => cv.SportsQualifications.ElementAtOrDefault(7)?.Entity ?? string.Empty,
            119 => cv.SportsQualifications.ElementAtOrDefault(7)?.Role ?? string.Empty,
            120 => cv.SportsQualifications.ElementAtOrDefault(7)?.StartDate.Month.ToString() ?? string.Empty,
            122 => cv.SportsQualifications.ElementAtOrDefault(7)?.StartDate.Year.ToString() ?? string.Empty,
            121 => cv.SportsQualifications.ElementAtOrDefault(7)?.EndDate.Month.ToString() ?? string.Empty,
            123 => cv.SportsQualifications.ElementAtOrDefault(7)?.EndDate.Year.ToString() ?? string.Empty,
            154 => cv.Observations ?? string.Empty,
            _ => string.Empty
        };
    }
}