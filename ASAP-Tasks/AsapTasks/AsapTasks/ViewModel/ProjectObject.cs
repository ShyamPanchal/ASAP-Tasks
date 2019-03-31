using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AsapTasks.ViewModel
{
    public class ProjectObject
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool CompletionStatus { get; set; }

        public bool AcceptStatus { get; set; }

        public Color Color { get; set; }
    }
}
