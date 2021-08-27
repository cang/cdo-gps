using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;

namespace Route.Api.Auth.Models.Entity
{
    [Table(DbType = DbSupportType.MicrosoftSqlServer)]
    public class Company : IEntity
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
        public virtual long Id { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual long CompanyId { get; set; }
        
        [ReferenceColumn(Name = "Account")]
        public virtual Account Account { get; set; }


        public virtual void FixNullObject()
        {
        }
    }
}