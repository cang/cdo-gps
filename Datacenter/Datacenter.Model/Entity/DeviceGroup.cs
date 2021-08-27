using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;

namespace Datacenter.Model.Entity
{
    /// <summary>
    /// chứa thông tin đội xe
    /// </summary>
    [Table]
    public class DeviceGroup:IEntity, ICacheModel
    {
        /// <summary>
        /// id
        /// </summary>
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
        public virtual long Id { get; set; }
        /// <summary>
        /// tên
        /// </summary>
        [BasicColumn]
        public virtual string Name { get; set; }
        /// <summary>
        /// id công ty
        /// </summary>
        [BasicColumn]
        public virtual long CompnayId { get; set; }

        public virtual void FixNullObject()
        {
            
        }
    }
}