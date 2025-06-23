namespace SimulationExercise.Core.Contracts.Repository
{
    public interface IContext : IDisposable
    {
        string GetConnectionString();
        IReadOnlyList<dynamic> Query(string sql, object param);
        IList<T> Query<T>(string sql, object param);
        IList<T> Query<T>(string sql);
        T ExecuteScalar<T>(string sql, object param = null, int? commandTimeout = null);
        int Execute(string sql, object param = null, int? commandTimeout = null);
        void Commit();
    }
}
