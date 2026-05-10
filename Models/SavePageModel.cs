using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScrollBase.Services;

namespace ScrollBase.Models
{
    public class SavedPageModel
    {
        // These match the keys in your Firebase Realtime Database
        public string PageName { get; set; }
        public string PageLink { get; set; }

        // Computed property for the WebView binding. 
        // Ensures the URL has "https://" so the WebView doesn't fail to load.
        public string Url => LinkCheck(PageLink);

        // Matches your requested UI binding
        public double HeightRequest { get; set; } = 400;

        public static string LinkCheck(string link)
        {
            string Url = string.IsNullOrEmpty(link) ? string.Empty :
                             link.StartsWith("http") ? link : $"https://{link}";
            return Url;
        }
    }
}