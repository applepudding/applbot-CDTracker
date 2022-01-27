using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace applbot_CDTracker
{
    public class ffxiv_spell
    {
        private string _varName;
        private string _varInfo;
        private int _varCooldown;
        private int _varDuration;
        private int _varType;
        private Image _varImg;
        public ffxiv_spell(string name, int duration, int cooldown, Image img, int type, string info)
        {
            _varName = name;
            _varCooldown = cooldown;
            _varDuration = duration;
            _varImg = img;
            _varType = type;
            _varInfo = info;
        }
        public string varName
        {
            get { return _varName; }
            set { _varName = value; }
        }
        public string varInfo
        {
            get { return _varInfo; }
            set { _varInfo = value; }
        }
        public int varCooldown
        {
            get { return _varCooldown; }
            set { _varCooldown = value; }
        }
        public int varDuration
        {
            get { return _varDuration; }
            set { _varDuration = value; }
        }
        public int varType
        {
            get { return _varType; }
            set { _varType = value; }
        }
        public Image varImg
        {
            get { return _varImg; }
            set { _varImg = value; }
        }
    }
}
