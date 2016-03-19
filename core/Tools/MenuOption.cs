using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackCS.Core.Tools
{
    public class MenuOption
    {
        public string Command { get; set; }
        public string Description { get; set; }
        public Action<string[]> Method { get; set; }

        public MenuOption(string command, Action<string[]> method)
        {
            Command = command;
            Method = method;
        }
    }
}
