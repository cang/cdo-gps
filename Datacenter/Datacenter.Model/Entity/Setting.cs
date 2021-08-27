#region header
// /*********************************************************************************************/
// Project :Datacenter.Model
// FileName : DeviceModel.cs
// Time Create : 1:54 PM 20/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;

namespace Datacenter.Model.Entity
{
    [Table]
    public class Setting : IEntity,ICacheModel
    {
        /// <summary>
        ///     Key
        /// </summary>
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Manual)]
        public virtual string Id { get; set; }

        /// <summary>
        ///  Value
        /// </summary>
        [BasicColumn]
        public virtual string Val { get; set; }
      
        #region Implementation of IEntity

        /// <summary>
        /// sửa các giá trị ko được phép null trước khi insert vào cơ sở dữ liệu
        /// </summary>
        public virtual void FixNullObject()
        {
            
        }

        #endregion
    }
}