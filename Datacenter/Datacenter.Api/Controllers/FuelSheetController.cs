#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : FuelSheetController.cs
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
    ///     quản lý thông tin Bình chứa nhiên liệu
    /// </summary>
    [Auth]
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class FuelSheetController : BaseController
    {
        /// <summary>
        ///     thêm thông tin Bình chứa nhiên liệu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse Add(FuelSheetTranfer model)
        {
            if (model == null)
                return new BaseResponse {Description = "Thông tin Bình chứa nhiên liệu null" };
            if (string.IsNullOrEmpty(model.Name))
                return new BaseResponse {Description = "Tên Bình chứa nhiên liệu không được để trống" };

            // kiểm tra trong cache xem có chưa
            if (Cache.GetQueryContext<FuelSheet>().GetByKey(model.Name) != null)
                return new BaseResponse {Description = "Tên Bình chứa nhiên liệu này đã tôn tại" };

            // insert vào database

            try
            {
                var devModel = new FuelSheet();

                devModel.Name = model.Name;
                devModel.Note = model.Note;
                devModel.BarrelType = model.BarrelType;
                devModel.Params = model.Params;

                devModel.Length = model.Length;
                devModel.Height = model.Height;
                devModel.MinValue = model.MinValue;
                devModel.MinHz = model.MinHz;
                devModel.MaxHz = model.MaxHz;

                devModel.LostThreshold = model.LostThreshold;
                devModel.AddThreshold = model.AddThreshold;

                devModel.TimeCreate = DateTime.Now;

                DataContext.Insert(devModel, MotherSqlId);
                DataContext.Commit(MotherSqlId);

                if (!Cache.GetQueryContext<FuelSheet>().Add(devModel, 0))
                    return new BaseResponse
                    {
                        Description = $"Thêm thông tin Bình chứa nhiên liệu {model.Name} vào cache không thành công"
                    };
                return new BaseResponse {Status = 1, Description = "Thêm thông tin Bình chứa nhiên liệu thành công" };
            }
            catch (Exception ex)
            {
                Log.Exception("FuelSheetController", ex,
                    $"Thêm thông tin Bình chứa nhiên liệu {model.Name} vào database ko thành công");
                return new BaseResponse {Description = $"Thêm thông tin Bình chứa nhiên liệu {model.Name} vào database ko thành công"};
            }
        }

        /// <summary>
        ///     cập nhật Bình chứa nhiên liệu
        /// </summary>
        /// <param name="name"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse Update(string name, FuelSheetTranfer model)
        {
            if (model == null)
                return new BaseResponse {Description = "Thông tin Bình chứa nhiên liệu null" };
            if (string.IsNullOrEmpty(name))
                return new BaseResponse {Description = "tên Bình chứa nhiên liệu null" };
            var FuelSheet = Cache.GetQueryContext<FuelSheet>().GetByKey(name);
            if (FuelSheet == null)
                return new BaseResponse {Description = $"Không tìm thấy thông tin Bình chứa nhiên liệu {name}"};


            FuelSheet.Note = model.Note;
            FuelSheet.BarrelType = model.BarrelType;
            FuelSheet.Params = model.Params;

            FuelSheet.Length = model.Length;
            FuelSheet.Height = model.Height;
            FuelSheet.MinValue = model.MinValue;
            FuelSheet.MinHz = model.MinHz;
            FuelSheet.MaxHz = model.MaxHz;

            FuelSheet.LostThreshold = model.LostThreshold;
            FuelSheet.AddThreshold = model.AddThreshold;

            try
            {
                //DataContext.Update(FuelSheet, MotherSqlId);
                if (model.TimeCreate == DateTime.MinValue)
                {
                    DataContext.Update(FuelSheet, MotherSqlId
                    , m => m.Note
                    , m => m.BarrelType
                    , m => m.Params
                    , m => m.Length
                    , m => m.Height
                    , m => m.MinValue
                    , m => m.MinHz
                    , m => m.MaxHz
                    , m => m.LostThreshold
                    , m => m.AddThreshold
                    );
                }
                else
                {
                    FuelSheet.TimeCreate = model.TimeCreate;
                    DataContext.Update(FuelSheet, MotherSqlId
                                      , m => m.Note
                                      , m => m.BarrelType
                                      , m => m.Params
                                      , m => m.Length
                                      , m => m.Height
                                      , m => m.MinValue
                                      , m => m.MinHz
                                      , m => m.MaxHz
                                      , m => m.LostThreshold
                                      , m => m.AddThreshold
                                      , m => m.TimeCreate);
                }
            

                DataContext.Commit(MotherSqlId);

                return new BaseResponse {Status = 1, Description = "Cập nhật Bình chứa nhiên liệu xe thành công" };
            }
            catch (Exception ex)
            {
                Log.Exception("FuelSheetController", ex, $"Cập nhật thông tin Bình chứa nhiên liệu {name}");
                return new BaseResponse {Description = $"Cập nhật thông tin Bình chứa nhiên liệu {name} vào database ko thành công"};
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
                return new BaseResponse {Description = "tên Bình chứa nhiên liệu null" };
            var FuelSheet = Cache.GetQueryContext<FuelSheet>().GetByKey(name);
            if (FuelSheet == null)
                return new BaseResponse {Description = $"Không tìm thấy thông tin Bình chứa nhiên liệu {name}"};

            try
            {
                DataContext.Delete(FuelSheet, MotherSqlId);
                DataContext.Commit(MotherSqlId);
                if (!Cache.GetQueryContext<FuelSheet>().Del(FuelSheet, 0))
                    return new BaseResponse {Description = $"Xóa thông tin Bình chứa nhiên liệu trên cache ko thành công" };
                return new BaseResponse {Status = 1, Description = "Xóa thông tin Bình chứa nhiên liệu thành công" };
            }
            catch (Exception ex)
            {
                Log.Exception("FuelSheetController", ex, $"Xóa thông tin Bình chứa nhiên liệu {name}");
                return new BaseResponse {Description = $"Xóa thông tin Bình chứa nhiên liệu {name} vào database ko thành công"};
            }
        }

        /// <summary>
        ///     lấy thông tin Bình chứa nhiên liệu theo tên
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public FuelSheetGetSingle GetByName(string name)
        {
            var model = Cache.GetQueryContext<FuelSheet>().GetByKey(name);
            if (model == null)
                return new FuelSheetGetSingle {Description = $"Không tìm thấy thông tin Bình chứa nhiên liệu {name}"};
            return new FuelSheetGetSingle
            {
                Status = 1,
                Description = "OK",
                FuelSheet = new FuelSheetTranfer
                {
                    Name = model.Name,
                    Note = model.Note,
                    BarrelType = model.BarrelType,
                    Params = model.Params,

                    Length = model.Length,
                    Height = model.Height,
                    MinValue = model.MinValue,
                    MinHz = model.MinHz,
                    MaxHz = model.MaxHz,

                    AddThreshold = model.AddThreshold,
                    LostThreshold = model.LostThreshold

                    ,TimeCreate = model.TimeCreate
                }
            };
        }

        /// <summary>
        ///     Lấy toàn bộ danh sách Bình chứa nhiên liệu
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public FuelSheetGetMulti GetAll()
        {
            return new FuelSheetGetMulti
            {
                Status = 1,
                Description = "OK",
                FuelSheets = Cache.GetQueryContext<FuelSheet>().GetAll().Select(model => new FuelSheetTranfer
                {
                    Name = model.Name,
                    Note = model.Note,
                    BarrelType = model.BarrelType,
                    Params = model.Params,

                    Length = model.Length,
                    Height = model.Height,
                    MinValue = model.MinValue,
                    MinHz = model.MinHz,
                    MaxHz = model.MaxHz,

                    AddThreshold = model.AddThreshold,
                    LostThreshold = model.LostThreshold

                    ,
                    TimeCreate = model.TimeCreate

                }).ToList()
            };
        }


    }
}