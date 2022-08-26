using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TGS_PAGE.Model
{

    public class m
    {
        public string gateway_id { get; set; }
        public string server_id { get; set; }
        public string server_name { get; set; }
        public string driver_id { get; set; }
        public string driver_name { get; set; }
        public string terminal_name { get; set; }
        public string device_id { get; set; }
        public string ip { get; set; }

        public string server_ip { get; set; }
    }
    public class m2
    {
        public string server_name { get; set; }
        public string driver_name { get; set; }
        public string terminal_name { get; set; }
        public string device_id { get; set; }
        public byte[] driver_par { get; set; }
    }
    public class m3
    {
        public string name { get; set; }
        public string ip { get; set; }
    }

    public class m4
    {
        public string prefix3 { get; set; }
        public string tail { get; set; }
    }
}
