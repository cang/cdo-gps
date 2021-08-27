using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;

namespace Route.Api.Auth.Models.Entity
{
    /// <summary>
    /// 
    /// </summary>
    [Table(DbType = DbSupportType.MicrosoftSqlServer)]
    public class Device : IEntity
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
        public virtual long Id { get; set; }
        [BasicColumn(IsIndex = true)]
        public virtual long Serial { get; set; }

        [ReferenceColumn(Name = "Account")]
        public virtual Account Account { get; set; }

        public virtual void FixNullObject()
        {
        }
    }
}