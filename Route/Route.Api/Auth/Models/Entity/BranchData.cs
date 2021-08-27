using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;

namespace Route.Api.Auth.Models.Entity
{
    [Table(DbType = DbSupportType.MicrosoftSqlServer)]
    public class BranchData : IEntity
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Manual)]
        public virtual string Id { get; set; }

        /// <summary>
        /// Tên cty
        /// </summary>
        [BasicColumn()]
        public virtual string Name { get; set; }

        /// <summary>
        /// Base64 String logo
        /// </summary>
        [BasicColumn(CustomSqlType = "varchar(max)", Length = int.MaxValue)]
        public virtual string Logo { get; set; }

        [BasicColumn()]
        public virtual string SupportPhoneNumber { get; set; }

        [BasicColumn()]
        public virtual string ReportPhoneNumber { get; set; }

        [BasicColumn()]
        public virtual string WebSite { get; set; }

        [BasicColumn(CustomSqlType = "nvarchar(max)", Length = int.MaxValue)]
        public virtual string Reserve { get; set; }

        [BasicColumn()]
        public virtual string Header { get; set; }

        [BasicColumn()]
        public virtual string Footer { get; set; }

        [BasicColumn()]
        public virtual string Address { get; set; }

        [BasicColumn()]
        public virtual string LongName { get; set; }

        public virtual void FixNullObject()
        {
        }
    }

}