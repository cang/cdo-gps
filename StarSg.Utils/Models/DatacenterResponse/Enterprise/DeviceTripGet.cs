using System.Collections;
using System.Collections.Generic;
using StarSg.Core;
using StarSg.Utils.Models.Tranfer.DeviceManager;

namespace StarSg.Utils.Models.DatacenterResponse.Enterprise
{
    public class DeviceTripGet:BaseResponse
    {
         public IList<DeviceTripTranfer> Datas { get; set; }

        /// <summary>
        /// thông tin trả về dạng nén
        /// </summary>
        public string ZipData { get; set; }
    }

    public class DeviceRawGet : BaseResponse
    {
        public IList<DeviceRawTranfer> Datas { get; set; }
    }

    public class SyncDuplicateResponse : BaseResponse
    {
        public List<SerialCounter> Data { get; set; }
    }

    public class SerialCounter
    {
        public long Serial { get; set; }
        public int Counter { get; set; }
    }

    public class ReplaceSerialGet : BaseResponse
    {
        public List<long> Datas { get; set; }
    }

}