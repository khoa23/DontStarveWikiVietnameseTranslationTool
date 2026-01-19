using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DonStarveWikiTranslator.Modules
{
    public class Translator
    {
        private static readonly HttpClient http = new HttpClient();

        public static async Task<string> TranslateAsync(string text)
        {
            string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=en&tl=vi&dt=t&q={Uri.EscapeDataString(text)}";
            var res = await http.GetStringAsync(url);
            var arr = JArray.Parse(res);
            string result = string.Join("", arr[0].Select(t => t[0].ToString()));
            return result;
        }
    }
}
