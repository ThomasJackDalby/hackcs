using HackCS.Core.Data.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackCS.Core.Data.Actors
{
    public abstract class Actor
    {

        public abstract void Process(Game game);
    }
}
