#region header
// /*********************************************************************************************/
// Project :Datacenter.Model
// FileName : DeviceSetupRequest.cs
// Time Create : 3:34 PM 22/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System;
using DaoDatabase;
using DaoDatabase.AutoMapping.MapAtribute;
using Datacenter.Model.Utils;

namespace Datacenter.Model.Setup
{
    [Table]
    public class DeviceSetupRequest:IEntity
    {
        [PrimaryKey]
        public virtual long Id { get; set; }
        [BasicColumn(IsIndex = true)]
        public virtual long Serial { get; set; }
        [BasicColumn(IsIndex = true)]
        public virtual DateTime Request { set; get; }
        [BasicColumn]
        public virtual DateTime Response { set; get; }
        [BasicColumn(Length = 2048)]
        public virtual byte[] Data { set; get; }
        [BasicColumn(IsIndex = true)]
        public virtual string UserName { set; get; }
        [BasicColumn(IsIndex = true)]
        public virtual bool Complete { set; get; }
        [BasicColumn]
        public virtual string Note { set; get; }
        #region Implementation of IEntity

        /// <summary>
        /// sửa các giá trị ko được phép null trước khi insert vào cơ sở dữ liệu
        /// </summary>
        public virtual void FixNullObject()
        {
            Request = Request.Fix();
            Response = Request.Fix();
        }

        #endregion
    }
}