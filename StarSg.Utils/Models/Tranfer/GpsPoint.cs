namespace StarSg.Utils.Models.Tranfer
{
    public class GpsPoint
    {
        public virtual float Lat { get; set; }
        /// <summary>
        /// kinh độ
        /// </summary>
        public virtual float Lng { get; set; }
        /// <summary>
        /// địa chỉ
        /// </summary>
        public virtual string Address { get; set; }
    }
}