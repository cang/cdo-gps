using System;
using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;

namespace Route.Api.Auth.Models.Entity
{
    /// <summary>
    ///     lưu log của user
    /// </summary>
    [Table(DbType = DbSupportType.MicrosoftSqlServer)]
    public class UserLog : IEntity
    {
        /// <summary>
        ///     key của log
        /// </summary>
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
        public virtual long Id { get; set; }

        /// <summary>
        ///     user name
        /// </summary>
        [BasicColumn(IsIndex = true)]
        public virtual string Username { get; set; }

        /// <summary>
        ///     nội dung log
        /// </summary>
        [BasicColumn]
        public virtual string Description { get; set; }

        /// <summary>
        ///     loại log
        ///     loại log được lấy từ enum AuthLogType
        /// </summary>
        [BasicColumn(IsIndex = true)]
        public virtual int TypeLog { get; set; }

        /// <summary>
        ///     ngày thêm log
        /// </summary>
        [BasicColumn(IsIndex = true)]
        public virtual DateTime CreateDate { get; set; }

        /// <summary>
        ///     thời gian xử lý
        /// </summary>
        [BasicColumn]
        public virtual int TimeHandle { get; set; }

        public virtual void FixNullObject()
        {
           // CreateDate = CreateDate.Fix();
        }
    }
}