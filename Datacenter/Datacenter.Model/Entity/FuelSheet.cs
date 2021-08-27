using System.Collections.Generic;
using System.Linq;
using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;
using System;

namespace Datacenter.Model.Entity
{
    /// <summary>
    /// Thông tin thùng chứa và cây nhiện liệu mặc định tương ứng
    /// </summary>
    [Table]
    public class FuelSheet : IEntity, ICacheModel
    {
        private static char[] SEPS = new char[] { '|', ',', ';', ' ' };

        [PrimaryKey(KeyGenerateType = KeyGenerateType.Manual)]
        public virtual string Name { get; set; }

        /// <summary>
        /// Ghi chú 
        /// </summary>
        [BasicColumn]
        public virtual string Note { get; set; }


        private int _BarrelType;
        /// <summary>
        /// Dạng thùng chứa
        /// </summary>
        [BasicColumn]
        public virtual int BarrelType
        {
            get
            {
                return _BarrelType;
            }
            set
            {
                _BarrelType = value;
                MaxFuelCapacity = GetMaxFuelCapacity();
            }
        }

        private int[] _ParamList;
        public virtual int[] ParamList
        {
            get
            {
                return _ParamList;
            }
            set
            {
                _ParamList = value;
                MaxFuelCapacity = GetMaxFuelCapacity();
            }
        }


        private int[] _ManuallyHz;
        public virtual int[] ManuallyHz
        {
            get
            {
                return _ManuallyHz;
            }
            set
            {
                _ManuallyHz = value;
                //MaxFuelCapacity = GetMaxFuelCapacity();
            }
        }

        private int[] _ManuallyMl;
        public virtual int[] ManuallyMl
        {
            get
            {
                return _ManuallyMl;
            }
            set
            {
                _ManuallyMl = value;
                MaxFuelCapacity = GetMaxFuelCapacity();
            }
        }


        private string _Params = "";
        /// <summary>
        /// Các tham số liên quan, cách nhau dấu '|', ',', ';', ' '
        /// </summary>
        [BasicColumn]
        public virtual string Params
        {
            get
            {
                return _Params;
            }
            set
            {
                _Params = value;
                if (string.IsNullOrWhiteSpace(_Params)) return;

                try
                {
                    //xử lý cho bình đo tay Manually
                    if (_Params.Contains(':'))
                    {
                        String[] ss = _Params.Split(':');
                        if(ss.Length==2)
                        {
                            String[] ssml = ss[0].Split(SEPS, StringSplitOptions.RemoveEmptyEntries);
                            String[] sshz = ss[1].Split(SEPS, StringSplitOptions.RemoveEmptyEntries);
                            if(ssml.Length==sshz.Length)
                            {
                                int[] retml = new int[ssml.Length];
                                int[] rethz = new int[sshz.Length];
                                for (int i = 0; i < sshz.Length; i++)
                                {
                                    retml[i] = int.Parse(ssml[i]);
                                    rethz[i] = int.Parse(sshz[i]);
                                }
                                ManuallyMl = retml;
                                ManuallyHz = rethz;
                           }
                        }
                    }
                    //xử lý còn lại
                    else
                    {
                        String[] ss = _Params.Split(SEPS, StringSplitOptions.RemoveEmptyEntries);
                        int[] ret = new int[ss.Length];
                        for (int i = ss.Length - 1; i >= 0; i--)
                        {
                            if (!String.IsNullOrWhiteSpace(ss[i]))
                                ret[i] = int.Parse(ss[i]);
                            else
                                ret[i] = -1;
                        }
                        ParamList = ret;
                    }
                }
                catch 
                {
                }

            }
        }

        /// <summary>
        /// Kích thước cây nhiên liệu (mm)
        /// </summary>
        [BasicColumn]
        public virtual int Length { get; set; }

        /// <summary>
        /// Độ cao của đầu mút cột nhiên liệu từ đáy bình (mm)
        /// </summary>
        [BasicColumn]
        public virtual int Height { get; set; }

        /// <summary>
        /// Giá trị dầu ứng với độ cao của đầu mút cột nhiên liệu (ml)
        /// </summary>
        [BasicColumn]
        public virtual int MinValue { get; set; }

        /// <summary>
        /// Tầng số tương ứng với đầu cột nhiên liệu (Hz)
        /// </summary>
        [BasicColumn]
        public virtual int MinHz { get; set; }

        /// <summary>
        /// Tầng số tương ứng với cuối cột nhiên liệu (Hz)
        /// </summary>
        [BasicColumn]
        public virtual int MaxHz { get; set; }

