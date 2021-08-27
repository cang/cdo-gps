using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datacenter.Model
{
    public interface ISerializerModal
    {
        void Deserializer(BinaryReader stream,int version);
        void Serializer(BinaryWriter stream);
    }

}
