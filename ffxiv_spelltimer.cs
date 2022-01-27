using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Drawing;

namespace applbot_CDTracker
{
    public class ffxiv_spelltimer
    {
        private string _varCaster;
        private int _varPicSize;
        private ffxiv_spell _varSpell;
        private PictureBox _varPictureBox;
        private ToolTip _varToolTip;
        private int _varTimerValue;
        private long _varTimestamp;
        private SolidBrush _varCurrentBrush;

        private bool _varDisplayName;

        private static SolidBrush brush_default = new SolidBrush(Color.FromArgb(0, 0, 0, 0));
        private static SolidBrush brush_inactive = new SolidBrush(Color.FromArgb(200, 0, 0, 0));
        private static SolidBrush brush_active = new SolidBrush(Color.FromArgb(128, 0, 255, 0));



        public ffxiv_spelltimer(string caster, ffxiv_spell spell, int picSize)
        {
            _varCaster = caster;
            _varSpell = spell;
            _varCurrentBrush = brush_default;
            _varTimerValue = _varSpell.varCooldown;
            _varDisplayName = false;
            _varTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            _varPicSize = picSize;
        }
        public void start(bool displaycastername)
        {
            _varTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            _varTimerValue = _varSpell.varCooldown;
            _varDisplayName = displaycastername;
        }
        public void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            string text = "Ready";
            float fontSize = 18;

            if (_varTimerValue > 1)
            {
                if (_varTimerValue > _varSpell.varCooldown - _varSpell.varDuration)
                {
                    text = (_varTimerValue + _varSpell.varDuration - _varSpell.varCooldown).ToString();
                    _varCurrentBrush = brush_active;
                }
                else
                {
                    text = _varTimerValue.ToString();
                    _varCurrentBrush = brush_inactive;
                }

            }
            else
            {
                _varCurrentBrush = brush_default;
            }
            Pen p = new Pen(Brushes.Black, 2);
            p.LineJoin = System.Drawing.Drawing2D.LineJoin.Round; //prevent "spikes" at the path
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            Rectangle r = new Rectangle(0, 0, _varPicSize, _varPicSize);
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            Font f = new Font("Microsoft Sans Serif", fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            gp.AddString(text, f.FontFamily, (int)f.Style, fontSize, r, sf);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            e.Graphics.FillRectangle(_varCurrentBrush, r);
            e.Graphics.DrawPath(p, gp);
            e.Graphics.FillPath(Brushes.White, gp);

            //add name to it
            if (_varDisplayName)
            {
                sf.LineAlignment = StringAlignment.Far;
                gp = new System.Drawing.Drawing2D.GraphicsPath();
                f = new Font("Microsoft Sans Serif", fontSize - 6, FontStyle.Bold, GraphicsUnit.Pixel);
                gp.AddString(_varCaster.Substring(0, Math.Min(_varCaster.Length, 5)), f.FontFamily, (int)f.Style, fontSize - 6, r, sf);
                e.Graphics.DrawPath(p, gp);
                e.Graphics.FillPath(Brushes.White, gp);
            }
            /////
            gp.Dispose();
            f.Dispose();
            sf.Dispose();
        }
        public string varCaster
        {
            get { return _varCaster; }
            set { _varCaster = value; }
        }
        public ffxiv_spell varSpell
        {
            get { return _varSpell; }
            set { _varSpell = value; }
        }
        public PictureBox varPictureBox
        {
            get { return _varPictureBox; }
            set { _varPictureBox = value; }
        }
        public ToolTip varToolTip
        {
            get { return _varToolTip; }
            set { _varToolTip = value; }
        }
        public int varTimerValue
        {
            get { return _varTimerValue; }
            set { _varTimerValue = value; }
        }
        public bool varDisplayName
        {
            get { return _varDisplayName; }
            set { _varDisplayName = value; }
        }
        public long varTimestamp
        {
            get { return _varTimestamp; }
            set { _varTimestamp = value; }
        }
        public int varPicSize
        {
            get { return _varPicSize; }
            set { _varPicSize = value; }
        }
    }
}
