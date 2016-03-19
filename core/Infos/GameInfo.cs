using HackCS.Core;
using HackCS.Core.Infos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackCS.Core
{
    public class GameInfo
    {
        public ExtendedPlayerInfo ExtendedPlayerInfo { get; set; }
        public List<PlayerInfo> Players { get; set; }
        public List<BulletInfo> Bullets { get; set; }
        public List<string> Messages { get; set; }

        public GameInfo()
        {
            Players = new List<PlayerInfo>();
            Bullets = new List<BulletInfo>();
            Messages = new List<string>();
        }

    }
}
