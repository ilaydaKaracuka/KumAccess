using System.Data;
using Dapper;
using KumAccess.Models;

namespace KumAccess.Services
{
    public class UserAppRoleGet
    {
        private readonly IDbConnection _db;

        public UserAppRoleGet(IDbConnection db)
        {
            _db = db;
        }

        public List<UserAppRole> GetAllUserAppRoles()
        {
            string sql = "SELECT UserAppRoleId, UserId,  ApplicationId, RoleId FROM KumAccess.dbo.UserAppRole";
            return _db.Query<UserAppRole>(sql).ToList();
        }

    }
}
