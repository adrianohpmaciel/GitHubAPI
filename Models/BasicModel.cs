using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GitHubAPI.Models
{
    public class UserData
    {
        [JsonProperty("login")]
        public string Login { get; set; }
        [JsonProperty("avatar_url")]
        public string Avatar_url { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("company")]
        public string Company { get; set; }
        [JsonProperty("public_Repos")]
        public int Public_Repos { get; set; }
        [JsonProperty("followers")]
        public int Followers { get; set; }
        [JsonProperty("folllowing")]
        public int Folllowing { get; set; }
        [JsonProperty("created_At")]
        public DateTime Created_At { get; set; }
        [JsonProperty("updated_At")]
        public DateTime Updated_At { get; set; }
    }

    public class RepositoryData
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("owner")]
        public UserData Owner { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("created_At")]
        public DateTime Created_At { get; set; }
        [JsonProperty("updated_At")]
        public DateTime Updated_At { get; set; }
        [JsonProperty("contributors_url")]
        public string Contributors_url { get; set; }
    }

    public class RepositorySearchData
    {
        [JsonProperty("total_count")]
        public int Total_count { get; set; }
        [JsonProperty("incomplete_results")]
        public bool Incomplete_results;
        [JsonProperty("items")]
        public RepositoryData[] Items { get; set; }
        public int QttPages { get; set; }

        public void GetQttPages(int per_Page)
        {
            double QttPag = this.Total_count != 0 ? this.Total_count / per_Page : 0;
            this.QttPages = Convert.ToInt16(Math.Round(QttPag + (Total_count % per_Page != 0 ? 0.51 : 0), 0));
        }
    }

}
