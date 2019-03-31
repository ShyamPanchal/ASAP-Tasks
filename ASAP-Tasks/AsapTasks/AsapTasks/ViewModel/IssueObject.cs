using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AsapTasks.ViewModel
{
    class IssueObject
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool CompletionStatus { get; set; }

        public Color BackgroundColor { get; set; }

        public Color TextColor { get; set; }
    }
}
