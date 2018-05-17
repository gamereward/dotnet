using System;
using System.Collections.Generic;
using System.IO;
namespace GameReward
{
    public class GrdManager
    {
        private const string mainNetUrl = "https://gamereward.io/appapi/";
        private const string testNetUrl = "https://test.gamereward.io/appapi/";
        private static string api_Id = "";
        private static string api_Secret = "";
        private static string url = "";
        public static void Init(string appId, string secret,GrdNet net)
        {
            api_Id = appId;
            api_Secret = secret;
            url = net == GrdNet.Main ? mainNetUrl : testNetUrl;
        }

        public static string Md5(string s)
        {
            System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
            byte[] bytes = ue.GetBytes(s);

            // encrypt bytes
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(bytes);
            // Convert the encrypted bytes back to a string (base 16)
            string hashstring = "";

            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashstring += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }

            return hashstring.PadLeft(32, '0');
        }

        private static string RequestHttp(string action, System.Collections.Generic.Dictionary<string, string> parameters, bool isGet)
        {
            if (isGet)
            {
                return GetData(url + action, parameters);
            }
            return PostData(url + action, parameters);

        }

        private static Dictionary<string, object> GetJsonObject(string json)
        {
            Dictionary<string, object> result = null;
            try
            {
                result = (Dictionary<string, object>)Json.Deserialize(json);
            }
            catch
            {
            }
            if (result == null)
            {
                result = new Dictionary<string, object>();
                result.Add("error", 100);
                result.Add("message", json);
            }
            return result;
        }

        private static string GetAppToken()
        {
            int t = (Int32)(DateTime.UtcNow-new DateTime(1970, 1, 1)).TotalSeconds;
            t = t / 15;
            int k = (int)(t % 20);
            int len = api_Secret.Length / 20;
            string str = api_Secret.Substring(k * len, len);
            str = GrdManager.Md5(str + t) + "&sec=" + t;
            return str;
        }

