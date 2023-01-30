using PaypalLab.ViewModels;

namespace PaypalLab.Repositories
{
    public class UserRepo
    {
        ApplicationDbContext _context;

        public UserRepo(ApplicationDbContext context)
        {
            this._context = context;
        }

        public List<UserVM> GetAllUsers()
        {
            IEnumerable<UserVM> users = _context.Users.Select(u => new UserVM() { Email = u.Email });

            return users.ToList();
        }

        public UserVM GetUserName(string email)
        {
            UserVM user = _context.MyRegisteredUsers.Select(u => new UserVM() { FirstName = u.FirstName, LastName = u.LastName }).FirstOrDefault();
            return user;

        }

    }
}
