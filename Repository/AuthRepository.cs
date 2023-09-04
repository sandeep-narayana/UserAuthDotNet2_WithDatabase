
using Dapper;
using static UserAuthDotBet2_WithDatabase.AuthController;

namespace UserAuthDotBet2_WithDatabase.Repositories;

public interface IAuthRepository
{
    Task<bool> CheckAuthentication(string Username, string Password);
    Task<bool> RegisterUser(UserCredentials userCredentials);
}

public class AuthRepository : BaseRepository, IAuthRepository
{
    public AuthRepository(IConfiguration config) : base(config)
    {
    }

    public async Task<bool> CheckAuthentication(string Username, string Password)
    {
        var query = "SELECT * FROM users WHERE username = @Username AND password = @Password";

        using var con = NewConnection;
        return await con.QueryFirstOrDefaultAsync(query, new { Username, Password }) != null;
    }

    public async Task<bool> RegisterUser(UserCredentials userCredentials)
    {
        // SQL query to insert a new user into the database.
        var query = "INSERT INTO users (email, password) VALUES (@Username, @Password)";

        var parameters = new
        {
            Username = userCredentials.Username,
            Password = userCredentials.Password
        };

        using var con = NewConnection;
        await con.ExecuteAsync(query, parameters);
        return true;

    }
}