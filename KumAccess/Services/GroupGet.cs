using System.Data;
using Dapper;
using KumAccess.Models;

namespace KumAccess.Services
{
    public class GroupGet
    {
        private readonly IDbConnection _db;

        public GroupGet(IDbConnection db)
        {
            _db = db;
        }

        public List<Group> GetAllGroups()
        {
            string sql = "SELECT GroupId, GroupName FROM KumAccess.dbo.group3";
            var groups = _db.Query<Group>(sql).ToList();

            if (groups == null || !groups.Any())
            {
                Console.WriteLine("Grup verisi alınamadı!");
            }

            return groups;
        }

    }
}
