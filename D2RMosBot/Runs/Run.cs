using MapAssist.D2Assist.Builds;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapAssist.D2Assist.Runs
{
    public abstract class Run
    {
        protected static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public abstract void Execute(Build build);
        public abstract string GetName();
    }
}
