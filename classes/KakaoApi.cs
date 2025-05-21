using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using mapApi.classes;

namespace mapApi.classes
{
    class KakaoApi
    {
		    // 검색 기능
        internal static List<MyLocale> Search(string query) 
        { 
            List<MyLocale> mls = new List<MyLocale>(); 
            
            string site = "https://dapi.kakao.com/v2/local/search/keyword.json"; 
            string rquery = string.Format("{0}?query={1}", site, query); 
            // 키워드가 전주한옥마을일 때 rquery 형태
            // "https://dapi.kakao.com/v2/local/search/keyword.json?query=전주한옥마을"
            
            WebRequest request = WebRequest.Create(rquery); 
            string rkey = "c099b94227df5f605dfed9f2a35e63c7"; 
            string header = "KakaoAK " + rkey;
            request.Headers.Add("Authorization", header);
            
            
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8); 
            String json = reader.ReadToEnd();
            
            JavaScriptSerializer js = new JavaScriptSerializer(); 
            dynamic dob = js.Deserialize<dynamic>(json);
            dynamic docs = dob["documents"]; 
            
            object[] buf = docs; 
            int length = buf.Length;
            for (int i = 0; i < length; i++)
            { 
                string lname = docs[i]["place_name"];
                double x = double.Parse(docs[i]["x"]);
                double y = double.Parse(docs[i]["y"]);
                mls.Add(new MyLocale(lname, y, x));
            } 
            return mls; 
        }

       
    }
}
