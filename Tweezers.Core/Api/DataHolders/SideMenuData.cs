using System;
using System.Collections.Generic;
using System.Text;

namespace Tweezers.Api.DataHolders
{
    public class SideMenuData
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string ReferenceLink { get; set; }
        public string IconName { get; set; }

        public Type TweezersType { get; set; }
    }
}
