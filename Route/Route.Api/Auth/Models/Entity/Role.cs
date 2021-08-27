using System.Collections.Generic;
using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;

namespace Route.Api.Auth.Models.Entity
{
    /// <summary>
    /// nhóm user
    /// </summary>
    [Table(DbType = DbSupportType.MicrosoftSqlServer)]
    public class Role : IEntity
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Manual)]
        public virtual string Name { get; set; }
        [BasicColumn]
        public virtual string Description { get; set; }

        /// <summary>
        /// chứa thông tin các chức năng của nhóm user
        /// </summary>
        [HasManyColumn(Child = typeof (RoleFun), ForeignKeyName = "role_fun", Name = "Role",
            Type = HasManyType.List, Key = typeof (long), KeyName = "Id")]
        public virtual IList<RoleFun> Functions { get; set; } = new List<RoleFun>();

        [BasicColumn(IsIndex = true)]
        public virtual string Parent { get; set; }

        public virtual void FixNullObject()
        {
        }
    }
}