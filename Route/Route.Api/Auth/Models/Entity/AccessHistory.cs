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
    public class AccessHistory : IEntity
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
        public virtual long Id { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual string   Username { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual DateTime AtTime { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual long     CompanyId { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual long     GroupId { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual long     Serial { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual string   Method { get; set; }

        [BasicColumn]
        public virtual string   Content { get; set; }

        [BasicColumn]
        public virtual string   Note { get; set; }

        public virtual void     FixNullObject()
        {
            AtTime = DateTime.Now;
        }
    }

    public enum AccessHistoryMethod : byte
    {
        Get,
        List,
        Add,
        Edit,
        Delete,
        Setup
    }


    /// <summary>
    /// SetupDevice
    /// </summary>
    [Table(DbType = DbSupportType.MicrosoftSqlServer)]
    public class SetupDeviceHistory : IEntity
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
        public virtual int Id { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual string Username { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual DateTime AtTime { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual long CompanyId { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual long GroupId { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual long Serial { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual short opcode { get; set; }

        [BasicColumn]
        public virtual string Note { get; set; }

        [BasicColumn]
        public virtual int Retry { get; set; }

        public virtual void FixNullObject()
        {
            AtTime = DateTime.Now;
        }
    }

}