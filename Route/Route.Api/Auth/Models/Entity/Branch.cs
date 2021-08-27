using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;

namespace Route.Api.Auth.Models.Entity
{
    [Table(DbType = DbSupportType.MicrosoftSqlServer)]
    public class Branch : IEntity
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Manual)]
        public virtual long CompanyId { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual string BranchCode { get; set; }

        public virtual void FixNullObject()
        {
        }
    }
}