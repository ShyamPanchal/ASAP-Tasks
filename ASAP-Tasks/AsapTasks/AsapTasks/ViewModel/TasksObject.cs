using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AsapTasks.ViewModel
{
    public class TasksObject
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string CompletionStatus { get; set; }

        public Color Color { get; set; }
    }
}
