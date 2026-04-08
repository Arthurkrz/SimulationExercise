namespace SimulationExercise.Core.Contracts.Infrastructure
{
    public interface IContext : IDisposable
    {
        string GetConnectionString();
        IReadOnlyList<dynamic> Query(string sql, object param);
        IList<T> Query<T>(string sql, object param);
        IList<T> Query<T>(string sql);
        Task<IList<T>> QueryAsync<T>(string sql);
        Task<IList<T>> QueryAsync<T>(string sql, object param);
        T ExecuteScalar<T>(string sql, object? param = null, int? commandTimeout = null);
        int Execute(string sql, object? param = null, int? commandTimeout = null);
        Task<int> ExecuteAsync(string sql, object? param = null, int? commandTimeout = null);
        void Commit();
    }
}
