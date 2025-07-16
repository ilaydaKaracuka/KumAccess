using Dapper;
using System.Data;
using System.Threading.Tasks;

namespace KumAccess.Services
{
    public class RoleSet
    {
        private readonly IDbConnection _db;

        public RoleSet(IDbConnection db)
        {
            _db = db;
        }

        public async Task AddRoleAsync(string roleName)
        {
            var sql = "INSERT INTO KumAccess.dbo.Role (RoleName) VALUES (@RoleName)";
            await _db.ExecuteAsync(sql, new { RoleName = roleName});
        }
    }
}