        private static string GetSendData(System.Collections.Generic.Dictionary<string, string> parameters)
        {
            string data = "api_id=" + api_Id + "&api_key=" + GetAppToken();

            if (parameters != null)
            {
                foreach (string key in parameters.Keys)
                {
                    data += "&" + key + "=" + parameters[key];
                }
            }
            return data;
        }
        private static string GetData(string urlstring, System.Collections.Generic.Dictionary<string, string> parameters)
        {
            string data = GetSendData(parameters);
            try
            {
                System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(urlstring + "/" + data);
                System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)webRequest.GetResponse();
                string result = "";
                using (System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                    int iPos = result.IndexOf("{");
                    if (iPos > 0)
                    {
                        result = result.Substring(iPos);
                    }
                }
                return result;

            }
            catch (Exception e)
            {
                return "{\"error\":100,\"message\":\"" + e.Message + "\"}";
            }
            return "";
        }
        private static string PostData(string urlstring, Dictionary<string, string> parameters)
        {
            string data = GetSendData(parameters);
            string result = "";
            try
            {
                System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(urlstring);
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(data);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.ContentLength = buffer.Length;
                using (Stream strm = webRequest.GetRequestStream())
                {
                    strm.Write(buffer, 0, buffer.Length);
                }
                System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)webRequest.GetResponse();
                using (System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                    int iPos = result.IndexOf("{");
                    if (iPos > 0)
                    {
                        result = result.Substring(iPos);
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                return "{\"error\":100,\"message\":\"" + e.Message + "\"}";
            }
        }


        public static string GetAddressQRCode(string address)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("text", "gamereward:" + address);
            string data = RequestHttp("qrcode", parameters, false);
            return data;
        }

        public static GrdResult<GrdAccountInfo> AccountBalance(string username)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("username", username);
            string data = RequestHttp("accountbalance", parameters, false);
            Dictionary<string, object> obj = GetJsonObject(data);
            int error = 100;
            string message = "";
            GrdAccountInfo info = null;
            try
            {
                error = int.Parse(obj["error"].ToString());
            }
            catch (Exception e)
            {
            }
            if (error == 0)
            {
                info = Json.GetObject<GrdAccountInfo>(obj);
            }
            else
            {
                try
                {
                    message = obj["message"].ToString();
                }
                catch (Exception e)
                {
                }
            }
            return new GrdResult<GrdAccountInfo>(error, message, info);
        }

        public static GrdResultBase ChargeMoney(string username, decimal money)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("username", username);
            parameters.Add("value", money.ToString());
            string data = RequestHttp("chargemoney", parameters, false);
            Dictionary<string, object> obj = GetJsonObject(data);
            int error = 100;
            string message = "";
            try
            {
                error = int.Parse(obj["error"].ToString());
            }
            catch (Exception e)
            {
            }
            try
            {
                message = obj["message"].ToString();
            }
            catch (Exception e)
            {
            }
            return new GrdResultBase(error, message);
        }

        public static GrdResult<GrdTransaction[]> GetTransactions(string username, int start, int count)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("username", username);
            parameters.Add("start", start + "");
            parameters.Add("count", count + "");
            string data = RequestHttp("transactions", parameters, false);
            Dictionary<string, object> jsonObject = GetJsonObject(data);
            int error = 100;
            string message = "";
            GrdTransaction[] transactions = null;
            try
            {
                error = int.Parse(jsonObject["error"].ToString());
            }
            catch (Exception e)
            {
            }
            if (error != 0)
            {
                try
                {
                    message = jsonObject["message"].ToString();
                }
                catch (Exception e)
                {
                }
            }
            else
            {
                try
                {
                    transactions = Json.GetObject<GrdTransaction[]>(jsonObject["transactions"]);
                }
                catch (Exception e)
                {
                }
            }
            return new GrdResult<GrdTransaction[]>(error, message, transactions);
        }

        public static GrdResult<int> GetTransactionCount(string username)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("username", username);
            string data = RequestHttp("counttransactions", parameters, false);
            Dictionary<string, object> jsonObject = GetJsonObject(data);
            int error = 100;
            string message = "";
            int count = 0;
            try
            {
                error = int.Parse(jsonObject["error"].ToString());
            }
            catch (Exception e)
            {
            }
            if (error != 0)
            {
                try
                {
                    message = jsonObject["message"].ToString();
                }
                catch (Exception e)
                {
                }
            }
            else
            {
                try
                {
                    count = (int)jsonObject["count"];
                }
                catch (Exception e)
                {
                }
            }
            return new GrdResult<int>(error, message, count);
        }
        public static GrdResult<GrdSessionData[]> GetUserSessionData(string username, string store, string key, int start, int count)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("username", username);
            parameters.Add("store", store);
            parameters.Add("keys", key);
            parameters.Add("start", start + "");
            parameters.Add("count", count + "");
            string data = RequestHttp("getusersessiondata", parameters, false);
            Dictionary<string, object> jsonObject = GetJsonObject(data);
            int error = 100;
            string message = "";
            GrdSessionData[] list = null;
            try
            {
                error = int.Parse(jsonObject["error"].ToString());
            }
            catch (Exception e)
            {
            }
            if (error != 0)
            {
                try
                {
                    message = jsonObject["message"].ToString();
                }
                catch (Exception e)
                {

                }
            }
            else
            {
                try
                {
                    list = Json.GetObject<GrdSessionData[]>(jsonObject["data"]);
                }
                catch (Exception e)
                {

                }
            }
            return new GrdResult<GrdSessionData[]>(error, message, list);
        }
        public static GrdResult<GrdSessionData[]> GetUserSessionData(string username, string store, string[] keys, int start, int count)
        {
            string sKey = "";
            for (int i = 0; i < keys.Length; i++)
            {
                sKey += "," + keys[i];
            }
            if (sKey.Length > 0)
            {
                sKey = sKey.Substring(1);
            }
            return GetUserSessionData(username, store, sKey, start, count);
        }
        public static GrdResultBase SaveUserScore(string username, string scoreType, double score)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("username", username);
            parameters.Add("scoretype", scoreType);
            parameters.Add("score", score + "");
            string data = RequestHttp("saveuserscore", parameters, false);
            Dictionary<string, object> jsonObject = GetJsonObject(data);
            int error = 100;
            try
            {
                error = int.Parse(jsonObject["error"].ToString());
            }
            catch (Exception e)
            {

            }
            string message = "";
            if (error != 0)
            {
                try
                {
                    message = jsonObject["message"].ToString();
                }
                catch (Exception e)
                {

                }
            }
            return new GrdResultBase(error, message);
        }
        public static GrdResultBase IncreaseUserScore(string username, string scoreType, double score)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("username", username);
            parameters.Add("scoretype", scoreType);
            parameters.Add("score", score + "");
            string data = RequestHttp("increaseuserscore", parameters, false);
            Dictionary<string, object> jsonObject = GetJsonObject(data);
            int error = 100;
            try
            {
                error = int.Parse(jsonObject["error"].ToString());
            }
            catch (Exception e)
            {

            }
            string message = "";
            if (error != 0)
            {
                try
                {
                    message = jsonObject["message"].ToString();
                }
                catch (Exception e)
                {

                }
            }
            return new GrdResultBase(error, message);
        }
        public static GrdResult<GrdLeaderBoard> GetUserScoreRank(string username, string scoreType)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("username", username);
            parameters.Add("scoretype", scoreType);
            string data = RequestHttp("getuserscore", parameters, false);
            Dictionary<string, object> jsonObject = GetJsonObject(data);
            int error = 100;
            string message = "";
            GrdLeaderBoard score = null;
            try
            {
                error = int.Parse(jsonObject["error"].ToString());
            }
            catch (Exception e)
            {

            }
            if (error != 0)
            {
                try
                {
                    message = jsonObject["message"].ToString();
                }
                catch (Exception e)
                {

                }
            }
            else
            {
                score = Json.GetObject<GrdLeaderBoard>(jsonObject);
            }
            return new GrdResult<GrdLeaderBoard>(error, message, score);
        }
        public static GrdResult<GrdLeaderBoard[]> GetLeaderBoard(string username, string scoreType, int start, int count)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("username", username);
            parameters.Add("scoretype", scoreType);
            parameters.Add("start", start + "");
            parameters.Add("count", count + "");
            string data = RequestHttp("getleaderboard", parameters, false);
            Dictionary<string, object> jsonObject = GetJsonObject(data);
            int error = 100;
            string message = "";
            GrdLeaderBoard[] list = null;
            try
            {
                error = int.Parse(jsonObject["error"].ToString());
            }
            catch (Exception e)
            {

            }
            if (error != 0)
            {
                try
                {
                    message = jsonObject["message"].ToString();
                }
                catch (Exception e)
                {

                }
            }
            else
            {
                try
                {
                    list = Json.GetObject<GrdLeaderBoard[]>(jsonObject["leaderboard"]);
                }
                catch (Exception e)
                {

                }
            }
            return new GrdResult<GrdLeaderBoard[]>(error, message, list);
        }

        public static GrdCustomResult CallServerScript(string username, string scriptName, string functionName, Object[] parameters)
        {
            Dictionary<string, string> pars = new Dictionary<string, string>();
            string vars = Json.Serialize(parameters);
            pars.Add("username", username);
            pars.Add("fn", functionName);
            pars.Add("script", scriptName);
            pars.Add("vars", vars);
            string data = RequestHttp("callserverscript", pars, false);
            Dictionary<string, object> obj = GetJsonObject(data);
            int error = 100;
            try
            {
                error = int.Parse(obj["error"].ToString());
            }
            catch (Exception e)
            {

            }
            Object rdata = null;
            string message = "";
            if (error != 0)
            {
                try
                {
                    message = obj["message"].ToString();
                }
                catch (Exception e)
                {

                }
            }
            else
            {
                try
                {
                    rdata = obj["result"];
                }
                catch (Exception e)
                {

                }
            }
            return new GrdCustomResult(error, message, rdata);
        }


        public static GrdResultBase Transfer(string username, string address, decimal money)
        {
            Dictionary<string, string> pars = new Dictionary<string, string>();
            pars.Add("username", username);
            pars.Add("to", address);
            pars.Add("value", money.ToString());
            string data = RequestHttp("transfer", pars, false);
            Dictionary<string, object> jsonObject = GetJsonObject(data);
            int error = 100;
            try
            {
                error = int.Parse(jsonObject["error"].ToString());
            }
            catch (Exception e)
            {

            }
            string message = "";
            if (error != 0)
            {
                try
                {
                    message = jsonObject["message"].ToString();
                }
                catch (Exception e)
                {

                }
            }
            return new GrdResultBase(error, message);
        }

    }
    public enum GrdNet
    {
        Main,Test
    }
}