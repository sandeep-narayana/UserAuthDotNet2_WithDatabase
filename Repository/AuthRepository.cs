
using Dapper;
using static UserAuthDotBet2_WithDatabase.AuthController;

namespace UserAuthDotBet2_WithDatabase.Repositories;

public interface IAuthRepository
{
    Task<bool> CheckAuthentication(UserCredentials userCredentials);
    Task<bool> RegisterUser(User user);
}

public class AuthRepository : BaseRepository, IAuthRepository
{
    public AuthRepository(IConfiguration config) : base(config)
    {
    }

    public async Task<bool> CheckAuthentication(UserCredentials userCredentials)
    {
        var query = "SELECT * FROM users WHERE email = @Email AND password = @Password";

        var parameters = new
        {
            Email = userCredentials.Email,
            Password = userCredentials.Password
        };


        using var con = NewConnection;
        return await con.QueryFirstOrDefaultAsync(query, parameters) != null;
    }

    public async Task<bool> RegisterUser(User user)
    {
        // SQL query to insert a new user into the database.
        var query = "INSERT INTO users (username ,email, password,) VALUES (@Username,@Email, @Password)";

        var parameters = new
        {
            Username = user.Username,
            Email = user.Email,
            Password = user.Password
        };

        using var con = NewConnection;
        await con.ExecuteAsync(query, parameters);
        return true;

    }
}