#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : DeviceModelController.cs
// Time Create : 1:53 PM 20/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web.Http;
using Core.Models.Tranfer;
using Datacenter.Api.Core;
using Datacenter.Model.Entity;
using StarSg.Core;
using StarSg.Utils.Models.DatacenterResponse.DeviceModel;

#endregion

namespace Datacenter.Api.Controllers
{
    /// <summary>
    ///     quản lý thông tin các loại xe
    /// </summary>
    [Auth]
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DeviceModelController : BaseController
    {
        /// <summary>
        ///     thêm thông tin model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse Add(DeviceModelTranfer model)
        {
            if (model == null)
                return new BaseResponse {Description = "Thông tin model null"};
            if (string.IsNullOrEmpty(model.Name))
                return new BaseResponse {Description = "Tên Model xe không được để trống"};

            // kiểm tra trong cache xem có chưa
            if (Cache.GetQueryContext<DeviceModel>().GetByKey(model.Name) != null)
                return new BaseResponse {Description = "Tên model này đã tôn tại"};

            // insert vào database

            try
            {
                var devModel = new DeviceModel();
                devModel.Name = model.Name;
                devModel.KmDaoLop = model.KmDaoLop;
                devModel.KmThayLocDau = model.KmThayLocDau;
                devModel.KmThayLocGio = model.KmThayLocGio;
                devModel.KmThayLocNhot = model.KmThayLocNhot;
                devModel.KmThayNhot = model.KmThayNhot;
                devModel.KmThayVo = model.KmThayVo;
                devModel.Sheat = model.Sheat;
                devModel.Xilanh = model.Xilanh;
                DataContext.Insert(devModel, MotherSqlId);
                DataContext.Commit(MotherSqlId);

                if (!Cache.GetQueryContext<DeviceModel>().Add(devModel, 0))
                    return new BaseResponse
                    {
                        Description = $"Thêm thông tin model {model.Name} vào cache không thành công"
                    };
                return new BaseResponse {Status = 1, Description = "Thêm thông tin model thành công"};
            }
            catch (Exception ex)
            {
                Log.Exception("DeviceModelController", ex,
                    $"Thêm thông tin model {model.Name} vào database ko thành công");
                return new BaseResponse {Description = $"Thêm thông tin model {model.Name} vào database ko thành công"};
            }
        }

        /// <summary>
        ///     cập nhật model xe
        /// </summary>
        /// <param name="name"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse Update(string name, DeviceModelTranfer model)
        {
            if (model == null)
                return new BaseResponse {Description = "Thông tin model null"};
            if (string.IsNullOrEmpty(name))
                return new BaseResponse {Description = "tên model null"};
            var deviceModel = Cache.GetQueryContext<DeviceModel>().GetByKey(name);
            if (deviceModel == null)
                return new BaseResponse {Description = $"Không tìm thấy thông tin model {name}"};

            deviceModel.KmDaoLop = model.KmDaoLop;
            deviceModel.KmThayLocDau = model.KmThayLocDau;
            deviceModel.KmThayLocGio = model.KmThayLocGio;
            deviceModel.KmThayLocNhot = model.KmThayLocNhot;
            deviceModel.KmThayNhot = model.KmThayNhot;
            deviceModel.KmThayVo = model.KmThayVo;
            deviceModel.Sheat = model.Sheat;
            deviceModel.Xilanh = model.Xilanh;

            try
            {
                DataContext.Update(deviceModel, MotherSqlId);
                DataContext.Commit(MotherSqlId);
                return new BaseResponse {Status = 1, Description = "Cập nhật model xe thành công"};
            }
            catch (Exception ex)
            {
                Log.Exception("DeviceModelController", ex, $"Cập nhật thông tin model {name}");
                return new BaseResponse {Description = $"Cập nhật thông tin model {name} vào database ko thành công"};
            }
        }

        /// <summary>
        ///     xóa thông tin model
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpDelete]
        public BaseResponse Del(string name)
        {
            if (string.IsNullOrEmpty(name))
                return new BaseResponse {Description = "tên model null"};
            var deviceModel = Cache.GetQueryContext<DeviceModel>().GetByKey(name);
            if (deviceModel == null)
                return new BaseResponse {Description = $"Không tìm thấy thông tin model {name}"};

            try
            {
                DataContext.Delete(deviceModel, MotherSqlId);
                DataContext.Commit(MotherSqlId);
                if (!Cache.GetQueryContext<DeviceModel>().Del(deviceModel, 0))
                    return new BaseResponse {Description = $"Xóa thông tin model trên cache ko thành công"};
                return new BaseResponse {Status = 1, Description = "Xóa thông tin model thành công"};
            }
            catch (Exception ex)
            {
                Log.Exception("DeviceModelController", ex, $"Xóa thông tin model {name}");
                return new BaseResponse {Description = $"Xóa thông tin model {name} vào database ko thành công"};
            }
        }

        /// <summary>
        ///     lấy thông tin model theo tên
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public DeviceModelGetSingle GetByName(string name)
        {
            var model = Cache.GetQueryContext<DeviceModel>().GetByKey(name);
            if (model == null)
                return new DeviceModelGetSingle {Description = $"Không tìm thấy thông tin model {name}"};
            return new DeviceModelGetSingle
            {
                Status = 1,
                Description = "OK",
                Model = new DeviceModelTranfer
                {
                    KmDaoLop = model.KmDaoLop,
                    KmThayVo = model.KmThayVo,
                    KmThayNhot = model.KmThayNhot,
                    KmThayLocDau = model.KmThayLocDau,
                    KmThayLocNhot = model.KmThayLocNhot,
                    KmThayLocGio = model.KmThayLocGio,
                    Name = name,
                    Sheat = model.Sheat,
                    Xilanh = model.Xilanh
                }
            };
        }

        /// <summary>
        ///     Lấy toàn bộ danh sách model
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public DeviceModelGetMulti GetAll()
        {
            return new DeviceModelGetMulti
            {
                Status = 1,
                Description = "OK",
                Models = Cache.GetQueryContext<DeviceModel>().GetAll().Select(model => new DeviceModelTranfer
                {
                    KmDaoLop = model.KmDaoLop,
                    KmThayVo = model.KmThayVo,
                    KmThayNhot = model.KmThayNhot,
                    KmThayLocDau = model.KmThayLocDau,
                    KmThayLocNhot = model.KmThayLocNhot,
                    KmThayLocGio = model.KmThayLocGio,
                    Name = model.Name,
                    Sheat = model.Sheat,
                    Xilanh = model.Xilanh
                }).ToList()
            };
        }
    }
}