using System.Web;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Collections.Generic;
using System;

namespace WhatsappService.Helpers {

    public static class  ConversionHelpers {
        public static Dictionary<string,string> ToDictonary(this NameValueCollection obj) {
            Dictionary<string,string> dictionary = new Dictionary<string, string>();
            foreach(string key in obj.AllKeys) 
            {
                dictionary.Add(key,obj[key]);
            }
            return dictionary;
        }
        
        public static Status queryString2Status( string sample) 
        {   
            var objSample=HttpUtility.ParseQueryString(HttpUtility.UrlDecode(sample));
            Dictionary<string,string> dictionary = objSample.ToDictonary();
            string json = JsonConvert.SerializeObject(dictionary);
            return JsonConvert.DeserializeObject<Status>(json);
        }

        public static UserMessageContainer queryString2UserResponse( string sample) 
        {   
            var objSample=HttpUtility.ParseQueryString(HttpUtility.UrlDecode(sample));
            Dictionary<string,string> dictionary = objSample.ToDictonary();
            string json = JsonConvert.SerializeObject(dictionary);
            return JsonConvert.DeserializeObject<UserMessageContainer>(json);
        }
    }
}