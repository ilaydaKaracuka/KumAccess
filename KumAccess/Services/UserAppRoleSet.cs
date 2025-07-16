using Dapper;
using System.Data;
using System.Threading.Tasks;

namespace KumAccess.Services
{
    public class UserAppRoleSet
    {
        private readonly IDbConnection _db;

        public UserAppRoleSet(IDbConnection db)
        {
            _db = db;
        }

        public async Task AddUserAppRoleAsync(int userId, int applicationId, int roleId)
        {
            try
            {
                Console.WriteLine($"[DB] SQL INSERT: UserId={userId}, AppId={applicationId}, RoleId={roleId}");

                var sql = "INSERT INTO KumAccess.dbo.UserAppRole (UserId, ApplicationId, RoleId) VALUES (@UserId, @ApplicationId, @RoleId)";

                await _db.ExecuteAsync(sql, new { UserId = userId, ApplicationId = applicationId, RoleId = roleId });

                Console.WriteLine("[DB] EKLEME BAŞARILI");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[DB] SQL HATASI: " + ex.Message);
                throw;
            }
        }




    }
}
