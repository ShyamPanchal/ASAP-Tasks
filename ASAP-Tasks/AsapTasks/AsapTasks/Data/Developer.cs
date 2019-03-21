using System;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace AsapTasks.Data
{
    public class Developer
    {
        string id;
        string name;
        bool isVerified;
        string password;
        string email;

        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        [JsonProperty(PropertyName = "Name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [JsonProperty(PropertyName = "IsVerified")]
        public bool IsVerified
        {
            get { return isVerified; }
            set { isVerified = value; }
        }

        [JsonProperty(PropertyName = "Password")]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        [JsonProperty(PropertyName = "Email")]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        [Version]
        public string Version { get; set; }
    }
}
