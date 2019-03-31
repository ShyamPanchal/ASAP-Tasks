using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsapTasks.Data
{
    public class Issue
    {
        string id;
        string name;
        string description;
        bool completionStatus;
        string enrollmentId;
        string developerId;
        string projectId;

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

        [JsonProperty(PropertyName = "CompletionStatus")]
        public bool CompletionStatus
        {
            get { return completionStatus; }
            set { completionStatus = value; }
        }

        [JsonProperty(PropertyName = "Description")]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        [JsonProperty(PropertyName = "EnrollmentId")]
        public string EnrollmentId
        {
            get { return enrollmentId; }
            set { enrollmentId = value; }
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
