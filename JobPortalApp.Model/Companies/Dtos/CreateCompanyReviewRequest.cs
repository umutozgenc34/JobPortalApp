namespace JobPortalApp.Model.Companies.Dtos;

public sealed record CreateCompanyReviewRequest(int Rating, string Comment, int CompanyId, string UserId);
