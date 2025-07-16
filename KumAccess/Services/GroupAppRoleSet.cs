using Dapper;
using KumAccess.Models;
using System.Data;
using System.Threading.Tasks;

namespace KumAccess.Services
{
    public class GroupAppRoleSet
    {
        private readonly IDbConnection _db;

        public GroupAppRoleSet(IDbConnection db)
        {
            _db = db;
        }

        public async Task AddGroupAppRoleAsync(int groupId, int applicationId, int roleId)
        {
            try
            {
                Console.WriteLine($"[DB] SQL INSERT: GroupId={groupId}, AppId={applicationId}, RoleId={roleId}");

                var sql = "INSERT INTO KumAccess.dbo.GroupAppRole (GroupId, ApplicationId, RoleId) VALUES (@GroupId, @ApplicationId, @RoleId)";

                await _db.ExecuteAsync(sql, new { GroupId = groupId, ApplicationId = applicationId, RoleId = roleId });

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


