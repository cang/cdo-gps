namespace DaoDatabase
{
    public class DatabaseConfig
    {
        public DbSupportType Direct { get; set; }
        
        protected DatabaseConfig()
        {
            Direct=DbSupportType.MicrosoftSqlServer;
        }

        protected DatabaseConfig(DbSupportType sp)
        {
            Direct = sp;
        }
    }

    public enum DbSupportType
    {
        MicrosoftSqlServer,
        Oracle,
        MicrosoftAccess,
        Firebird,
        PostgreSql,
        Db2Udb,
        MySql,
        SqLite,
        MongoDb
    }
}