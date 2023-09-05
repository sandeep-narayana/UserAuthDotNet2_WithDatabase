using Dapper;

namespace UserAuthDotBet2_WithDatabase.Repositories;

public interface ICartRepository
{
    public Task<List<Cart>> getCartById(int UserId);
}

public class CartRepository : BaseRepository, ICartRepository
{
    public CartRepository(IConfiguration config) : base(config)
    {
    }


    public async Task<List<Cart>> getCartById(int UserId)
    {
        var query = "SELECT * FROM cart where user_id = @UserId";
        using var con = NewConnection;

        var res = await con.QueryAsync<Cart>(query, new { UserId = UserId });
        return res.AsList();
    }
}