        /// <summary>
        /// Giá trị dầu tối thiểu bị hút (ml)(tùy chọn), mặc định 0 là không xét
        /// </summary>
        [BasicColumn]
        public virtual int LostThreshold { get; set; }

        /// <summary>
        /// Giá trị dầu tối thiểu thêm vô  (ml)(tùy chọn), mặc định 0 là không xét
        /// </summary>
        [BasicColumn]
        public virtual int AddThreshold { get; set; }

        /// <summary>
        /// Ngay tao
        /// </summary>
        [BasicColumn]
        public virtual DateTime TimeCreate { get; set; }


        public virtual void FixNullObject()
        {
        }

        /// <summary>
        /// Tính giá trị dầu theo chiều cao
        /// </summary>
        /// <param name="h">đơn vị mm</param>
        /// <returns>đơn vị ml</returns>
        public virtual int GetFuelCapacity(int h)
        {
            if (BarrelType == (int)eBarrelType.Rectangular && ParamList!=null &&  ParamList.Length >= 3)
            {
                return (int)Math.Round(ParamList[1] * ParamList[2] * h / 1000.0);
            }
            if (BarrelType == (int)eBarrelType.Cylinder && ParamList !=null && ParamList.Length >= 2)
            {
                if (h > ParamList[1]) h = ParamList[1];
                double radious = ParamList[1] / 2.0;

                //diện tích quạt 
                double sQuat = radious * radious * Math.Acos((radious - h) / radious);// *2/2;

                //diện tích tam giác bên trong
                double STriangle = (radious - h) * radious * Math.Sqrt(1 - (radious - h) * (radious - h) / (radious * radious));// *2/2;

                return (int)Math.Round((sQuat - STriangle) * ParamList[0] / 1000.0);
            }

            return 0;
        }


        /// <summary>
        /// Tính giá trị dầu theo hz
        /// </summary>
        /// <param name="hz">đơn vị hz</param>
        /// <returns>đơn vị ml</returns>
        public virtual int GetFuelCapacityByHz(int hz)
        {
            if (BarrelType == (int)eBarrelType.Manually && ManuallyHz != null && ManuallyMl != null && ManuallyHz.Length > 1 && ManuallyMl.Length > 1)
            {
                //correct
                if (hz < ManuallyHz[0]) hz = ManuallyHz[0];
                if (hz > ManuallyHz[ManuallyHz.Length - 1]) hz = ManuallyHz[ManuallyHz.Length - 1];

                //process
                for (int i = 0; i < ManuallyHz.Length-1; i++)
                {
                    if(hz >= ManuallyHz[i] && hz<= ManuallyHz[i+1])
                    {
                        return (int)Math.Round(ManuallyMl[i] +  (double)(hz - ManuallyHz[i])*(ManuallyMl[i + 1] - ManuallyMl[i]) / (ManuallyHz[i + 1] - ManuallyHz[i]));
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// Tính giá trị dầu tối đa
        /// </summary>
        /// <returns>đơn vị ml</returns>
        private /*virtual*/ int GetMaxFuelCapacity()
        {
            if (BarrelType == (int)eBarrelType.Rectangular && ParamList!=null &&  ParamList.Length >= 3)
                return GetFuelCapacity(ParamList[0]);

            if (BarrelType == (int)eBarrelType.Cylinder && ParamList != null  && ParamList.Length >= 2)
                return GetFuelCapacity(ParamList[1]);

            if (BarrelType == (int)eBarrelType.Manually && ManuallyMl != null && ManuallyMl.Length > 0)
                return ManuallyMl[ManuallyMl.Length - 1];

            return 0;
        }

        /// <summary>
        /// Giá trị dầu tối đa
        /// </summary>
        public virtual int MaxFuelCapacity { get; set; }

    }

    /// <summary>
    /// Dạng thùng chứa
    /// </summary>
    public enum eBarrelType
    {
        None,

        /// <summary>
        /// Khối chữ nhật : Param 1 = chiều cao , Param 2 = ngang, Param 3 = dài
        /// </summary>
        Rectangular,

        /// <summary>
        /// Khối trụ  Param 1 = chiều cao , Param 2 = đường kính
        /// </summary>
        Cylinder,

        /// <summary>
        /// Tham số đo theo từng nấc ví dụ 0,10,20,30,40,50:0,20,30,60,100,150 
        /// có nghĩa là 
        /// 0-10 lít ứng với 0-20hz
        /// 10-20 lít ứng với 20-30hz
        /// ...
        /// </summary>
        Manually
    }


}