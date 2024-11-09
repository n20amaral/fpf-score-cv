public class Cv
{
    public required Header Header { get; set; }
    public required PersonalData PersonalData { get; set; }
    public required StaffAssignment StaffAssignment { get; set; }
    public IEnumerable<AcademicQualification> AcademicQualifications { get; set; } = new AcademicQualification[8];
    public IEnumerable<ProfessionalExperience> SportsQualifications { get; set; } = new ProfessionalExperience[8];
    public IEnumerable<ProfessionalExperience> OtherQualifications { get; set; } = new ProfessionalExperience[3];
    public string Observations { get; set; } = string.Empty;
}

public record Header(
    string Entity,
    string Association,
    string SeasonStartYear,
    string SeasonEndYear
);

public record PersonalData(
    string Name,
    string DayOfBirth,
    string MonthOfBirth,
    string YearOfBirth,
    string Nationality,
    string Phone,
    string Email,
    string Address
);

public record StaffAssignment(
    string Department,
    DateOnly? StartDate,
    IEnumerable<string> Roles
);

public record AcademicQualification(
    string Course,
    string EndYear,
    string Observations
);

public record ProfessionalExperience(
    string Entity,
    string Role,
    DateOnly StartDate,
    DateOnly EndDate
);