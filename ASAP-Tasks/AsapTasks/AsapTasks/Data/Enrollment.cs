using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsapTasks.Data
{
    public class Enrollment
    {
        string id;
        bool acceptStatus;

        string developerId;

        string projectId;

        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        [JsonProperty(PropertyName = "AcceptStatus")]
        public bool AcceptStatus
        {
            get { return acceptStatus; }
            set { acceptStatus = value; }
        }

        [JsonProperty(PropertyName = "DeveloperId")]
        public string DeveloperId
        {
            get { return developerId; }
            set { developerId = value; }
        }

        [JsonProperty(PropertyName = "ProjectId")]
        public string ProjectId
        {
            get { return projectId; }
            set { projectId = value; }
        }

        [Version]
        public string Version { get; set; }
    }
}
