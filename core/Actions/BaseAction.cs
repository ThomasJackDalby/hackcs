using HackCS.Core.Data.Actors;
using HackCS.Core.Data.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackCS.Core.Actions
{
    public abstract class BaseAction
    {
        public abstract void Process(Player actor, Game game);
    }
}
