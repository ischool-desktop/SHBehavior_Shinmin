using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.DSAUtil;
using K12.Data.Utility;

namespace K12.Behavior.Shinmin
{
    public static class Config
    {
        public static DSResponse GetMDReduce()
        {
            return DSAServices.CallService("SmartSchool.Config.GetMDReduce", new DSRequest());
        }

        public static void Update(DSRequest request)
        {
            DSAServices.CallService("SmartSchool.Config.UpdateList", request);
        }

        public static DSResponse GetDisciplineReasonList()
        {
            string serviceName = "SmartSchool.Config.GetDisciplineReasonList";

            // 拿掉快取
            //if (DataCacheManager.Get(serviceName) == null)
            //{

            DSRequest request = new DSRequest();
            DSXmlHelper helper = new DSXmlHelper("GetDisciplineReasonListRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            request.SetContent(helper);
            DSResponse dsrsp = DSAServices.CallService(serviceName, request);
            return dsrsp;

            //DataCacheManager.Add(serviceName, dsrsp);
            //}
            //return DataCacheManager.Get(serviceName);
        }
    }
}
