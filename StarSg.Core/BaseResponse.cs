using System.Runtime.Serialization;

namespace StarSg.Core
{
    public class BaseResponse
    {
        /// <summary>
        /// trạng thái
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// thông tin trả về
        /// </summary>
        public string Description { get; set; }

        ///// <summary>
        ///// thông tin trả về dạng nén
        ///// </summary>
        //public string ZipData { get; set; }
    }

}