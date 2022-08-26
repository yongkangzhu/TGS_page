using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Utils;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TGS_PAGE.Model;
using TGS_PAGE.Utils;
using System.Text.Json;

namespace WebApplication1
{
    [Route("api/[controller]/[action]")]
    public class TgsInfoController : Controller
    {
        private readonly TgsInfoManager _tgsInfo;

        public TgsInfoController(TgsInfoManager tgsInfo)
        {
            this._tgsInfo = tgsInfo;
        }

        [HttpGet]
        public IActionResult all()
        {
            return Json(_tgsInfo.TgsInfoList);
        }

        [HttpGet]
        public IActionResult GetNextIP([FromQuery]string ip)
        {
            return Json(_tgsInfo.recommendIP(ip),new JsonSerializerOptions { WriteIndented = true});
        }



    }


}
