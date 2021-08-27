#region include

using System;
using System.Linq;
using Datacenter.Model.Entity;
using DevicePacketModels;
using Datacenter.Model.Log;
using Datacenter.Model.Utils;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using StarSg.Core;
using Datacenter.Api.Models;
using Datacenter.Model.Components;

#endregion

namespace Datacenter.Api.Core.DeviceLogicHandles.Logics
{
    /// <summary>
    ///     xử lý thông tin cây nhiên liệu
    /// </summary>
    [Sort(2)]
    public class FuelHandleLogic : ILogic
    {
        //public void Handle(P01SyncPacket packet, ILogicUtil uTils, Device device, Company company)
        public void Handle(PBaseSyncPacket packet, ILogicUtil uTils, Device device, Company company)
        {
            ////không xử lý cho gói nén
            //if (uTils.PacketType == 1) return;

            //Kiểm tra xe này đã có cấu hình chưa, nếu chưa có thì thoát ngay lập tức
            if (string.IsNullOrWhiteSpace(device.FuelSheet))
            {
                //nếu xe chưa có FuelSheet thì cập nhật giá trị hz vô Fuel để theo dõi ( yêu cầu từ Thái )
                device.Status.BasicStatus.Fuel = packet.Fuel;
                return;//không xử lý
            }

            //Lấy cấu hình ra
            var sheet = uTils.DataCache.GetQueryContext<FuelSheet>().GetByKey(device.FuelSheet);
            if (sheet == null)
                return;//không xử lý

            //Tạo biến xử lý sự kiện nếu chưa có
            if (device.Temp.FuelTest == null) device.Temp.FuelTest = new FuelTest();

            //Kiểm tra cờ cảm ứng
            if (!device.Status.BasicStatus.UseFuel)
            {
                //if (device.Status.BasicStatus.Fuel > 0) device.Status.BasicStatus.Fuel = 0;
                //uTils.Log.Info("FuelHandleLogic", $"Thiết bị sai {device.Serial} Fuel Âm ={packet.Fuel} UseFuel={device.Status.BasicStatus.UseFuel}");

                //Vẫn kiểm tra và xử lý sự kiện theo giá trị dầu trước đó theo thời gian hiện hành
                ProcessEvent(packet, uTils, device, company, sheet);
                return;//không xử lý
            }

            //Kiểm tra giá trị hz dương
            if (packet.Fuel <= 0)
            {
                //Vẫn kiểm tra và xử lý sự kiện theo giá trị dầu trước đó theo thời gian hiện hành
                ProcessEvent(packet, uTils, device, company, sheet);
                return;//không xử lý
            }

            //Bắt đầu tính toán giá trị dầu
            try
            {
                //Giá trị nhiên liệu tính toán ra hiện hành
                int fuelValue = 0;

                //Kiểm tra loại bình chứa có phải bình đo tay không
                if(sheet.BarrelType== (int)eBarrelType.Manually)
                {
                    fuelValue = sheet.GetFuelCapacityByHz(packet.Fuel);
                }
                //các loại bình chứa còn lại
                else
                {
                    int MinHeight = sheet.Height;
                    int MinValue = sheet.MinValue;

                    //lấy thông tin tham số theo thiết bị nếu có
                    if (device.FuelParamList != null && device.FuelParamList.Length >= 2)
                    {
                        if (device.FuelParamList[0] >= 0) MinHeight = device.FuelParamList[0];
                        if (device.FuelParamList[1] >= 0) MinValue = device.FuelParamList[1];
                    }

                    //Lấy thông tin tỉ lệ chiều cao và Hz để tìm chiều cao từ Hz
                    double deltah = (double)sheet.Length / (sheet.MaxHz - sheet.MinHz);

                    if ((sheet.MinHz < sheet.MaxHz && packet.Fuel >= sheet.MinHz)  //bình chuẩn
                        || (sheet.MinHz > sheet.MaxHz && packet.Fuel <= sheet.MinHz) //bình đảo Hz
                         )
                    {
                        //tính ra giá trị độ cao tương ứng hiện hành
                        int totalh = (int)Math.Round(deltah * (packet.Fuel - sheet.MinHz)) + MinHeight;

                        //Tổng tính được từ đáy theo lý thuyết
                        fuelValue = sheet.GetFuelCapacity(totalh);

                        //Nếu có nhập giá trị dưới đáy  bình
                        if (MinValue > 0)
                            fuelValue = fuelValue
                                - sheet.GetFuelCapacity(MinHeight) // giá trị dưới đáy theo lý thuyết
                                + MinValue;// giá trị dưới đáy nhập vào
                    }
                    else // sẽ không xảy ra tại đây... tuy nhiên cứ làm cho chắc ăn
                    {
                        //tính ra giá trị độ cao tương ứng hiện hành
                        int totalh = (int)Math.Round(deltah * packet.Fuel);

                        //Tổng tính được từ đáy theo lý thuyết
                        fuelValue = sheet.GetFuelCapacity(totalh);
                    }

                    if (fuelValue < 0)
                    {
                        uTils.Log.Info("FuelHandleLogic", $"Thiết bị {device.Serial} Fuel Âm ={packet.Fuel} fuelValue={fuelValue}");
                        //Vẫn kiểm tra và xử lý sự kiện theo giá trị dầu trước đó theo thời gian hiện hành
                        ProcessEvent(packet, uTils, device, company, sheet);
                        return;//lỗi
                    }

                    int maxval = sheet.MaxFuelCapacity;//.GetMaxFuelCapacity();
                    if (maxval > 0)
                    {
                        //nếu vượt quá 1000 ml thì không xét
                        if (fuelValue > maxval + 1000)
                        {
                            uTils.Log.Info("FuelHandleLogic", $"Thiết bị {device.Serial} Fuel Vượt giới hạn 1000ml {packet.Fuel} fuelValue={fuelValue}/{maxval}");
                            //Vẫn kiểm tra và xử lý sự kiện theo giá trị dầu trước đó theo thời gian hiện hành
                            ProcessEvent(packet, uTils, device, company, sheet);
                            return;//lỗi
                        }
                        //nếu chấp nhận được thì gán bằng giá trị max
                        else if (fuelValue > maxval)
                        {
                            uTils.Log.Info("FuelHandleLogic", $"Thiết bị {device.Serial} Fuel Vượt giới hạn <= 1000ml {packet.Fuel} fuelValue={fuelValue}/{maxval}");
                            fuelValue = maxval;
                        }
                    }
                }

                //cập nhật lại Fuel cho Device Status
                device.Status.BasicStatus.Fuel = fuelValue;
            }
            catch (Exception e)
            {
                uTils.Log.Exception("FuelHandleLogic", e, "UpdateFuelValueFromHz");
            }

            //Xử lý sự kiện
            ProcessEvent(packet, uTils, device, company, sheet);
        }

