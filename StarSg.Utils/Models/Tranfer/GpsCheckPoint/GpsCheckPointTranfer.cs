#region header

// /*********************************************************************************************/
// Project :Core
// FileName : DeviceTranfer.cs
// Time Create : 9:39 AM 24/03/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using StarSg.Utils.Models.Tranfer;

namespace Core.Models.Tranfer.GpsCheckPoint
{
    /// <summary>
    ///     điểm check
    /// </summary>
    public class GpsCheckPointTranfer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public GpsPoint Location { get; set; }
        public int Radius { get; set; }
        public long CompanyId { get; set; }
        public long GroupId { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
    }
}