using JobPortalApp.Model.Users.Entities;
using JobPortalApp.Repository.Contexts;
using JobPortalApp.Repository.Users.Abstracts;
using JobPortalApp.Shared.Repositories.Concretes;

namespace JobPortalApp.Repository.Users.Concretes;

public class UserProfileRepository(AppDbContext context) : GenericRepository<AppDbContext,UserProfile,int>(context), IUserProfileRepository
{
}
