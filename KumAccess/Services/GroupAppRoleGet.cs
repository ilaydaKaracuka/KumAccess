using System.Data;
using Dapper;
using KumAccess.Models;

namespace KumAccess.Services
{
    public class GroupAppRoleGet
    {
        private readonly IDbConnection _db;

        public GroupAppRoleGet(IDbConnection db)
        {
            _db = db;
        }

        public List<GroupAppRole> GetAllGroupAppRoles()
        {
            string sql = "SELECT GroupAppRoleId, GroupId, ApplicationId, RoleId FROM KumAccess.dbo.GroupAppRole";
            var groupAppRoles = _db.Query<GroupAppRole>(sql).ToList();

            if (groupAppRoles == null || !groupAppRoles.Any())
            {
                Console.WriteLine("GroupAppRole verisi alınamadı!");
            }

            return groupAppRoles;
        }
    }
}
