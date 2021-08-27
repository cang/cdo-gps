using System;
using System.IO;
using DaoDatabase.AutoMapping.MapAtribute;

namespace Datacenter.Model.Log
{
    [Serializable()]
    public class IndexLogDevice : ISerializerModal
    {
        [BasicColumn(IsIndex = true)]
        public virtual long Serial { get; set; }
        [BasicColumn(IsIndex = true)]
        public virtual Guid Indentity { get; set; }
        [BasicColumn(IsIndex = true)]
        public virtual long GroupId { set; get; }
        //[BasicColumn(IsIndex = true)]
        //public virtual long DriverId { set; get; }
        [BasicColumn(IsIndex = true)]
        public virtual long CompanyId { set; get; }

        public virtual void Deserializer(BinaryReader stream, int version)
        {
        }

        public virtual void Serializer(BinaryWriter stream)
        {
        }
    }
}