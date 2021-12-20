using System.Threading.Tasks;
using Dapper;
using MiaCore.Models;
using MiaCore.Utils;
using Microsoft.Extensions.Options;

namespace MiaCore.Infrastructure.Persistence
{
    internal class UserRepository : GenericRepository<MiaUser>, IUserRepository
    {
        public UserRepository(IOptions<MiaCoreOptions> options) : base(options)
        {
        }

        public async Task<MiaUser> GetByEmailAsync(string email)
        {
            using var conn = GetConnection();
            var query = "select * from " + Tablename + " where email = @email";
            return await conn.QueryFirstOrDefaultAsync<MiaUser>(query, new { email });
        }

        public async Task<MiaUser> LoginAsync(string email, string password)
        {
            password = Hashing.GenerateSha256(password);
            using var conn = GetConnection();
            var query = "select * from " + Tablename + " where email = @email and password = @password";
            return await conn.QueryFirstOrDefaultAsync<MiaUser>(query, new { email, password });
        }
    }
}