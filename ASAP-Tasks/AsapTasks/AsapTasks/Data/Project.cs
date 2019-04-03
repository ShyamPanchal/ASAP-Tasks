using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsapTasks.Data
{
    public class Project
    {
        #region Private Variables

        string id;
        string name;
        bool openStatus;
        string startDate;
        string description;

        #endregion

        #region Json Properties

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

        [JsonProperty(PropertyName = "OpenStatus")]
        public bool OpenStatus
        {
            get { return openStatus; }
            set { openStatus = value; }
        }

        [JsonProperty(PropertyName = "StartDate")]
        public string StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }

        [JsonProperty(PropertyName = "Description")]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        #endregion

        [Version]
        public string Version { get; set; }
    }
}
