using System.Collections;
using System.Collections.Generic;
using Microsoft.SqlServer.Server;
using StarSg.Core;

namespace StarSg.Utils.Models.DatacenterResponse.Qc31
{
    public class AllDriverReportGet:BaseResponse
    {
         public IList<Driver31Report> Datas { get; set; }
    }

    public class Driver31Report
    {
        public string Name { get; set; }
        public string Gplx { get; set; }
        public double Distance { get; set; }
        public int OverTime4hCount { get; set; }
        /// <summary>
        ///     số lần chạy quá tốc độ từ 5-10
        /// </summary>
        public int Speed5To10 { get; set; }

        /// <summary>
        ///     số lần chạy quá tốc độ từ 10-20
        /// </summary>
        public int Speed10To20 { get; set; }

        /// <summary>
        ///     số lần chạy quá tốc độ từ 20-35
        /// </summary>
        public int Speed20To35 { get; set; }

        /// <summary>
        ///     số lần chạy quá tốc độ từ >35
        /// </summary>
        public int Speed35 { get; set; }

        /// <summary>
        ///     phần trăm số lần chạy quá tốc độ từ 5-10
        /// </summary>
        public double Speed5To10Percent { get; set; }

        /// <summary>
        ///     phần trăm số lần chạy quá tốc độ từ 10-20
        /// </summary>
        public double Speed10To20Percent { get; set; }

        /// <summary>
        ///     phần trăm số lần chạy quá tốc độ từ 20-35
        /// </summary>
        public double Speed20To35Percent { get; set; }

        /// <summary>
        ///     phần trăm số lần chạy quá tốc độ từ >35
        /// </summary>
        public double Speed35Percent { get; set; }
    }
}