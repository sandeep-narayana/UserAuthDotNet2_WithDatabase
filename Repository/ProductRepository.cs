using Dapper;

namespace UserAuthDotBet2_WithDatabase.Repositories;

public interface IProductRepository
{
    public Task<List<Product>> get();
}

public class ProductRepository : BaseRepository, IProductRepository
{
    public ProductRepository(IConfiguration config) : base(config)
    {
    }

    async public Task<List<Product>> get()
    {
        var query = "SELECT * FROM products";
        using var con = NewConnection;

        var res = await con.QueryAsync<Product>(query);
        return res.AsList();

    }

}