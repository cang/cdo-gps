#region header

// /*********************************************************************************************/
// Project :Datacenter.Model
// FileName : DeviceRawLog.cs
// Time Create : 10:09 AM 22/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;
using Datacenter.Model.Utils;

namespace Datacenter.Model.Log
{
    [Table]
    public class DeviceRawLog : IDbLog
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
        public virtual long Id { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual long Serial { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual Guid Indentity { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual DateTime ClientSend { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual DateTime ServerRecv { get; set; }

        [BasicColumn(Length = 512)]
        public virtual byte[] Data { get; set; }

        [BasicColumn]
        public virtual string Note { get; set; }

        #region Implementation of IEntity

        /// <summary>
        ///     sửa các giá trị ko được phép null trước khi insert vào cơ sở dữ liệu
        /// </summary>
        public virtual void FixNullObject()
        {
            ClientSend = ClientSend.Fix();
            ServerRecv = ServerRecv.Fix();
        }

        #endregion

        #region Implementation of IDbLog

        /// <summary>
        ///     Id của máy chủ lưu trữ dữ liệu log
        /// </summary>
        [BasicColumn]
        public virtual int DbId { get; set; }

        #endregion
    }
}