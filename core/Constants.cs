using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackCS.Core
{
    public class Constants
    {
        public const int DeathTimeLimit = 100;
        public const int BulletSpeedLimit = 10;

        public const int WorldUpdateRefresh = 100;

        public const int ServerToClientBufferSize = 100000;
        public const int ClientToServerBufferSize = 100000;

        public const bool Debug = false;
    }
}
