using System.Data;
using Dapper;
using KumAccess.Models;

namespace KumAccess.Services
{
    public class RoleGet
    {
        private readonly IDbConnection _db;

        public RoleGet(IDbConnection db)
        {
            _db = db;
        }

        public List<Role> GetAllRoles()
        {
            string sql = "SELECT RoleId, RoleName FROM KumAccess.dbo.Role";
            return _db.Query<Role>(sql).ToList();
        }
    }
}
