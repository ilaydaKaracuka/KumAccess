using Dapper;
using System.Data;
using System.Threading.Tasks;

namespace KumAccess.Services
{
    public class UserGroupSet
    {
        private readonly IDbConnection _db;

        public UserGroupSet(IDbConnection db)
        {
            _db = db;
        }

        public async Task AddUserGroupAsync(int userId, int groupId)
        {
            var sql = "INSERT INTO KumAccess.dbo.UserGroup (UserId, GroupId) VALUES (@UserId, @GroupId)";
            await _db.ExecuteAsync(sql, new { UserId = userId, GroupId = groupId });
        }

    }
}
