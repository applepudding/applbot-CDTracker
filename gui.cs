using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace applbot_CDTracker
{
    public partial class gui : Form
    {
        public gui()
        {
            InitializeComponent();
        }

        List<ffxiv_spelltimer> ffxiv_spelltimerList = new List<ffxiv_spelltimer>();
        List<ffxiv_spelltype> ffxiv_spelltypeList = new List<ffxiv_spelltype>();
        private bool mouseDown;
        public Point lastLocation;
        public int autoreset = 0;
        public int picSize = 75;
        public int picMargin = 10;

        private void gui_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.Magenta;
            this.TransparencyKey = Color.Magenta;
            this.TopMost = true;
            this.timer1.Interval = 250;
            this.timer1.Enabled = true;
            this.ShowInTaskbar = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            long newestTimestamp = 0;
            foreach (ffxiv_spelltimer spelltimer in ffxiv_spelltimerList)
            {
                newestTimestamp = newestTimestamp < spelltimer.varTimestamp ? spelltimer.varTimestamp : newestTimestamp;    //check for autoreset
                if (spelltimer.varTimerValue > 0)
                {
                    spelltimer.varTimerValue = (int)(spelltimer.varTimestamp + spelltimer.varSpell.varCooldown - DateTimeOffset.Now.ToUnixTimeSeconds());
                }
                spelltimer.varPictureBox.Refresh();
                
            }
            if (newestTimestamp > 0 && autoreset > 0 && DateTimeOffset.Now.ToUnixTimeSeconds() - newestTimestamp > autoreset)
            {
                this.resetForm();
            }
        }

        public void resetForm()
        {
            if (!InvokeRequired)
            {
                foreach (ffxiv_spelltimer spelltimer in ffxiv_spelltimerList)
                {
                    spelltimer.varToolTip.SetToolTip(spelltimer.varPictureBox, null);   //may not be needed
                    this.Controls.Remove(spelltimer.varPictureBox);
                }
                this.Size = new System.Drawing.Size(105, 105);
                ffxiv_spelltimerList.Clear();
                ffxiv_spelltypeList.Clear();
                this.label1.Visible = true;
            }
            else
            {
                Invoke(new Action(resetForm));
            }
        }

        public void useSpell(string caster, ffxiv_spell spell, bool displayCasterName)
        {
            if (!InvokeRequired)
            {
                //check if spell type already exist
                bool typeExist = false;
                foreach (ffxiv_spelltype el in this.ffxiv_spelltypeList)
                {
                    if (el.varType == spell.varType)
                    {
                        typeExist = true;
                        break;
                    }
                }
                if (!typeExist)
                {
                    this.ffxiv_spelltypeList.Add(new ffxiv_spelltype(spell.varType));
                }

                bool alreadyExist = false;
                foreach (ffxiv_spelltimer spelltimer in ffxiv_spelltimerList)
                {
                    if (spelltimer.varCaster == caster && spelltimer.varSpell.varName == spell.varName)
                    {
                        alreadyExist = true;
                        spelltimer.start(displayCasterName);
                        break;
                    }
                }
                if (!alreadyExist)
                {
                    ffxiv_spelltimerList.Add(new ffxiv_spelltimer(caster, spell, picSize));

                    //check if type already exist, increase if it is
                    int maxCountOfType = 0;
                    int maxType = 0;
                    foreach (ffxiv_spelltype el in this.ffxiv_spelltypeList)
                    {
                        if (el.varType == spell.varType)
                        {
                            el.increaseCount();
                        }
                        maxCountOfType = maxCountOfType > el.varCount ? maxCountOfType : el.varCount;
                        maxType = maxType > el.varType ? maxType : el.varType;
                    }

                    //count types
                    int numberOfSameType = 0;
                    foreach (ffxiv_spelltimer spelltimer in ffxiv_spelltimerList)
                    {
                        numberOfSameType = spelltimer.varSpell.varType ==spell.varType ? numberOfSameType+1 : numberOfSameType;
                    }

                    var picture = new PictureBox
                    {
                        Size = new Size(picSize, picSize),
                        Location = new Point(picMargin + (numberOfSameType -1) * (picSize + picMargin), (spell.varType * (picSize+picMargin))),
                        Image = spell.varImg,
                        BorderStyle = BorderStyle.FixedSingle,
                        SizeMode = PictureBoxSizeMode.StretchImage
                    };
                    ffxiv_spelltimerList[ffxiv_spelltimerList.Count - 1].varPictureBox = picture;
                    ffxiv_spelltimerList[ffxiv_spelltimerList.Count - 1].varPictureBox.Paint += new PaintEventHandler(ffxiv_spelltimerList[ffxiv_spelltimerList.Count - 1].pictureBox1_Paint);
                    ffxiv_spelltimerList[ffxiv_spelltimerList.Count - 1].varPictureBox.MouseDown += new MouseEventHandler(label1_MouseDown);
                    ffxiv_spelltimerList[ffxiv_spelltimerList.Count - 1].varPictureBox.MouseMove += new MouseEventHandler(label1_MouseMove);
                    ffxiv_spelltimerList[ffxiv_spelltimerList.Count - 1].varPictureBox.MouseUp += new MouseEventHandler(label1_MouseUp);

                    ffxiv_spelltimerList[ffxiv_spelltimerList.Count - 1].varToolTip = new ToolTip();
                    ffxiv_spelltimerList[ffxiv_spelltimerList.Count - 1].varToolTip.InitialDelay = 10;
                    ffxiv_spelltimerList[ffxiv_spelltimerList.Count - 1].varToolTip.ReshowDelay = 100;
                    ffxiv_spelltimerList[ffxiv_spelltimerList.Count - 1].varToolTip.ShowAlways = true;
                    ffxiv_spelltimerList[ffxiv_spelltimerList.Count - 1].varToolTip.SetToolTip(ffxiv_spelltimerList[ffxiv_spelltimerList.Count - 1].varPictureBox, ffxiv_spelltimerList[ffxiv_spelltimerList.Count - 1].varSpell.varInfo);

                    ffxiv_spelltimerList[ffxiv_spelltimerList.Count - 1].start(displayCasterName);

                    this.label1.Visible = false;
                    this.Controls.Add(ffxiv_spelltimerList[ffxiv_spelltimerList.Count - 1].varPictureBox);

                    this.Width = (maxCountOfType * (picSize + picMargin)) + (picMargin*2);
                    this.Height = ((maxType+1) * (picSize + picMargin)) + (picMargin * 3);
                    //this.Size = new System.Drawing.Size(20 + (ffxiv_spelltypeList[0].varCount) * 85, picSize * 2);
                }
            }
            else
            {
                Invoke(new Action<string, ffxiv_spell, bool>(useSpell), caster, spell, displayCasterName);
            }
        }
        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point((this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);
                this.Update();
            }
        }

        private void label1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }
    }
}
