#region header
// /*********************************************************************************************/
// Project :Authentication
// FileName : RoleFun.cs
// Time Create : 11:08 AM 22/04/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;

namespace Route.Api.Auth.Models.Entity
{
    [Table]
    public class RoleFun:IEntity
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
        public virtual  long Id { get; set; }
        [ReferenceColumn(Name = "Fun")]
        public virtual Functions Fun { get; set; }

        [ReferenceColumn(Name = "Role")]
        public virtual Role Role { get; set; }

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