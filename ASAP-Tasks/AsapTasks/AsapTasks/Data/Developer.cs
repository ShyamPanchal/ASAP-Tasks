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
        string phoneNumber;

        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [JsonProperty(PropertyName = "is_verified")]
        public bool IsVerified
        {
            get { return isVerified; }
            set { isVerified = value; }
        }

        [JsonProperty(PropertyName = "password")]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        [JsonProperty(PropertyName = "phoneNumber")]
        public string PhoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value; }
        }

        [Version]
        public string Version { get; set; }
    }
}
