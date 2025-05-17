using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationExercise.Core.Contracts.Repository
{
    public interface IContext : IDisposable
    {
        IReadOnlyList<dynamic> Query(string sql, object param);
        IList<T> Query<T>(string sql, object param);
        IList<T> Query<T>(string sql);
        T ExecuteScalar<T>(string sql, object param = null, int? commandTimeout = null);
        int Execute(string sql, object param = null, int? commandTimeout = null);
        void Commit();
    }
}
