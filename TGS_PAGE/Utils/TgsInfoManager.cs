using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS_PAGE.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;
using System.Text;

namespace TGS_PAGE.Utils
{
    public class TgsInfoManager
    {
        private readonly IConfiguration _config;
        private readonly IDBHelper _dBHelper;
        private readonly IMemoryCache _cache;

        public List<m> TgsInfoList => GetAllInfo();

        public TgsInfoManager( IConfiguration config, 
            IDBHelper dBHelper, 
            IMemoryCache cache)
        {
            this._config = config ?? throw new NullReferenceException();
            this._dBHelper = dBHelper ?? throw new NullReferenceException();
            this._cache = cache ?? throw new NullReferenceException();

            dBHelper.cstr = _config.GetConnectionString(_config["AppSetting:use_db"]);
        }

        private List<m> GetAllInfo()
        {
            var TgsInfoList = new List<m>();


            if (_cache.TryGetValue(nameof(TgsInfoList), out TgsInfoList))
            {
                return TgsInfoList;
            } 

            List<m3> servers_ip = new List<m3>();

            string sql_host = "select * from sajet.tgs_host_address";

            var dt = _dBHelper.Query(sql_host);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    servers_ip.Add(new m3
                    {
                        name = dt.Rows[i]["TGS_NAME"].ToString(),
                        ip = dt.Rows[i]["HOST_ADDRESS"].ToString(),
                    });
                }
            }



            // var servers_ip = JsonConvert.DeserializeObject<List<m3>>(str);

            StringBuilder builder = new StringBuilder();

            builder.AppendLine("select * from sajet.tgs_gateway_base ");

            dt = _dBHelper.Query(builder.ToString());

            var tt = dt.AsEnumerable().Cast<DataRow>().
                Select(x => new
                {
                    server_id = x.Field<decimal>("SERVER_id").ToString(),
                    driver_id = x.Field<decimal>("driver_id").ToString(),
                    gateway_id = x.Field<decimal>("GATEWAY_ID").ToString(),
                    driver_parameter = x.Field<byte[]>("DRIVER_PARAMETER")
                }).ToList();


            builder.Clear();

            builder.AppendLine("select C.GATEWAY_ID,D.SERVER_ID,D.SERVER_DESC_E,C.DRIVER_ID,E.DRIVER_DESC_E,b.TERMINAL_NAME,a.DEVICE_ID from sajet.TGS_TERMINAL_LINK a");
            builder.AppendLine("join SAJET.SYS_TERMINAL b on B.TERMINAL_ID = a.TERMINAL_ID");
            builder.AppendLine("join sajet.TGS_GATEWAY_BASE c on A.SERVER_ID = C.SERVER_ID and A.GATEWAY_ID = C.GATEWAY_ID");
            builder.AppendLine("join SAJET.TGS_SERVER_BASE d on A.SERVER_ID = D.SERVER_ID");
            builder.AppendLine("join SAJET.TGS_DRIVER_BASE e on c.DRIVER_ID = E.DRIVER_ID");
            builder.AppendLine("WHERE E.DRIVER_NAME NOT IN ( 'SAJET_SAMPLE.DLL' )");
            //builder.AppendLine("where A.SERVER_ID = 16");

            dt = _dBHelper.Query(builder.ToString());


            var data = dt.AsEnumerable().Cast<DataRow>().
                Select(x => new m
                {
                    gateway_id = x.Field<decimal>("GATEWAY_ID").ToString(),
                    server_id = x.Field<decimal>("server_id").ToString(),
                    server_name = x.Field<string>("SERVER_DESC_E"),
                    driver_id = x.Field<decimal>("driver_id").ToString(),
                    driver_name = x.Field<string>("DRIVER_DESC_E"),
                    terminal_name = x.Field<string>("TERMINAL_NAME"),
                    device_id = x.Field<decimal>("DEVICE_ID").ToString()
                }).ToList();

            data.ForEach(x =>
            {
                var parameter = tt.Find(
                    z => z.server_id == x.server_id
                    && z.driver_id == x.driver_id
                    && z.gateway_id == x.gateway_id
                    )?.driver_parameter;

                string str = Encoding.Default.GetString(parameter);
                //去除統計
                string tmp = str.Substring(str.IndexOf(";") + 1);
                //去除末尾
                tmp = tmp.Substring(0, tmp.Length - 1);

                string[] iplistarr = tmp.Split(',');

                var ipgroups = iplistarr.AsEnumerable().Select(z => new
                {
                    no = (iplistarr.ToList().IndexOf(z) + 1).ToString(),
                    ip = z
                }).ToList();

                x.ip = ipgroups.Find(z => z.no == x.device_id)?.ip;

                x.server_ip = servers_ip.Find(z => z.name.ToUpper() == x.server_name)?.ip;

            });

            data = data.Where(x => x.ip != null).ToList();

            if (Convert.ToBoolean(_config["AppSetting:append_kepware"]))
            {
                var plc_ips = JsonConvert.DeserializeObject<List<m>>(System.IO.File.ReadAllText("plc_ips.json"));

                data.AddRange(plc_ips);
            }
    
            //缓存60秒
            _cache.Set(nameof(TgsInfoList), data, new DateTimeOffset(DateTime.UtcNow.AddMinutes(5)));

            return data;
        }

        public List<string> recommendIP(string IP)
        {
            string IPSegment = IP.Substring(0, IP.LastIndexOf('.'));


            var ip_part_list = TgsInfoList.Where(x => x.ip.StartsWith(IPSegment)).Select(x => x.ip).ToList();

            List<string> ip_all_list = new List<string>();
            for (int i = 1; i <= 253; i++)
            {
                ip_all_list.Add($"{IPSegment}.{i}");
            }
            var no_setting_ip_list = ip_all_list.Except(ip_part_list).OrderBy(x=>Convert.ToByte(x.Split('.')[3]))
                .Select(x=>$"{x}\n").ToList();

            return no_setting_ip_list;
        }
    }
}
