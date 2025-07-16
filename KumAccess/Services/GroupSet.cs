using Dapper;
using System.Data;
using System.Threading.Tasks;

namespace KumAccess.Services
{
    public class GroupSet
    {
        private readonly IDbConnection _db;

        public GroupSet(IDbConnection db)
        {
            _db = db;
        }

        public async Task AddGroupAsync(string groupName)
        {
            string sql = "INSERT INTO KumAccess.dbo.group3 (GroupName) VALUES (@GroupName)";
            await _db.ExecuteAsync(sql, new { GroupName = groupName });
        }
    }
}
