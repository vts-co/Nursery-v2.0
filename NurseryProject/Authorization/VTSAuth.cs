using NurseryProject.Utilities;
using System;
using System.Web;

namespace NurseryProject.Authorization
{
    public class VTSAuth
    {
        readonly string cookiename = "ElmohandesNursery";
        readonly int cookieKeyCount = 1;
        readonly int cookieDays = 30;

        public UserInfo CookieValues { get; set; }
        //private Guid SessionID { get; set; }


        /// <summary>
        /// check if the cookies has values and the values count is equal to or bigger than the cookie keys count
        /// </summary>
        public bool CheckCookies() => HttpContext.Current.Request.Cookies[cookiename] != null && HttpContext.Current.Request.Cookies[cookiename].Values.Count >= cookieKeyCount;
        /// <summary>
        /// Load Data saved in the cookies
        /// </summary>
        /// <returns>true if data existed in the cookies</returns>
        public bool LoadDataFromCookies()
        {
            if (CheckCookies())
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[cookiename];
                CookieValues = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfo>(Security.Decrypt(cookie.Values[0]));
                return true;
            }
            return false;
        }


        internal void ClearCookies()
        {
            if (HttpContext.Current.Request.Cookies[cookiename] != null)
            {
                HttpContext.Current.Response.Cookies[cookiename].Expires = DateTime.UtcNow.AddDays(-1);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        internal bool SaveToCookies(UserInfo cookieValues)
        {
            if (cookieValues != null)
            {
                HttpCookie cookie = new HttpCookie(cookiename);
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(cookieValues);
                cookie.Values.Add("k0", Security.Encrypt(jsonString.ToString()));
                cookie.Expires = DateTime.UtcNow.AddDays(cookieDays);
                HttpContext.Current.Response.Cookies.Add(cookie);
                return true;
            }
            return false;
        }

        public static bool UpdateUserInfo(string username)
        {
            var auth = new VTSAuth();
            if (auth.LoadDataFromCookies())
            {
                auth.CookieValues.UserName = username;
                auth.SaveToCookies(auth.CookieValues);
                return true;
            }
            return false;
        }
    }

    public class VTSCookieVistor
    {
        readonly string cookiename = "TLMS" + ".Vistor";
        readonly int cookieKeyCount = 1;
        readonly int cookieDays = 365;

        public VisitorInfo CookieValues { get; set; }
        //private Guid SessionID { get; set; }


        /// <summary>
        /// check if the cookies has values and the values count is equal to or bigger than the cookie keys count
        /// </summary>
        public bool CheckCookies() => HttpContext.Current.Request.Cookies[cookiename] != null && HttpContext.Current.Request.Cookies[cookiename].Values.Count >= cookieKeyCount;
        /// <summary>
        /// Load Data saved in the cookies
        /// </summary>
        /// <returns>true if data existed in the cookies</returns>
        public bool LoadDataFromCookies()
        {
            if (CheckCookies())
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[cookiename];
                CookieValues = Newtonsoft.Json.JsonConvert.DeserializeObject<VisitorInfo>(Security.Decrypt(cookie.Value));
                return true;
            }
            return false;
        }


        internal void ClearCookies()
        {
            if (HttpContext.Current.Request.Cookies[cookiename] != null)
            {
                HttpContext.Current.Response.Cookies[cookiename].Expires = DateTime.UtcNow.AddDays(-1);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        internal bool SaveToCookies(VisitorInfo cookieValues)
        {
            if (cookieValues != null)
            {
                HttpCookie cookie = new HttpCookie(cookiename);
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(cookieValues);
                cookie.Value = Security.Encrypt(jsonString.ToString());
                cookie.Expires = DateTime.UtcNow.AddDays(cookieDays);
                HttpContext.Current.Response.Cookies.Add(cookie);
                return true;
            }
            return false;
        }
        public class VisitorInfo
        {
            public Guid Id { get; set; }
        }
    }

}