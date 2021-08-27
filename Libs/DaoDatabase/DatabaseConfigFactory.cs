using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using FluentNHibernate.Cfg.Db;

namespace DaoDatabase
{
    /// <summary>
    ///     chỉ sử dụng cho MSSQL và MySQL
    /// </summary>
    public static class DatabaseConfigFactory
    {
        public static IPersistenceConfigurer GetDataConfig(bool isMySql, string sAddress, int sPort, string databaseName,
            string username, string password, bool isShowScript, IEnumerable<string> schema)
        {
            return isMySql
                ? GetMySqlDataConfig(sAddress, sPort, databaseName, username, password, isShowScript)
                : GetMsDataConfig(sAddress, schema, databaseName, username, password, isShowScript);
        }

        public static IPersistenceConfigurer GetMsDataConfig(string sAddress, IEnumerable<string> schema,
            string databaseName, string username, string password, bool isShowScript)
        {
            var connectionString = $@"Server={sAddress};Database={databaseName};User Id={username};Password={password};";
            if (schema != null)
                using (var sqlconnect = new SqlConnection(connectionString))
                {
                    var sql = schema.Aggregate("",
                        (current, s) =>
                            current +
                            string.Format(
                                "if not exists(select 1 from information_schema.schemata where schema_name='{0}') BEGIN     EXEC ('CREATE SCHEMA {0} AUTHORIZATION dbo;') END; ",
                                s));

                    sqlconnect.Open();
                    using (var cmd = new SqlCommand(sql, sqlconnect))
                    {
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    sqlconnect.Dispose();
                }

            return isShowScript
                ? MsSqlConfiguration
                    .MsSql2008.ConnectionString(connectionString)
                    .AdoNetBatchSize(100)
                    .ShowSql()
                : MsSqlConfiguration
                    .MsSql2008
                    .ConnectionString(connectionString)
                    .AdoNetBatchSize(100);
            //.Cache(c => c.UseQueryCache());
        }

        public static IPersistenceConfigurer GetMySqlDataConfig(string sAddress, int sPort, string databaseName,
            string username, string password, bool isShowScript)
        {
            var connectionString =
                $"Server={sAddress};Port={sPort};Database={databaseName};Uid={username};Password={password};";

            return isShowScript
                ? MySQLConfiguration
                    .Standard
                    .ConnectionString(connectionString)
                    .AdoNetBatchSize(1)
                    .ShowSql()
                : MySQLConfiguration
                    .Standard
                    .ConnectionString(connectionString)
                    .AdoNetBatchSize(1);
            //.Cache(c => c.UseQueryCache());
        }
    }
}