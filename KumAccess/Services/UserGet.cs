using System.Data;
using Dapper;
using KumAccess.Models;

namespace KumAccess.Services
{
    public class UserGet
    {
        private readonly IDbConnection _db;

        public UserGet(IDbConnection db)
        {
            _db = db;
        }

        public List<User> GetAllUsers()
        {
            string sql = "SELECT UserId, TC, FirstName, LastName FROM KumAccess.dbo.Users";
            return _db.Query<User>(sql).ToList();
        }
    }
}
