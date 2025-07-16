using Dapper;
using KumAccess.Models;
using System.Data;
using System.Threading.Tasks;

namespace KumAccess.Services
{
    public class UserGroupGet
    {
        private readonly IDbConnection _db;

        public UserGroupGet(IDbConnection db)
        {
            _db = db;
        }

        public List<UserGroup> GetAllUserGroups()
        {
            string sql = "SELECT UserGroupId,UserId, GroupId FROM KumAccess.dbo.UserGroup";
            return _db.Query<UserGroup>(sql).ToList();
        }

    }
}
