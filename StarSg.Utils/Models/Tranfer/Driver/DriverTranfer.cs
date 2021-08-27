using System;

namespace Core.Models.Tranfer.Driver
{
    public class DriverTranfer
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Gplx { get; set; }
        public string Mnv { get; set; }
        public long CompanyId { get; set; }
        public long GroupId { get; set; }
        public int Born { get; set; }
        public string Cmnd { get; set; }
        public string Address { get; set; }
        public int Sex { get; set; }
        public DateTime CreateDateOfGplx { get; set; }
        public DateTime EndDateOfGplx { get; set; }
        public string AddressOfGplx { get; set; }
        public string Phone { get; set; }
    }
}