using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web.Mvc;
using Route.Api.Core;
using Route.Api.Models.Response;
using Route.DeviceServer;
using StarSg.Core;
using StarSg.Utils.Models.DatacenterResponse.System;

namespace Route.Api.Controllers
{
    /// <summary>
    ///     quản lý các thông tin trạng thái của hệ thống
    /// </summary>
    [Auth, Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class SystemController : BaseController
    {
        [Import]
        private Server DeviceServer { get; set; }

        /// <summary>
        ///     lấy thông tin log của route
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        [HttpGet]
        public SysLogResponse GetRouteLog(string tag, long index)
        {
            try
            {
                var result = MefLoader.RealLog.GetLog(index);
                return new SysLogResponse
                {
                    Status = 1,
                    Description = "OK",
                    Datas = result.Item2,
                    Index = result.Item1
                };
            }
            catch (Exception ex)
            {
                return new SysLogResponse
                {
                    Status = 1,
                    Description = "OK",
                    Datas =new List<String>() { $"[EXCEPTION] {ex.Message}" , $"[EXCEPTION] {ex.StackTrace}" },
                    Index = index
                };
            }
          
        }

        /// <summary>
        ///     Lấy thông tin log của datacenter
        /// </summary>
        /// <param name="datacenterId"></param>
        /// <param name="tag"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        [HttpGet]
        public SysLogResponse GetDatacenterLog(Guid datacenterId, string tag, long index)
        {
            var datacenter = DataCenterStore.Get(datacenterId);
            if (datacenter == null) return new SysLogResponse {Description = "Tồn tại datacenter"};
            var api = new ForwardApi();
            return
                api.Get<SysLogResponse>(
                    $"{datacenter.Ip}:{datacenter.Port}/api/System/GetDatacenterLog?tag={tag}&index={index}");
        }

        /// <summary>
        ///     lấy thông tin các datacenter
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public DatacenterGet GetDatacenter()
        {
            var datacenters = DataCenterStore.GetAll();
            return new DatacenterGet
            {
                Status = 1,
                Description = "OK",
                Data = datacenters.Select(m =>
                {
                    var list = new List<int>();
                    for (var i = 0; i < m.ReportCount; i++)
                    {
                        list.Add(i);
                    }
                    return new DatacenterRespose
                    {
                        Id = m.Id,
                        Name = m.NodeName,
                        ReportServerId = list
                    };
                }).ToList()
            };
        }

        /// <summary>
        ///     lấy thông tin các connection thiết bị hiện tại đang kết nối lên hệ thống
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ConcurentDeviceConnectGet GetConcurentDevice()
        {
            return new ConcurentDeviceConnectGet
            {
                Status = 1,
                Description = "OK",
                Data = new ConcurentDeviceConnect
                {
                    Concurent = DeviceServer.Concurent,
                    MaxConnect = DeviceServer.MaxConnect
                }
            };
        }
    }
}