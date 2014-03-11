using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaseManagement.DataItems
{
    public class CloudFileUrlItem
    {
        public CloudFileUrlItem(string name, string url)
        {
            Url = url;
            Name = name;
        }

        public string Url { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
