using Dapper;

namespace UserAuthDotBet2_WithDatabase.Repositories;

public interface ICategoryRepository
{
    public Task<List<Category>> get();
}

public class CategoryRepository : BaseRepository, ICategoryRepository
{
    public CategoryRepository(IConfiguration config) : base(config)
    {
    }

    async public Task<List<Category>> get()
    {
        var query = "SELECT * FROM categories";
        using var con = NewConnection;

        var res = await con.QueryAsync<Category>(query);
        return res.AsList();

    }
}