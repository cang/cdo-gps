#region header
// /*********************************************************************************************/
// Project :Authentication
// FileName : Account.cs
// Time Create : 2:13 PM 14/04/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;

namespace Route.Api.Auth.Models.Entity
{
    /// <summary>
    /// thông tin tài khoản
    /// </summary>
    [Table(DbType = DbSupportType.MicrosoftSqlServer)]
    public class Account : IEntity
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Manual)]
        public virtual string Username { get; set; }
        [BasicColumn]
        public virtual string Pwd { get; set; }
        [BasicColumn(IsIndex = true)]
        public virtual DateTime CreateDate { get; set; }
        [BasicColumn]
        public virtual AccountLevel Level { get; set; }
        [BasicColumn]
        public virtual bool IsBlock { get; set; }
        [BasicColumn(IsIndex = true)]
        public virtual string GroupUserId { get; set; }

        [BasicColumn]
        public virtual string DisplayName { get; set; }

        [BasicColumn]
        public virtual string Phone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [HasManyColumn(Child = typeof(Company), ForeignKeyName = "user_company", Name = "Account",
             Type = HasManyType.List, Key = typeof(long), KeyName = "Id")]
        public virtual IList<Company> CompanyIds { get; set; }=new List<Company>();
        
        /// <summary>
        /// 
        /// </summary>

        [HasManyColumn(Child = typeof(Device), ForeignKeyName = "user_device", Name = "Account",
             Type = HasManyType.List, Key = typeof(long), KeyName = "Id")]
        public virtual IList<Device> DeviceIds { get; set; }
        public virtual Role Role { get; set; }
        public virtual void FixNullObject()
        {
            CreateDate = DateTime.Now;
        }
    }
}