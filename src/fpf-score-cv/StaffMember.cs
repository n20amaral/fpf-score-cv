public class StaffMember
{
    public int RegistrationId { get; set; }
    public int LicenseNr { get; set; }
    public int PersonEntityId { get; set; }
    public int? CoachLevelId { get; set; }
    public string PersonName { get; set; } = string.Empty;
    public string PersonFullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public int ClubEntityId { get; set; }
    public string ClubName { get; set; } = string.Empty;
    public int AssociationEntityId { get; set; }
    public int? OrganizationEntityId { get; set; }
    public string AssociationName { get; set; } = string.Empty;
    public int FederationEntityId { get; set; }
    public string FederationName { get; set; } = string.Empty;
    public string Sport { get; set; } = string.Empty;
    public int SportId { get; set; }
    public int CategoryId { get; set; }
    public string Category { get; set; } = string.Empty;
    public int TeamGenderId { get; set; }
    public int ProcessNumber { get; set; }
    public string ProcessNumberStr { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public int StatusId { get; set; }
    public string StatusDescription { get; set; } = string.Empty;
    public bool Urgent { get; set; }
    public DateTime UpdateDate { get; set; }
    public DateTime CreateDate { get; set; }
    public int RegistrationTypeId { get; set; }
    public string ProcessType { get; set; } = string.Empty;
    public string SportAgentTypeName { get; set; } = string.Empty;
    public int FunctionId { get; set; }
    public string FunctionName { get; set; } = string.Empty;
    public string FunctionMaleName { get; set; } = string.Empty;
    public string FunctionFemaleName { get; set; } = string.Empty;
    public int FunctionScopeId { get; set; }
    public bool? IsPaid { get; set; }
    public int HigherOrganizationId { get; set; }
    public string HigherOrganizationName { get; set; } = string.Empty;
    public int SubEntityTypeId { get; set; }
    public int SeasonId { get; set; }
    public string SeasonName { get; set; } = string.Empty;
    public string IdentificationValue { get; set; } = string.Empty;
    public DateTime MovementDate { get; set; }
    public int DivisionId { get; set; }
    public int InsuranceTypeId { get; set; }
    public string InsurancePolicy { get; set; } = string.Empty;
    public int InsuranceCompanyId { get; set; }
    public bool RequestAssCard { get; set; }
    public bool RequestAssBarCode { get; set; }
    public bool RequestFpfCard { get; set; }
    public int AolCode { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string CoachTypeName { get; set; } = string.Empty;
    public bool HasSBVDAE { get; set; }
    public bool ProcessWasWaitingCorrectionStatus { get; set; }
    public bool ProcessWasRejectedStatusMoreThanOnce { get; set; }
    public bool UserIsAssociation { get; set; }
    public int UserAssociationId { get; set; }
}