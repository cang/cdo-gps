using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;

namespace Route.Api.Auth.Models.Entity
{
    /// <summary>
    /// </summary>
    [Table(DbType = DbSupportType.MicrosoftSqlServer)]
    public class Functions : IEntity
    {

        [PrimaryKey(KeyGenerateType = KeyGenerateType.Manual)]
        public virtual string Name { get; set; }

        [BasicColumn]
        public virtual string Description { get; set; }

        public virtual void FixNullObject()
        {
        }
    }
}