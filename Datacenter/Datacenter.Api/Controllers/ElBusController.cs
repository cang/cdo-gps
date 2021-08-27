#region include

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web.Http;
using Datacenter.Api.Core;
using Datacenter.Model.Entity;
using StarSg.Core;
using StarSg.Utils.Models.DatacenterResponse.Device;
using StarSg.Utils.Models.Tranfer.ElBus;

#endregion

namespace Datacenter.Api.Controllers
{
    /// <summary>
    ///     quản lý thông tin xe bus dien tu
    /// </summary>
    [Auth]
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ElBusController : BaseController
    {

        [HttpPost]
        [Route("Route")]
        public BaseResponse AddRoute(ElBusRouteTransfer pobj)
        {
            if (pobj == null)
                return new BaseResponse { Description = "Thông tin gửi lên null" };

            try
            {
                var obj = new ElBusRoute();
                obj.company_id = pobj.company_id;
                obj.group_id = pobj.group_id;
                obj.name = pobj.name;
                obj.data = pobj.data;
                obj.km = pobj.km;
                obj.price = pobj.price;
                obj.created_at = DateTime.Now;
                obj.note = pobj.note;

                Log.Debug("DEVICE", $"bắt đầu thêm route {obj.name}");
                DataContext.Insert(obj, MotherSqlId);
                DataContext.Commit(MotherSqlId);

                return new ID32Response { Status = 1, id = obj.id, Description = "Thêm route thành công" };
            }
            catch (Exception ex)
            {
                Log.Exception("ElBusController", ex, "Thêm route vào database ko thành công");
                return new BaseResponse { Description = "Thêm route vào database ko thành công" };
            }
        }

        [HttpGet]
        [Route("Route")]
        public BaseResponse GetRoute(int id)
        {
            try
            {
                ElBusRoute pobj = DataContext.Get<ElBusRoute>(id, MotherSqlId);
                if (pobj == null) return new BaseResponse { Description = $"Không tìm thấy route {id}" };

                ElBusRouteTransfer obj = new ElBusRouteTransfer();
                obj.id = pobj.id;
                obj.company_id = pobj.company_id;
                obj.group_id = pobj.group_id;
                obj.name = pobj.name;
                obj.data = pobj.data;
                obj.km = pobj.km;
                obj.price = pobj.price;
                obj.created_at = DateTime.Now;
                obj.note = pobj.note;

                return new ElBusRouteBaseResponse {Status = 1, Data = obj };
            }
            catch (Exception ex)
            {
                Log.Exception("DeviceController", ex, $"Không tìm thấy route {id}");
                return new BaseResponse { Description = $"Không tìm thấy route {id}" };
            }
        }

        [HttpGet]
        [Route("Routes")]
        public BaseResponse GetRoutes(long company_id, long group_id)
        {
            List<ElBusRoute> rawret = new List<ElBusRoute>();
            try
            {
                if (group_id > 0 && company_id > 0)
                {
                    rawret = DataContext.GetWhere<ElBusRoute>(m =>
                                            m.company_id == company_id
                                            && m.group_id >= group_id, MotherSqlId).ToList();
                }
                else if (company_id > 0)
                {
                    rawret = DataContext.GetWhere<ElBusRoute>(m =>
                                            m.company_id == company_id, MotherSqlId).ToList();
                }
                else
                {
                    rawret = DataContext.GetAll<int, ElBusRoute>(m => m.id, MotherSqlId).Values.ToList();
                }

                ElBusRouteBaseResponses ret = new ElBusRouteBaseResponses { Status = 1 };
                foreach (var pobj in rawret)
                {
                    ElBusRouteTransfer obj = new ElBusRouteTransfer();
                    obj.id = pobj.id;
                    obj.company_id = pobj.company_id;
                    obj.group_id = pobj.group_id;
                    obj.name = pobj.name;
                    obj.data = pobj.data;
                    obj.km = pobj.km;
                    obj.price = pobj.price;
                    obj.created_at = DateTime.Now;
                    obj.note = pobj.note;
                    ret.Data.Add(obj);
                }
                return ret;
            }
            catch (Exception ex)
            {
                Log.Exception("DeviceController", ex, $"Không tìm thấy {company_id} group {group_id}");
            }
            return new ElBusRouteBaseResponses { Status = 1, Data = new List<ElBusRouteTransfer>(0) };
        }

        [HttpPut]
        [Route("Route")]
        public BaseResponse UpdateRoute(ElBusRouteTransfer pobj)
        {
            if (pobj == null)
                return new BaseResponse { Description = "Thông tin gửi lên null" };

            try
            {
                ElBusRoute obj = DataContext.Get<ElBusRoute>(pobj.id, MotherSqlId);
                if (obj == null) return new BaseResponse { Description = $"Không tìm thấy route {pobj.id}" };

                obj.company_id = pobj.company_id;
                obj.group_id = pobj.group_id;
                obj.name = pobj.name;
                obj.data = pobj.data;
                obj.km = pobj.km;
                obj.price = pobj.price;
                obj.note = pobj.note;

                Log.Debug("DEVICE", $"bắt đầu update route {obj.name}");
                DataContext.Update<ElBusRoute>(obj, MotherSqlId);
                DataContext.Commit(MotherSqlId);

                return new BaseResponse { Status = 1, Description = "Update route thành công" };
            }
            catch (Exception ex)
            {
                Log.Exception("ElBusController", ex, "Update route vào database ko thành công");
                return new BaseResponse { Description = "Update route vào database ko thành công" };
            }
        }

        [HttpDelete]
        [Route("Route")]
        public BaseResponse DeleteRoute(int id)
        {
            try
            {
                Log.Debug("DEVICE", $"bắt đầu delete route {id}");
                DataContext.Delete<ElBusRoute>(new ElBusRoute() {  id = id }, MotherSqlId);
                DataContext.Commit(MotherSqlId);
                return new BaseResponse { Status = 1, Description = "delete route thành công" };
            }
            catch (Exception ex)
            {
                Log.Exception("ElBusController", ex, "delete route vào database ko thành công");
                return new BaseResponse { Description = "delete route vào database ko thành công" };
            }
        }

        [HttpPost]
        [Route("Price")]
        public BaseResponse AddPrice(ElBusPriceTransfer pobj)
        {
            if (pobj == null)
                return new BaseResponse { Description = "Thông tin gửi lên null" };

            try
            {
                var obj = new ElBusPrice();

                obj.company_id = pobj.company_id;
                obj.group_id = pobj.group_id;
                obj.price_by_km = pobj.price_by_km;
                obj.price_by_time = pobj.price_by_time;

                Log.Debug("DEVICE", $"bắt đầu thêm price {pobj.company_id} {pobj.group_id}");
                DataContext.Insert(obj, MotherSqlId);
                DataContext.Commit(MotherSqlId);

                return new ID32Response { Status = 1, id = obj.id, Description = "Thêm price thành công" };
            }
            catch (Exception ex)
            {
                Log.Exception("ElBusController", ex, "Thêm price vào database ko thành công");
                return new BaseResponse { Description = "Thêm price vào database ko thành công" };
            }
        }

        [HttpGet]
        [Route("Price")]
        public BaseResponse GetPrice(long company_id, long group_id)
        {
            try
            {
                List<ElBusPrice> specialobjs = DataContext.GetWhere<ElBusPrice>(m =>
                        m.company_id == company_id
                        && m.group_id>= group_id, MotherSqlId).ToList();
                if(specialobjs.Count<1) return new BaseResponse { Description = $"Không tìm thấy price cho company {company_id} group {group_id}" };

                ElBusPrice pobj = specialobjs[0];

                ElBusPriceTransfer obj = new ElBusPriceTransfer();
                obj.id = pobj.id;
                obj.company_id = pobj.company_id;
                obj.group_id = pobj.group_id;
                obj.price_by_km = pobj.price_by_km;
                obj.price_by_time = pobj.price_by_time;
                return new ElBusPriceBaseResponse { Status = 1, Data = obj };
            }
            catch (Exception ex)
            {
                Log.Exception("DeviceController", ex, $"Không tìm thấy price cho company {company_id} group {group_id}" );
                return new BaseResponse { Description = $"Không tìm thấy price cho company {company_id} group {group_id}" };
            }   
        }

        [HttpGet]
        [Route("Prices")]
        public BaseResponse GetPrices(long company_id, long group_id)
        {
            List<ElBusPrice> rawret = new List<ElBusPrice>();
            try
            {
                if (group_id > 0 && company_id > 0)
                {
                    rawret = DataContext.GetWhere<ElBusPrice>(m =>
                                            m.company_id == company_id
                                            && m.group_id >= group_id, MotherSqlId).ToList();
                }
                else if (company_id > 0)
                {
                    rawret = DataContext.GetWhere<ElBusPrice>(m =>
                                            m.company_id == company_id, MotherSqlId).ToList();
                }
                else
                {
                    rawret = DataContext.GetAll<int, ElBusPrice>(m => m.id, MotherSqlId).Values.ToList();
                }

                ElBusPriceBaseResponses ret = new ElBusPriceBaseResponses { Status = 1 };
                foreach (var pobj in rawret)
                {
                    ElBusPriceTransfer obj = new ElBusPriceTransfer();
                    obj.id = pobj.id;
                    obj.company_id = pobj.company_id;
                    obj.group_id = pobj.group_id;
                    obj.price_by_km = pobj.price_by_km;
                    obj.price_by_time = pobj.price_by_time;
                    ret.Data.Add(obj);
                }
                return ret;
            }
            catch (Exception ex)
            {
                Log.Exception("DeviceController", ex, $"Không tìm thấy {company_id} group {group_id}");
            }
            return new ElBusPriceBaseResponses { Status = 1, Data = new List<ElBusPriceTransfer>(0)};
        }

        [HttpPut]
        [Route("Price")]
        public BaseResponse UpdatePrice(ElBusPriceTransfer pobj)
        {
            if (pobj == null)
                return new BaseResponse { Description = "Thông tin gửi lên null" };

            try
            {
                List<ElBusPrice> specialobjs = DataContext.GetWhere<ElBusPrice>(m =>
                                        m.company_id == pobj.company_id
                                        && m.group_id >= pobj.group_id, MotherSqlId).ToList();

                if (specialobjs.Count < 1) return new BaseResponse { Description = $"Không tìm thấy price cho company {pobj.company_id} group {pobj.group_id}" };

                ElBusPrice obj = specialobjs[0];
                obj.company_id = pobj.company_id;
                obj.group_id = pobj.group_id;
                obj.price_by_km = pobj.price_by_km;
                obj.price_by_time = pobj.price_by_time;

                Log.Debug("DEVICE", $"bắt đầu update price company {pobj.company_id} group {pobj.group_id}");
                DataContext.Update<ElBusPrice>(obj, MotherSqlId);
                DataContext.Commit(MotherSqlId);

                return new BaseResponse { Status = 1, Description = "Update price thành công" };
            }
            catch (Exception ex)
            {
                Log.Exception("ElBusController", ex, "Update price vào database ko thành công");
                return new BaseResponse { Description = "Update roupricete vào database ko thành công" };
            }
        }

        [HttpDelete]
        [Route("Price")]
        public BaseResponse DeletePrice(long company_id, long group_id)
        {
            try
            {
                List<ElBusPrice> specialobjs = DataContext.GetWhere<ElBusPrice>(m =>
                       m.company_id == company_id
                       && m.group_id >= group_id, MotherSqlId).ToList();
                if (specialobjs.Count < 1) return new BaseResponse { Description = $"Không tìm thấy price cho company {company_id} group {group_id}" };

                foreach(var obj in specialobjs)
                    DataContext.Delete<ElBusPrice>(obj, MotherSqlId);
                DataContext.Commit(MotherSqlId);

                return new BaseResponse { Status = 1, Description = "delete Price thành công" };
            }
            catch (Exception ex)
            {
                Log.Exception("ElBusController", ex, "delete Price vào database ko thành công");
                return new BaseResponse { Description = "delete Price vào database ko thành công" };
            }
        }


        [HttpPost]
        [Route("Schedule")]
        public BaseResponse AddSchedule(ElBusScheduleTransfer pobj)
        {
            if (pobj == null)
                return new BaseResponse { Description = "Thông tin gửi lên null" };

            try
            {
                var obj = new ElBusSchedule();

                obj.company_id = pobj.company_id;
                obj.group_id = pobj.group_id;
                obj.approved_by = pobj.approved_by;
                obj.drived_by = pobj.drived_by;
                obj.created_by = pobj.created_by;
                obj.route_id = pobj.route_id;
                obj.type = pobj.type;
                obj.km = pobj.km;
                obj.time = pobj.time;
                obj.time_start = pobj.time_start;
                obj.time_end = pobj.time_end;
                obj.price = pobj.price;
                obj.active = pobj.active;
                obj.created_at = DateTime.Now;
                obj.note = pobj.note;
    
                Log.Debug("DEVICE", $"bắt đầu thêm Schedule {pobj.company_id} {pobj.group_id}");
                DataContext.Insert(obj, MotherSqlId);
                DataContext.Commit(MotherSqlId);

                return new ID32Response { Status = 1, id = obj.id, Description = "Thêm price thành công" };
            }
            catch (Exception ex)
            {
                Log.Exception("ElBusController", ex, "Thêm price vào database ko thành công");
                return new ID32Response { Description = "Thêm price vào database ko thành công" };
            }
        }

        [HttpGet]
        [Route("Schedule")]
        public BaseResponse GetSchedule(int id)
        {
            try
            {
                ElBusSchedule pobj = DataContext.Get<ElBusSchedule>(id, MotherSqlId);
                if (pobj == null) return new BaseResponse { Description = $"Không tìm thấy {id}" };

                ElBusScheduleTransfer obj = new ElBusScheduleTransfer();
                obj.id = pobj.id;
                obj.company_id = pobj.company_id;
                obj.group_id = pobj.group_id;
                obj.approved_by = pobj.approved_by;
                obj.drived_by = pobj.drived_by;
                obj.created_by = pobj.created_by;
                obj.route_id = pobj.route_id;
                obj.type = pobj.type;
                obj.km = pobj.km;
                obj.time = pobj.time;
                obj.time_start = pobj.time_start;
                obj.time_end = pobj.time_end;
                obj.price = pobj.price;
                obj.active = pobj.active;
                obj.created_at = DateTime.Now;
                obj.note = pobj.note;

                return new ElBusScheduleBaseResponse { Status = 1, Data = obj };
            }
            catch (Exception ex)
            {
                Log.Exception("DeviceController", ex, $"Không tìm thấy route {id}");
                return new BaseResponse { Description = $"Không tìm thấy route {id}" };
            }
        }


        [HttpGet]
        [Route("Schedules")]
        public BaseResponse GetSchedules(long company_id, long group_id)
        {
            List<ElBusSchedule> rawret = new List<ElBusSchedule>();
            try
            {
                if (group_id > 0 && company_id > 0)
                {
                    rawret = DataContext.GetWhere<ElBusSchedule>(m =>
                                            m.company_id == company_id
                                            && m.group_id >= group_id, MotherSqlId).ToList();
                }
                else if (company_id > 0)
                {
                    rawret = DataContext.GetWhere<ElBusSchedule>(m =>
                                            m.company_id == company_id, MotherSqlId).ToList();
                }
                else
                {
                    rawret = DataContext.GetAll<int, ElBusSchedule>(m => m.id, MotherSqlId).Values.ToList();
                }

                ElBusScheduleBaseResponses ret = new ElBusScheduleBaseResponses { Status = 1 };
                foreach (var pobj in rawret)
                {
                    ElBusScheduleTransfer obj = new ElBusScheduleTransfer();
                    obj.id = pobj.id;
                    obj.company_id = pobj.company_id;
                    obj.group_id = pobj.group_id;
                    obj.approved_by = pobj.approved_by;
                    obj.drived_by = pobj.drived_by;
                    obj.created_by = pobj.created_by;
                    obj.route_id = pobj.route_id;
                    obj.type = pobj.type;
                    obj.km = pobj.km;
                    obj.time = pobj.time;
                    obj.time_start = pobj.time_start;
                    obj.time_end = pobj.time_end;
                    obj.price = pobj.price;
                    obj.active = pobj.active;
                    obj.created_at = DateTime.Now;
                    obj.note = pobj.note;
                    ret.Data.Add(obj);
                }
                return ret;
            }
            catch (Exception ex)
            {
                Log.Exception("DeviceController", ex, $"Không tìm thấy {company_id} group {group_id}");
            }
            return new ElBusScheduleBaseResponses { Status = 1, Data = new List<ElBusScheduleTransfer>(0) };
        }


        [HttpPut]
        [Route("Schedule")]
        public BaseResponse UpdateRoute(ElBusScheduleTransfer pobj)
        {
            if (pobj == null)
                return new BaseResponse { Description = "Thông tin gửi lên null" };

            try
            {
                ElBusSchedule obj = DataContext.Get<ElBusSchedule>(pobj.id, MotherSqlId);
                if (obj == null) return new BaseResponse { Description = $"Không tìm thấy Schedule {pobj.id}" };

                obj.company_id = pobj.company_id;
                obj.group_id = pobj.group_id;
                obj.approved_by = pobj.approved_by;
                obj.drived_by = pobj.drived_by;
                obj.created_by = pobj.created_by;
                obj.route_id = pobj.route_id;
                obj.type = pobj.type;
                obj.km = pobj.km;
                obj.time = pobj.time;
                obj.time_start = pobj.time_start;
                obj.time_end = pobj.time_end;
                obj.price = pobj.price;
                obj.active = pobj.active;
                obj.created_at = DateTime.Now;
                obj.note = pobj.note;

                Log.Debug("DEVICE", $"bắt đầu update Schedule {obj.id}");
                DataContext.Update<ElBusSchedule>(obj, MotherSqlId);
                DataContext.Commit(MotherSqlId);

                return new BaseResponse { Status = 1, Description = "Update route thành công" };
            }
            catch (Exception ex)
            {
                Log.Exception("ElBusController", ex, "Update Schedule vào database ko thành công");
                return new BaseResponse { Description = "Update Schedule vào database ko thành công" };
            }
        }


        [HttpDelete]
        [Route("Schedule")]
        public BaseResponse DeleteSchedule(int id)
        {
            try
            {
                Log.Debug("DEVICE", $"bắt đầu delete Schedule {id}");
                DataContext.Delete<ElBusSchedule>(new ElBusSchedule() { id = id }, MotherSqlId);
                DataContext.Commit(MotherSqlId);
                return new BaseResponse { Status = 1, Description = "delete Schedule thành công" };
            }
            catch (Exception ex)
            {
                Log.Exception("ElBusController", ex, "delete Schedule vào database ko thành công");
                return new BaseResponse { Description = "delete Schedule vào database ko thành công" };
            }
        }

    }
}