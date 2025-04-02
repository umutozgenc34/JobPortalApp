namespace JobPortalApp.Model.Companies.Dtos;


public sealed record CompanyReviewDto
{
    public int Id { get; init; }
    public int Rating { get; init; }
    public string Comment { get; init; }
    public string CompanyName { get; init; }
    public string UserName { get; init; }
    public DateTime CreatedAt { get; init; }
}