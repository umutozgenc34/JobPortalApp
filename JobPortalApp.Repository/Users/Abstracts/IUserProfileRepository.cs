using JobPortalApp.Model.Users.Entities;
using JobPortalApp.Shared.Repositories.Abstracts;

namespace JobPortalApp.Repository.Users.Abstracts;

public interface IUserProfileRepository : IGenericRepository<UserProfile,int>
{
}
