using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;

namespace Route.Api.Auth.Models.Entity
{
    [Table(DbType = DbSupportType.MicrosoftSqlServer)]
    public class SystemPair : IEntity
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Manual)]
        public virtual string Id { get; set; }

        [BasicColumn()]
        public virtual string Val { get; set; }

        [BasicColumn()]
        public virtual string Note { get; set; }

        public virtual void FixNullObject()
        {
        }
    }
}