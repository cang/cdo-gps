#region header

// **********************************************************************
// SOLUTION: StarSg
// PROJECT: Route.Api
// TIME CREATE : 1:41 PM 18/12/2016
// FILENAME: Qc09Controller.cs
// AUTHOR: Cang Do (dovancang@gmail.com)
// -----------------------------------
// Copyrights 2016  - All Rights Reserved.
// **********************************************************************

#endregion

#region

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Core.Models.Tranfer;
using Datacenter.Api.Core;
using Route.Api.Core;
using Route.Api.Models;
using Route.Api.Models.Qc09;
using Route.UserRoute.Models.Qc09;
using StarSg.Core;
using StarSg.Utils.Models.Tranfer.DeviceManager;
using StarSg.Utils.Models.Tranfer.Qc09;

#endregion

namespace Route.Api.Controllers
{
    /// <summary>
    ///     báo cáo theo thông tư 09
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Auth]
    public class Qc09Controller : BaseController
    {
        [Import] private ISplitRequest _splitRequest;

        ///////////////////////////////////thông kê lái xe 10h ngày
        /// <summary>
        ///     thống kê vi phạm lái xe 10h theo công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [HttpGet]
        public DriverSession10HRequest Session10HByCompany([FromUri] long companyId, [FromUri] DateTime beginTime,
            [FromUri] DateTime endTime)
        {
            try
            {
                var center = CompanyRoute.GetDataCenter(companyId);
                if (center == null)
                {
                    return new DriverSession10HRequest() {Description = "Không tìm thấy máy chủ xử lý"};
                }
                var api = new ForwardApi();
                var driverSession10HList =
                    api.Get<List<DriverSession10HTranfer>>(
                        $"{center.Ip}:{center.Port}/api/Qc09/Session10HByCompany?companyId={companyId}&beginTime={beginTime}&endTime={endTime}");
                return
                    new DriverSession10HRequest
                    {
                        Status = 1,
                        DriverSession10HList =
                            driverSession10HList.Where(m => UserPermision.ContainDeviceSerial(m.DeviceSerial)).ToList()
                    };
            }
            catch (Exception exception)
            {
                Log.Exception("Qc09ReportController", exception, "Session10HByCompany");
            }
            return new DriverSession10HRequest {Status = 0};
        }


        /// <summary>
        ///     thống kê vi phạm lái xe 10h theo seriallist , nhóm hoặc cty
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="ids">Danh sách serial cách nhau | hoặc , hoặc ; hoặc kí tự trống, nếu = "" thì lấy hết theo groupId </param>
        /// <param name="serial">serial, nếu = 0 thì lấy hết theo ids</param>
        /// <returns></returns>
        [HttpGet]
        public DriverSession10HRequest Session10HEx(long companyId, DateTime begin, DateTime end, long groupId = 0, string ids = "", long serial = 0)
        {
            try
            {
                var center = CompanyRoute.GetDataCenter(companyId);
                if (center == null)
                {
                    return new DriverSession10HRequest() { Description = "Không tìm thấy máy chủ xử lý" };
                }
                var api = new ForwardApi();
                var driverSession10HList =
                    api.Get<List<DriverSession10HTranfer>>(
                        $"{center.Ip}:{center.Port}/api/Qc09/Session10HEx?companyId={companyId}&begin={begin}&end={end}&groupId={groupId}&ids={ids}&serial={serial}");
                return
                    new DriverSession10HRequest
                    {
                        Status = 1,
                        DriverSession10HList =
                            driverSession10HList.Where(m => UserPermision.ContainDeviceSerial(m.DeviceSerial)).ToList()
                    };
            }
            catch (Exception exception)
            {
                Log.Exception("Qc09ReportController", exception, "Session10HEx");
            }
            return new DriverSession10HRequest { Status = 0 };
        }

        /// <summary>
        ///     thông kê thống kê lái xe 10h theo serial list
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="serialList">các serial cách nhau = dấu '|' . VD 1|2|3</param>
        /// <returns></returns>
        [HttpGet]
        public DriverSession10HRequest Session10HBySerial([FromUri] long companyId, [FromUri] DateTime beginTime,
            [FromUri] DateTime endTime, string serialList)
        {
            try
            {
                var center = CompanyRoute.GetDataCenter(companyId);
                if (center == null)
                {
                    return new DriverSession10HRequest() {Description = "Không tìm thấy máy chủ xử lý"};
                }
                var driverSession10HList =
                    ForwardApi.Post<List<DriverSession10HTranfer>>(
                        $"{center.Ip}:{center.Port}/api/Qc09/Session10HBySerial?beginTime={beginTime}&endTime={endTime}&companyId={companyId}",
                        serialList.Split('|').Select(long.Parse));
                return new DriverSession10HRequest {Status = 1, DriverSession10HList = driverSession10HList};
            }
            catch (Exception exception)
            {
                Log.Exception("Qc09ReportController", exception, "hàm Session10HBySerial");
            }
            return new DriverSession10HRequest {Status = 0};
        }

        ///////////////////////////////////báo cáo tổng hợp theo thông tư 09/////////////////////////////////

        /// <summary>
        ///     báo cáo tổng hợp theo thông tư 09 theo công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(GeneralReport09Request))]
        public IHttpActionResult GeneralReport09Company([FromUri] long companyId, [FromUri] DateTime beginTime,
            [FromUri] DateTime endTime)
        {
            try
            {
                var center = CompanyRoute.GetDataCenter(companyId);
                if (center == null)
                {
                    return NotFound();
                }
                var generalReport09Company =
                    ForwardApi.Get<List<GeneralReport09Log>>(
                        $"{center.Ip}:{center.Port}/api/Qc09/GeneralReport09Company?companyId={companyId}&beginTime={beginTime}&endTime={endTime}");
                return
                    Ok(new GeneralReport09Request
                    {
                        Status = 1,
                        GeneralReport09List =
                            generalReport09Company.Where(m => UserPermision.ContainDeviceSerial(m.Serial)).ToList()
                    });
            }
            catch (Exception exception)
            {
                Log.Exception("Qc09ReportController", exception, "hàm GeneralReport09Company");
            }
            return Ok(new GeneralReport09Request {Status = 0});
        }

        /// <summary>
        ///     báo cáo tổng hợp thông tư 09 theo seriallist , nhóm hoặc cty
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="ids">Danh sách serial cách nhau | hoặc , hoặc ; hoặc kí tự trống, nếu = "" thì lấy hết theo groupId </param>
        /// <param name="serial">serial, nếu = 0 thì lấy hết theo ids</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(GeneralReport09Request))]
        public IHttpActionResult GeneralReport09Ex(long companyId, DateTime begin, DateTime end, long groupId = 0, string ids = "", long serial = 0)
        {
            try
            {
                var center = CompanyRoute.GetDataCenter(companyId);
                if (center == null)
                {
                    return NotFound();
                }

                var generalReport09Company =
                    ForwardApi.Get<List<GeneralReport09Log>>(
                        $"{center.Ip}:{center.Port}/api/Qc09/GeneralReport09Ex?companyId={companyId}&begin={begin}&end={end}&groupId={groupId}&ids={ids}&serial={serial}");
                return
                    Ok(new GeneralReport09Request
                    {
                        Status = 1,
                        GeneralReport09List =
                            generalReport09Company.Where(m => UserPermision.ContainDeviceSerial(m.Serial)).ToList()
                    });
            }
            catch (Exception exception)
            {
                Log.Exception("Qc09ReportController", exception, "GeneralReport09Ex");
            }
            return Ok(new GeneralReport09Request { Status = 0 });
        }


        /// <summary>
        ///     báo cáo tổng hợp thông tư 09 theo serial list
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="serialList">danh sách các serial cách nhau = dấu | . vd 1|2|3</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(GeneralReport09Request))]
        public GeneralReport09Request GeneralReport09Serial([FromUri] long companyId, [FromUri] DateTime beginTime,
            [FromUri] DateTime endTime, string serialList)
        {
            try
            {
                var center = CompanyRoute.GetDataCenter(companyId);
                if (center == null)
                {
                    return new GeneralReport09Request() {Description = "Không tìm thấy máy chủ xử lý"};
                }
                var result = _splitRequest.Split(serialList.Split('|').Select(long.Parse).ToList(), ss =>
                {
                    var tmpResult = new List<GeneralReport09Log>();
                    try
                    {
                        var generalReport09Logs =
                            ForwardApi.Post<List<GeneralReport09Log>>(
                                $"{center.Ip}:{center.Port}/api/Qc09/GeneralReport09Serial?companyId={companyId}&beginTime={beginTime}&endTime={endTime}",
                                ss);
                        if (generalReport09Logs != null)
                        {
                            tmpResult = generalReport09Logs;
                        }
                    }
                    catch (Exception exception)
                    {
                        Log.Exception("Qc09ReportController", exception, "hàm GeneralReport09Serial");
                    }
                    return tmpResult;
                });
                return new GeneralReport09Request {Status = 1, GeneralReport09List = result};
            }
            catch (Exception exception)
            {
                Log.Exception("Qc09ReportController", exception, "hàm GeneralReport09Serial");
            }
            return new GeneralReport09Request {Status = 0};
        }


        /// <summary>
        ///     thống kê quá tốc độ theo xe
        /// </summary>
        /// <param name="idCompany"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="serialList">danh sách các serial cách nhau = dấu | . vd 1|2|3</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(OverSpeedReport09Request))]
        public OverSpeedReport09Request OverSpeedReport09([FromUri] long idCompany, [FromUri] DateTime beginTime,
            [FromUri] DateTime endTime, string serialList)
        {
            try
            {
                var center = CompanyRoute.GetDataCenter(idCompany);
                if (center == null)
                {
                    return new OverSpeedReport09Request() {Description = "Không tìm thấy thông tin máy chủ"};
                }
                var result = _splitRequest.Split(serialList.Split('|').Select(long.Parse).ToList(), ss =>
                {
                    var tmpResult = new List<OverSpeedReport09Tranfer>();
                    try
                    {
                        var overSpeedReport09List =
                            ForwardApi.Post<List<OverSpeedReport09Tranfer>>(
                                $"{center.Ip}:{center.Port}/api/Qc09/OverSpeedReport09?companyId={idCompany}&beginTime={beginTime}&endTime={endTime}",
                                ss);
                        if (overSpeedReport09List != null)
                        {
                            tmpResult = overSpeedReport09List;
                        }
                    }
                    catch (Exception exception)
                    {
                        Log.Exception("Qc09ReportController", exception, "hàm OverSpeedReport09");
                    }
                    return tmpResult;
                });
                return new OverSpeedReport09Request {Status = 1, OverSpeedReport09List = result};
            }
            catch (Exception exception)
            {
                Log.Exception("Qc09ReportController", exception, "hàm OverSpeedReport09");
            }
            return new OverSpeedReport09Request {Status = 0};
        }

        /// <summary>
        ///     thống kế quá tốc độ xe theo thông tư 09
        /// </summary>
        /// <param name="idCompany"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(OverSpeedReport09Request))]
        public IHttpActionResult OverSpeedReport09Company([FromUri] long idCompany, [FromUri] DateTime beginTime,
            [FromUri] DateTime endTime)
        {
            try
            {
                var center = CompanyRoute.GetDataCenter(idCompany);
                if (center == null)
                {
                    return NotFound();
                }
                var overSpeedReport09List =
                    ForwardApi.Get<List<OverSpeedReport09Tranfer>>(
                        $"{center.Ip}:{center.Port}/api/Qc09/OverSpeedReport09Company?companyId={idCompany}&beginTime={beginTime}&endTime={endTime}");
                return
                    Ok(new OverSpeedReport09Request
                    {
                        Status = 1,
                        OverSpeedReport09List =
                            overSpeedReport09List.Where(m => UserPermision.ContainDeviceSerial(m.Serial)).ToList()
                    });
            }
            catch (Exception exception)
            {
                Log.Exception("Qc09ReportController", exception, "hàm OverSpeedReport09");
            }
            return Ok(new OverSpeedReport09Request {Status = 0});
        }

        /// <summary>
        ///     báo cáo quá tốc độ theo thông tư 09 (theo công ty)
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(OverSpeedLog09Request))]
        public OverSpeedLog09Request OverSpeed09Log([FromUri] long companyId, [FromUri] DateTime beginTime,
            [FromUri] DateTime endTime)
        {
            var result = new List<OverSpeedLog09Tranfer>();
            try
            {
                var center = CompanyRoute.GetDataCenter(companyId);
                if (center == null)
                {
                    return new OverSpeedLog09Request() {Description = "Không tìm thấy máy chủ xử lý"};
                }
                var overSpeed09List =
                    ForwardApi.Get<List<OverSpeedLog09Tranfer>>(
                        $"{center.Ip}:{center.Port}/api/Qc09/GetOverSpeedLog09?companyId={companyId}&beginTime={beginTime}&endTime={endTime}");
                if (overSpeed09List != null)
                {
                    result = overSpeed09List;
                }
            }
            catch (Exception exception)
            {
                Log.Exception("Qc09ReportController", exception, "hàm GetOverSpeed09Log");
            }
            return
                new OverSpeedLog09Request
                {
                    OverSpeed09LogList = result.Where(m => UserPermision.ContainDeviceSerial(m.Serial)).ToList()
                };
        }


        /// <summary>
        ///     lấy thông tin quá tốc độ theo seriallist , nhóm hoặc cty
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="ids">Danh sách serial cách nhau | hoặc , hoặc ; hoặc kí tự trống, nếu = "" thì lấy hết theo groupId </param>
        /// <param name="serial">serial, nếu = 0 thì lấy hết theo ids</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(OverSpeedLog09Request))]
        public OverSpeedLog09Request GetOverSpeedLog09Ex(long companyId, DateTime begin, DateTime end, long groupId = 0, string ids = "", long serial = 0)
        {
            var result = new List<OverSpeedLog09Tranfer>();
            try
            {
                var center = CompanyRoute.GetDataCenter(companyId);
                if (center == null)
                {
                    return new OverSpeedLog09Request() { Description = "Không tìm thấy máy chủ xử lý" };
                }
                var overSpeed09List =
                    ForwardApi.Get<List<OverSpeedLog09Tranfer>>(
                        $"{center.Ip}:{center.Port}/api/Qc09/GetOverSpeedLog09Ex?companyId={companyId}&begin={begin}&end={end}&groupId={groupId}&ids={ids}&serial={serial}");
                if (overSpeed09List != null)
                {
                    result = overSpeed09List;
                }
            }
            catch (Exception exception)
            {
                Log.Exception("Qc09ReportController", exception, "GetOverSpeedLog09Ex");
            }
            return
                new OverSpeedLog09Request
                {
                    OverSpeed09LogList = result.Where(m => UserPermision.ContainDeviceSerial(m.Serial)).ToList()
                };
        }

        /// <summary>
        ///     thống kê quá tốc độ theo thông tư 09 (theo serial)
        /// </summary>
        /// <param name="idCompany"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="serialList">danh sách các serial cách nhau = dấu | . vd 1|2|3</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(OverSpeedLog09Request))]
        public OverSpeedLog09Request OverSpeed09LogBySerial([FromUri] long idCompany, [FromUri] DateTime beginTime,
            [FromUri] DateTime endTime, string serialList)
        {
            try
            {
                var center = CompanyRoute.GetDataCenter(idCompany);
                if (center == null)
                {
                    return new OverSpeedLog09Request() {Description = "Không tìm thấy máy chủ xử lý"};
                }
                var result = _splitRequest.Split(serialList.Split('|').Select(long.Parse).ToList(), ss =>
                {
                    var tmpResult = new List<OverSpeedLog09Tranfer>();
                    try
                    {
                        var overSpeed09List =
                            ForwardApi.Post<List<OverSpeedLog09Tranfer>>(
                                $"{center.Ip}:{center.Port}/api/Qc09/GetOverSpeed09BySerial?companyId={idCompany}&beginTime={beginTime}&endTime={endTime}",
                                ss);
                        if (overSpeed09List != null)
                        {
                            tmpResult = overSpeed09List;
                        }
                    }
                    catch (Exception exception)
                    {
                        Log.Exception("Qc09ReportController", exception, "hàm OverSpeed09LogBySerial");
                    }
                    return tmpResult;
                });
                return new OverSpeedLog09Request {Status = 1, OverSpeed09LogList = result};
            }
            catch (Exception exception)
            {
                Log.Exception("Qc09ReportController", exception, "hàm OverSpeed09LogBySerial");
            }
            return new OverSpeedLog09Request {Status = 0};
        }


        ////////////////////////////////tính lái xe liên tục 4h //////////////////////////////////


        /// <summary>
        ///     báo cáo thời gian lái xe liên tục 4h theo công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(DriverSessionLogRequest))]
        public IHttpActionResult Session4HByCompany([FromUri] long companyId, [FromUri] DateTime beginTime,
            [FromUri] DateTime endTime)
        {
            var result = new List<DriverSessionLogTranfer>();
            try
            {
                var center = CompanyRoute.GetDataCenter(companyId);
                if (center == null)
                {
                    return NotFound();
                }
                var deviceTripLogs =
                    ForwardApi.Get<List<DriverSessionLogTranfer>>(
                        $"{center.Ip}:{center.Port}/api/Qc09/Session4HByCompany?companyId={companyId}&beginTime={beginTime}&endTime={endTime}");
                if (deviceTripLogs != null)
                {
                    result = deviceTripLogs;
                }
            }
            catch (Exception exception)
            {
                Log.Exception("Qc09ReportController", exception, "Session4HByCompany");
            }
            return
                Ok(new DriverSessionLogRequest
                {
                    DriverSessionLogList = result.Where(m => UserPermision.ContainDeviceSerial(m.DeviceSerial)).ToList()
                });
        }


        /// <summary>
        ///     báo cáo thời gian lái xe liên tục 4h theo seriallist , nhóm hoặc cty
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="ids">Danh sách serial cách nhau | hoặc , hoặc ; hoặc kí tự trống, nếu = "" thì lấy hết theo groupId </param>
        /// <param name="serial">serial, nếu = 0 thì lấy hết theo ids</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(DriverSessionLogRequest))]
        public IHttpActionResult Session4HEx(long companyId, DateTime begin, DateTime end, long groupId = 0, string ids = "", long serial = 0)
        {
            var result = new List<DriverSessionLogTranfer>();
            try
            {
                var center = CompanyRoute.GetDataCenter(companyId);
                if (center == null)
                {
                    return NotFound();
                }
                var deviceTripLogs =
                    ForwardApi.Get<List<DriverSessionLogTranfer>>(
                         $"{center.Ip}:{center.Port}/api/Qc09/Session4HEx?companyId={companyId}&begin={begin}&end={end}&groupId={groupId}&ids={ids}&serial={serial}");
                if (deviceTripLogs != null)
                {
                    result = deviceTripLogs;
                }
            }
            catch (Exception exception)
            {
                Log.Exception("Qc09ReportController", exception, "Session4HEx");
            }
            return
                Ok(new DriverSessionLogRequest
                {
                    DriverSessionLogList = result.Where(m => UserPermision.ContainDeviceSerial(m.DeviceSerial)).ToList()
                });
        }


        /// <summary>
        ///     báo cáo thời gian lái xe liên tục 4h theo serial
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="serialList">danh sách các serial cách nhau = dấu | . vd 1|2|3</param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(DriverSessionLogRequest))]
        public DriverSessionLogRequest Session4HBySerial([FromUri] long companyId, string serialList,
            [FromUri] DateTime beginTime,
            [FromUri] DateTime endTime)
        {
            var result = new List<DriverSessionLogTranfer>();
            try
            {
                var center = CompanyRoute.GetDataCenter(companyId);
                if (center == null)
                {
                    return new DriverSessionLogRequest() {Description="Không tìm thấy thông tin máy chủ" };
                }
                var deviceTripLogs =
                    ForwardApi.Post<List<DriverSessionLogTranfer>>(
                        $"{center.Ip}:{center.Port}/api/Qc09/Session4HBySerial?companyId={companyId}&beginTime={beginTime}&endTime={endTime}",
                        serialList.Split('|').Select(long.Parse));
                if (deviceTripLogs != null)
                {
                    result = deviceTripLogs;
                }
            }
            catch (Exception exception)
            {
                Log.Exception("Qc09ReportController", exception, "hàm Session4HBySerial");
            }
            return new DriverSessionLogRequest {DriverSessionLogList = result};
        }


        /////////////////////////////////báo cáo không truyền dự liệu lên bộ//////////////////////////////////////

        /// <summary>
        ///     lấy log không truyền data lên bộ theo serial
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="serial"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(DeviceTraceLogRequest))]
        public IHttpActionResult LostDataDetail09([FromUri] long companyId,
            [FromUri] DateTime beginTime,
            [FromUri] DateTime endTime, [FromUri] long serial)
        {
            var result = new List<DeviceTraceLogTranfer>();
            try
            {
                var center = CompanyRoute.GetDataCenter(companyId);
                if (center == null)
                {
                    return NotFound();
                }
                var deviceTraceLogs =
                    ForwardApi.Get<List<DeviceTraceLogTranfer>>(
                        $"{center.Ip}:{center.Port}/api/Qc09/LostDataDetail09?serial={serial}&beginTime={beginTime}&endTime={endTime}");
                if (deviceTraceLogs != null)
                {
                    result = deviceTraceLogs;
                }
            }
            catch (Exception exception)
            {
                Log.Exception("Qc09ReportController", exception, "hàm LostDataDetail09");
            }
            return Ok(new DeviceTraceLogRequest {DeviceTraceLogList = result});
        }

        /// <summary>
        ///     lấy thông kê vi phạm truyền data lên bộ
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="serialList">danh sách các serial cách nhau = dấu | . vd 1|2|3</param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [HttpGet]
        public LostDataReport09Request LostDataReport09([FromUri] long companyId, string serialList,
            [FromUri] DateTime beginTime,
            [FromUri] DateTime endTime)
        {
            var result = new List<LostDataReport09Tranfer>();
            try
            {
                var center = CompanyRoute.GetDataCenter(companyId);
                if (center == null)
                {
                    return new LostDataReport09Request() {Description = "Không tìm thấy thông tin máy chủ"};
                }
                var lostDataReport09Tranfers =
                    ForwardApi.Post<List<LostDataReport09Tranfer>>(
                        $"{center.Ip}:{center.Port}/api/Qc09/LostDataReport09?companyId={companyId}&beginTime={beginTime}&endTime={endTime}",
                        serialList.Split('|').Select(long.Parse));
                if (lostDataReport09Tranfers != null)
                {
                    result = lostDataReport09Tranfers;
                }
            }
            catch (Exception exception)
            {
                Log.Exception("Qc09ReportController", exception, "hàm LostDataReport09");
            }
            return new LostDataReport09Request {LostDataReport09List = result};
        }

        /// <summary>
        ///     thống kê mất truyền data lên bộ
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="ids">Danh sách serial cách nhau | hoặc , hoặc ; hoặc kí tự trống, nếu = "" thì lấy hết theo groupId </param>
        /// <param name="serial">serial, nếu = 0 thì lấy hết theo ids</param>
        /// <returns></returns>
        [HttpGet]
        public LostDataReport09Request LostDataReport09Ex(long companyId, DateTime begin, DateTime end, long groupId = 0, string ids = "", long serial = 0)
        {
            var result = new List<LostDataReport09Tranfer>();
            try
            {
                var center = CompanyRoute.GetDataCenter(companyId);
                if (center == null)
                    return new LostDataReport09Request() { Description = "Không tìm thấy thông tin máy chủ" };

                var lostDataReport09Tranfers =
                    ForwardApi.Get<List<LostDataReport09Tranfer>>(
                        $"{center.Ip}:{center.Port}/api/Qc09/LostDataReport09Ex?companyId={companyId}&begin={begin}&end={end}&groupId={groupId}&ids={ids}&serial={serial}");

                if (lostDataReport09Tranfers != null)
                {
                    result = lostDataReport09Tranfers;
                }
            }
            catch (Exception exception)
            {
                Log.Exception("Qc09ReportController", exception, "hàm LostDataReport09Ex");
            }
            return new LostDataReport09Request { LostDataReport09List = result };

        }


    }
}