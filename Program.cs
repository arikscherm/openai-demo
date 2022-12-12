using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            do
            {
                Console.WriteLine("Ask a question");
                var question = Console.ReadLine();
                var answer = callOpenAI(150, question, "text-davinci-002", 0.7,1,0,0);
                Console.WriteLine(answer);
                Console.WriteLine("\nPress Enter to Continue or 'quit' to Quit");
            } while(Console.ReadLine() != "quit");
            
        }



        private static string callOpenAI(int maxTokens, string prompt, string model,
          double temperature, int topP, int frequencyPenalty, int presencePenalty)
        {
            var openaiApiKey = System.Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            var url = "https://api.openai.com/v1/engines/" + model + "/completions";
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), url))
                    {
                        //Add Bearer token to header collection
                        request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + openaiApiKey);
                        
                        //Add request body
                        request.Content = new StringContent("{\n  \"prompt\": \"" + prompt + "\",\n  \"temperature\": " +
                            temperature.ToString(CultureInfo.InvariantCulture) + ",\n  \"max_tokens\": " + maxTokens + ",\n  \"top_p\": " + topP +
                            ",\n  \"frequency_penalty\": " + frequencyPenalty + ",\n  \"presence_penalty\": " + presencePenalty + "\n}");
                       
                        //Add content type
                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                        
                        var response = httpClient.SendAsync(request).Result;
                        //Serialize HTTP response to string
                        var json = response.Content.ReadAsStringAsync().Result;
                        //Console.WriteLine(response);
                        
                        //Convert to Json object
                        dynamic dynObj = JsonConvert.DeserializeObject(json);
                        if (dynObj != null)
                        {
                            return dynObj.choices[0].text.ToString();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }

    }
}