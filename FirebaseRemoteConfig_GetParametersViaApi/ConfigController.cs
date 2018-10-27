using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FirebaseRemoteConfig_GetParametersViaApi
{
    [Route("api/[controller]")]
    public class ConfigController : Controller
    {
        //STEP 1- TOKEN 
        //first of all, you have to download config file related your remote config project from google console. (for example remoteconfig.json)
        public async Task<string> GetToken()
        {
            GoogleCredential credential;
            using (var stream = new System.IO.FileStream("remoteconfig.json", System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(
                    new string[] {
                                    "https://www.googleapis.com/auth/firebase.database",
                                    "https://www.googleapis.com/auth/userinfo.email",
                                    "https://www.googleapis.com/auth/firebase",
                                    "https://www.googleapis.com/auth/firebase.remoteconfig",
                                    "https://www.googleapis.com/auth/firebase.messaging",
                                    "https://www.googleapis.com/auth/identitytoolkit"
                    });
            }

            ITokenAccess c = credential as ITokenAccess;
            return await c.GetAccessTokenForRequestAsync();
        }

        //STEP 2 
        //USE TOKEN AND GET REMOTE CONFIG PARAMETERS
        //write your firebase remote config project name into GetAsync request uri.
        public async Task<string> GetConfig(string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/gzip"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var parameters = await client.GetAsync("https://firebaseremoteconfig.googleapis.com/v1/projects/WRITE-YOUR-PROJECT-NAME/remoteConfig");
                var jsonData = parameters.Content.ReadAsStringAsync();
                return await jsonData;
            }
        }


        //STEP 3
        //TEST
        [HttpGet]
        public async Task<string> Get()
        {
            var token = GetToken().Result;
            var config = GetConfig(token).Result;
            return config;
        }
    }
}
