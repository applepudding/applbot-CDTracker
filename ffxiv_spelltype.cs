using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace applbot_CDTracker
{
    public class ffxiv_spelltype
    {
        private int _varCount;
        private int _varType;
        public ffxiv_spelltype(int type)
        {
            _varCount = 0;
            _varType = type;
        }
        public int varCount
        {
            get { return _varCount; }
            set { _varCount = value; }
        }
        public int varType
        {
            get { return _varType; }
            set { _varType = value; }
        }
        public void increaseCount()
        {
            _varCount = _varCount + 1;
        }
    }
}
