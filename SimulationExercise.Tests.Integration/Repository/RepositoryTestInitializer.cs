using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SimulationExercise.Architecture;

namespace SimulationExercise.Tests.Integration.Repository
{
    public class RepositoryTestInitializer
    {
        private readonly string _connectionString;
        private readonly string _connectionStringMaster;

        public RepositoryTestInitializer() 
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            _connectionString = config.GetConnectionString("Test");
            _connectionStringMaster = config.GetConnectionString("Master");
        }

        public void Initialize()
        {
            const string testDatabaseName = "Basis_Test";

            TestDatabaseCreationIfNotCreated(testDatabaseName);
            TestDatabaseTableCreationIfNotCreated(_connectionString);
        }

        private void TestDatabaseCreationIfNotCreated(string databaseName)
        {
            using (var connectionString = new SqlConnection(_connectionStringMaster))
            {
                connectionString.Open();

                var query = $"IF DB_ID('{databaseName}') IS NULL CREATE DATABASE [{databaseName}]";
                connectionString.Execute(query);

                var grantAccessQuery = $@"ALTER AUTHORIZATION ON DATABASE [{databaseName}] TO [{Environment.UserDomainName}\{Environment.UserName}];";

                connectionString.Execute(grantAccessQuery);
            }
        }

        private void TestDatabaseTableCreationIfNotCreated(string databaseConnectionString)
        {
            using (var connectionString = new SqlConnection(databaseConnectionString))
            {
                connectionString.Open();

                string tableCreationQuery =
                    "IF OBJECT_ID('dbo.BasisDataTest', 'U') IS NULL " +
                    "CREATE TABLE dbo.BasisDataTest " +
                        "(BASISID BIGINT PRIMARY KEY, " +
                        "BASISCODE NVARCHAR(50) NOT NULL, " +
                        "BASISDESCRIPTION NVARCHAR(255) NOT NULL);";

                connectionString.Execute(tableCreationQuery);
            }
        }
    }
}
