using System.Data;
using Dapper;
using KumAccess.Models;

namespace KumAccess.Services
{
    public class ApplicationGet
    {
        private readonly IDbConnection _db;

        public ApplicationGet(IDbConnection db)
        {
            _db = db;
        }

        public List<App> GetAllApplications()
        {
            string sql = "SELECT ApplicationId, ApplicationName FROM KumAccess.dbo.Application";
            return _db.Query<App>(sql).ToList();
        }
    }
}
