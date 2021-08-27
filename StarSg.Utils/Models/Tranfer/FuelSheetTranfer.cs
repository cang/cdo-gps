using System;

namespace Core.Models.Tranfer
{
    public class FuelSheetTranfer
    {
        /// <summary>
        /// Tên Loại Bình Nhiên Liệu
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Ghi Chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Dạng bình : 1 : hình khối chữ nhật, 2 : hình trụ
        /// Khối chữ nhật : Params = chiều cao,rộng,ngang theo mm
        /// Khối trụ  : Params = chiều cao,đường kính theo mm
        /// </summary>
        public int BarrelType { get; set; }

        /// <summary>
        /// Các tham số liên quan, cách nhau dấu '|', ',', ';', ' '
        /// </summary>
        public string Params { get; set; }

        /// <summary>
        /// Kích thước cây nhiên liệu (mm)
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Độ cao của đầu mút cột nhiên liệu từ đáy bình (mm)
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Giá trị dầu ứng với độ cao của đầu mút cột nhiên liệu (ml)
        /// </summary>
        public int MinValue { get; set; }

        /// <summary>
        /// Tầng số tương ứng với đầu cột nhiên liệu (Hz)
        /// </summary>
        public int MinHz { get; set; }

        /// <summary>
        /// Tầng số tương ứng với cuối cột nhiên liệu (Hz)
        /// </summary>
        public int MaxHz { get; set; }


        /// <summary>
        /// Giá trị dầu tối thiểu bị hút (ml)(tùy chọn), mặc định 0 là không xét 
        /// </summary>
        public int LostThreshold { get; set; }

        /// <summary>
        /// Giá trị dầu tối thiểu thêm vô (ml)(tùy chọn), mặc định 0 là không xét
        /// </summary>
        public int AddThreshold { get; set; }

        /// <summary>
        /// Ngay tao
        /// </summary>
        public DateTime TimeCreate { get; set; }

    }



}