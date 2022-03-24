using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace InvictaPartnersAPI.Services
{
    public static class HttpServices
    {
        public static async Task<string> SendURI(Uri u, HttpContent c, string token)
        {

            var response = string.Empty;
            using (var client = new HttpClient())            
            {
                Console.WriteLine("Token:"+token);
                Console.WriteLine("Uri:"+u.ToString());
                Console.WriteLine("Content:"+c.ToString());
                if(!String.IsNullOrEmpty(token)){  //use the token as authorization if provided
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    Console.WriteLine("autorization added to client");
                }
                HttpResponseMessage result = await client.PostAsync(u, c);
                Console.WriteLine("Result:"+result.Headers.ToString());
                Console.WriteLine("Response Code:"+result.StatusCode);
                if (result.IsSuccessStatusCode)
                {
                    response = await result.Content.ReadAsStringAsync();
                
                }
            }
            return response;
        }

        public static async Task<string> SendURI(Uri u, HttpContent c)
        {

            var response = string.Empty;
            using (var client = new HttpClient())            
            {
                Console.WriteLine("Uri:"+u.ToString());
                Console.WriteLine("Content:"+c.ToString());
                Console.WriteLine("autorization added to client");
                HttpResponseMessage result = await client.PostAsync(u, c);
                Console.WriteLine("Result:"+result.Headers.ToString());
                Console.WriteLine("Response Code:"+result.StatusCode);
                if (result.IsSuccessStatusCode)
                {
                    response = await result.Content.ReadAsStringAsync();
                
                }
            }
            return response;
        }
    }
}