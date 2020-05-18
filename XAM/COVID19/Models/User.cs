using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Perimeter.Models
{
    public class User
    {
        public User()
        {
            TokenExpiration = DateTime.MinValue;
            //Id = Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "");

            Id = CreateIdentifer();

            Token = "space";
        }

        public string Id { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonIgnore]
        public string Name {
            get
            {
                return string.Format("{0} {1}",this.FirstName,this.LastName);
            }
        }

        [JsonProperty("tagline")]
        public string Tagline { get; set; }

        [JsonProperty("userprofilesummary")]
        public string UserProfileSummary { get; set; }

        [JsonProperty("profileImage")]
        public string ProfileImage { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }
        [JsonProperty("isInfected")]
        public bool IsInfected { get; set; }

        [JsonProperty("blogViews")]
        public int BlogViews { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("tokenExpiration")]
        public DateTime TokenExpiration { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [JsonProperty("woeid")]
        public string WOEID { get; set; }

        [JsonProperty("joined")]
        public DateTime Joined { get; set; }

        [JsonProperty("lastdatasync")]
        public DateTime LastDataSync { get; set; }

        [JsonProperty("keepLoggedIn")]
        public bool KeepLoggedIn { get; set; }

        [JsonProperty("upVotes")]
        public int UpVotes { get; set; }
        [JsonProperty("downVotes")]
        public int DownVotes { get; set; }
        [JsonProperty("followers")]
        public int Followers { get; set; }


        [JsonIgnore]
        public bool SendMoment { get; set; }

        string CreateIdentifer()
        {

            StringBuilder builder = new StringBuilder();
            
            Enumerable
               .Range(65, 26)
                .Select(e => ((char)e).ToString())
                .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                .OrderBy(e => Guid.NewGuid())
                .Take(11)
                .ToList().ForEach(e => builder.Append(e));
            
            return builder.ToString();
        }
    }
}

