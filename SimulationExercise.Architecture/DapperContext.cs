using SimulationExercise.Core.Contracts.Repository;

namespace SimulationExercise.Architecture
{
    public class DapperContext : IContext
    {
        private bool _isTransactionCommited;
        private bool _isDisposed;

        public DapperContext(SqlTransaction transaction)
        {
            if (transaction == null) throw new
                    ArgumentNullException(nameof(transaction));

            this.Connection = transaction.Connection;
            this.Transaction = transaction;
            _isDisposed = false;
            _isTransactionCommited = false;
        }

        ~DapperContext() { Dispose(false); }

        public SqlConnection Connection { get; }
        public SqlTransaction Transaction { get; }

        public IReadOnlyList<dynamic> Query(string sql, object param) =>
                        Connection.Query(sql, param, Transaction).ToList();
        public IList<T> Query<T>(string sql, object param) =>
                        Connection.Query<T>(sql, param, Transaction).ToList();
        public IList<T> Query<T>(string sql) =>
                        Connection.Query<T>(sql, null, Transaction).ToList();
        public T ExecuteScalar<T>(string sql, object param = null, int? commandTimeout = null) =>
            (T)Connection.ExecuteScalar(sql, param, Transaction, commandTimeout: commandTimeout);
        public int Execute(string sql, object param = null, int? commandTimeout = null) => 
            Connection.Execute(sql, param, Transaction, commandTimeout: commandTimeout);
        public void Commit()
        {
            Transaction.Commit();
            _isTransactionCommited = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                if (Transaction != null)
                {
                    if (!_isTransactionCommited) Transaction.Rollback();
                    Transaction.Dispose();
                }

                if (Connection != null) { Connection.Dispose(); }
            }

            _isDisposed = true;
        } 
    }
}
