using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace We7Tools.Extend
{
    public static class We7SessionExtends
    {
        public static JObject GetWe7Data(this Microsoft.AspNetCore.Http.ISession session)
        {
            JObject jObject = null;
            try
            {
                byte[] dataArray;
                session.TryGetValue(ConfigData.We7Config.We7DataSessionName, out dataArray);
                if (dataArray == null)
                {
                    return null;
                }
                var json = Encoding.UTF8.GetString(dataArray);
                jObject = (JObject)JsonConvert.DeserializeObject(json);
            }
            catch (Exception)
            {
                //throw;
            }
            return jObject;
        }
        public static bool HasWe7Data(this Microsoft.AspNetCore.Http.ISession session)
        {
            return GetWe7Data(session)!=null;
        }

        public static void PushWe7Data(this Microsoft.AspNetCore.Http.ISession session, string data)
        {
            session.Set(ConfigData.We7Config.We7DataSessionName, Encoding.UTF8.GetBytes(data));
        }
        public static string GetUserName(this Microsoft.AspNetCore.Http.ISession session)
        {
            var jObj = session.GetWe7Data();
            if (jObj==null)
            {
                return null;
            }
            return (string)jObj["username"];
        }
      public static string GetUniacID(this Microsoft.AspNetCore.Http.ISession session)
        {
            var jObj = GetWe7Data(session);
            if (jObj==null)
            {
                return null;
            }
            return (string)jObj["uniacid"];
        }
    }



}