        private void ProcessEvent(PBaseSyncPacket packet, ILogicUtil uTils, Device device, Company company, FuelSheet sheet)
        {
            try
            {
                //Kiểm tra và tự động phát hiện sự kiện
                List<FuelEventCandidate> ret = device.Temp.FuelTest.CheckAndGetEvents(
                    device.Status.BasicStatus.GpsInfo.Lat
                    , device.Status.BasicStatus.GpsInfo.Lng
                    , device.Status.BasicStatus.Fuel
                    , device.Status.BasicStatus.ClientSend
                    , sheet.MaxFuelCapacity //, sheet.GetMaxFuelCapacity()
                    ,device.DeviceType
                    ); // dầu rút tối thiểu

                if (ret != null && ret.Count > 0)
                {
                    uTils.Log.Info("FuelHandleLogic", $"Thiết bị {device.Serial} Fuel={packet.Fuel} fuelValue={device.Status.BasicStatus.Fuel}");

                    if (ret.Count > 1) ret.Sort();

                    foreach (var item in ret)
                    {
                        //kiểm tra và loại bõ giá trị nhiễu trực tiếp ( sau này nâng cấp sẽ loại bõ bên trong engine hoặc sử dụng cách thông minh hơn )
                        if (sheet.AddThreshold > 0 && item.SumDelta >= 0 && item.SumDelta < sheet.AddThreshold) continue;
                        if (sheet.LostThreshold > 0 && item.SumDelta < 0 && -item.SumDelta < sheet.LostThreshold) continue;


                        GpsLocation loc = new GpsLocation() { Lat = item.MaxDeltaLat, Lng = item.MaxDeltaLng };
                        if (loc.Lng == 0 && loc.Lat == 0) loc = device.Status.BasicStatus.GpsInfo;
                        uTils.LocationQuery.GetAddress(loc);

                        //Tính lại giá trị thay đổi dựa vào giá trị dầu ở sự kiện trước và giá trị tại sự kiện này
                        float SumDelta = item.SumDelta;

                        //Thời điểm sự kiện là lúc độ lệch lớn nhất
                        DateTime eventtime = item.MaxDeltaTime;

                        //cập nhật báo cáo tổng hợp (các sự kiện xuyên qua 0 giờ phải đc tính cho ngày hôm sau)
                        if (device.Temp.GeneralReportLog != null && item.PrevTime.Date == device.Temp.GeneralReportLog.UpdateTime)
                        {
                            if (SumDelta >= 0) device.Temp.GeneralReportLog.AddFuel += SumDelta;
                            else device.Temp.GeneralReportLog.LostFuel -= SumDelta;
                        }

                        //thời gian sự kiện bị lùi lại, cập nhật trực tiếp vô database
                        else
                        {
                            try
                            {
                                var updateobjs = uTils.DataContext.GetWhere<GeneralReportLog>(m => m.GuidId == device.Indentity && m.UpdateTime == item.PrevTime.Date, company.DbId).ToList();
                                if (updateobjs.Count > 0)
                                {
                                    if (SumDelta >= 0) updateobjs[0].AddFuel += SumDelta;
                                    else updateobjs[0].LostFuel -= SumDelta;

                                    uTils.DataContext.Update(updateobjs[0], company.DbId);
                                    uTils.DataContext.Commit(company.DbId);
                                }
                            }
                            catch (Exception e)
                            {
                                uTils.Log.Exception("FuelHandleLogic", e, "Update GeneralReportLog");
                            }
                        }

                        //nếu thời gian bắt đầu và kết thúc khác ngày thì thời điểm xảy ra là lúc kết thúc sự kiện
                        if (item.PrevTime.Date != item.FistTime.Date) eventtime = item.PrevTime;

                        //cập nhật sự kiện
                        uTils.DataContext.Insert(new FuelTraceLog
                        {
                            CompanyId = company.Id,
                            GroupId = device.GroupId,
                            DbId = company.DbId,
                            DeviceId = device.Serial,
                            DriverId = device.Status?.DriverStatus?.DriverId ?? 0,
                            Id = 0,
                            Time = eventtime,
                            Location = loc,
                            CurrentValue = (int)item.PrevValue,
                            Delta = (int)SumDelta,
                            TimeBegin = item.FistTime
                        }, company.DbId);


                        //nhắn tin SMS
                        if (device.SmsAlarm && !String.IsNullOrWhiteSpace(device.OwnerPhone) && !string.IsNullOrWhiteSpace(ResponseDataConfig.GeoServerUrl))
                        {
                            String sms = "Xe " + device.Bs + ": ";
                            if (SumDelta > 0)
                                sms += "them " + Math.Round(SumDelta / 1000, 1) + " lit";
                            else
                                sms += "mat " + Math.Round(SumDelta / 1000, 1) + " lit";

                            sms += ", luc " + eventtime.ToString("HH:mm") + " ngay " + eventtime.ToString("dd/MM");

                            if (!String.IsNullOrWhiteSpace(loc.Address))
                            {
                                sms += ", tai ";
                                sms += TrimForSMS(RemoveVietnameseMark(loc.Address), 160 - sms.Length);
                            }

                            try
                            {
                                List<String> phonenos = device.OwnerPhone.Split('|', ',', ';').Where(m => !string.IsNullOrWhiteSpace(m)).Select(m => m.Trim()).ToList();
                                if (phonenos.Count > 0)
                                {
                                    SmsSendBody smsbody = new SmsSendBody()
                                    {
                                        content = sms,
                                        to = phonenos
                                        //to = new List<string>() { device.OwnerPhone }
                                    };

                                    //gui sms
                                    var smsSendResponse = new ForwardApi().Post<SmsSendResponse>($"{ResponseDataConfig.GeoServerUrl}/sms/send", smsbody);
                                    if (smsSendResponse != null && "success".Equals(smsSendResponse.status))
                                    {
                                        uTils.Log.Info("FuelHandleLogic", $"Đã gửi tin nhắn thiết bị {device.Serial} đến số {device.OwnerPhone} nội dung={sms}");
                                    }
                                    else
                                    {
                                        uTils.Log.Error("FuelHandleLogic", $"ERROR sms/send thiết bị {device.Serial} đến số {device.OwnerPhone} lỗi={(smsSendResponse != null && smsSendResponse.message != null ? smsSendResponse.message : "")}");
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                uTils.Log.Exception("FuelHandleLogic", e, "sms/send");
                            }

                        }

                    }

                }
            }
            catch (Exception e)
            {
                uTils.Log.Exception("FuelHandleLogic", e, "ProcessEvent");
            }
        }

        static readonly char[] SEPS = { ',' };
        public static string TrimForSMS(string accented,int len)
        {
            if (string.IsNullOrWhiteSpace(accented)) return accented;
            String[] ss = accented.Split(SEPS, StringSplitOptions.RemoveEmptyEntries);

            string ret = "";
            for (int i = 0; i < ss.Length; i++)
            {
                if (ret.Length + ss[i].Length + 2 > len) return ret;
                if(ret.Length>0)
                    ret += ", " + ss[i].Trim();
                else
                    ret += ss[i].Trim();
            }
            return ret;
        }

        public static string RemoveVietnameseMark(string accented)
        {
            if (string.IsNullOrWhiteSpace(accented)) return accented;
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string input = accented.Normalize(NormalizationForm.FormD);

            //return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
            return regex.Replace(input, string.Empty).Replace(Convert.ToChar(273), 'd').Replace(Convert.ToChar(272), 'D');
        }

        public class SmsSendResponse
        {
            public string status { get; set; }
            public string message { get; set; }
        }
        public class SmsSendBody
        {
            public List<String> to { get; set; }
            public string content { get; set; }
            public string type { get; set; }//brand or empty
        }

    }
}