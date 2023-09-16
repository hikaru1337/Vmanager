using Newtonsoft.Json;
using System.Net;

namespace VManager.AsposePSD
{
    public class GetFollowers
    {
        public static followers Followers(ulong UserId)
        {
            var Followers = new followers();
            var url = $"https://api.twitch.tv/helix/channels/followers?broadcaster_id={UserId}";

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);

            httpRequest.Accept = "application/json";
            httpRequest.Headers["Authorization"] = "Bearer Token";
            httpRequest.Headers["Client-Id"] = "Token";


            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                Followers = JsonConvert.DeserializeObject<followers>(result);
            }
            return Followers;
        }
    }
}
