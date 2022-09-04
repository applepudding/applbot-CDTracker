using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Advanced_Combat_Tracker;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace applbot_CDTracker
{
    public partial class cdtracker : UserControl, IActPluginV1
    {
        public cdtracker()
        {
            InitializeComponent();
        }
		Label lblStatus;    // The status label that appears in ACT's Plugin tab
		List<ffxiv_spell> skillTemplates_offensive = new List<ffxiv_spell>();
		List<ffxiv_spell> skillTemplates_defensive = new List<ffxiv_spell>();
        List<ffxiv_spell> skillTemplates_misc = new List<ffxiv_spell>();
        public gui cdTrackerForm;
		private static string pluginPath = Directory.GetCurrentDirectory();
		private dynamic appConfig;
		private static string skillListFile = pluginPath + "\\applbot-CDTracker\\config.json";
		private static string skillImgFolder = pluginPath + "\\applbot-CDTracker\\imgs\\";

		public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
		{
			lblStatus = pluginStatusText;   // Hand the status label's reference to our local var
			pluginScreenSpace.Controls.Add(this);   // Add this UserControl to the tab ACT provides
			this.Dock = DockStyle.Fill; // Expand the UserControl to fill the tab's client space

			ActGlobals.oFormActMain.OnCombatStart += new CombatToggleEventDelegate(oFormActMain_OnCombatStart);
			//ActGlobals.oFormActMain.OnCombatEnd += new CombatToggleEventDelegate(oFormActMain_OnCombatEnd);

			lblStatus.Text = "Plugin Started";

			//init UI
			cdTrackerForm = new gui();
			cdTrackerForm.Show();
			this.loadConfigJson();
		}
		public void DeInitPlugin()
		{
			try
			{
				ActGlobals.oFormActMain.OnCombatStart -= oFormActMain_OnCombatStart;
				//ActGlobals.oFormActMain.OnCombatEnd -= oFormActMain_OnCombatEnd;
				if (this.appConfig.rawmode_autoreset > 0)
				{
					ActGlobals.oFormActMain.OnLogLineRead -= oFormActMain_OnLogLineRead;
				}
                else
                {
					//ActGlobals.oFormActMain.AfterCombatAction -= oFormActMain_AfterCombatAction; 
					ActGlobals.oFormActMain.BeforeCombatAction -= oFormActMain_AfterCombatAction;
				}

				//save CD list
				appConfig.loc_x = this.cdTrackerForm.DesktopLocation.X;
				appConfig.loc_y = this.cdTrackerForm.DesktopLocation.Y;
				appConfig.rawmode_autoreset = this.cdTrackerForm.autoreset;
                appConfig.pic_size = this.cdTrackerForm.picSize;
                appConfig.pic_spacing = this.cdTrackerForm.picSpacing;

                using (StreamWriter file = File.CreateText(skillListFile))
				{
					JsonSerializer serializer = new JsonSerializer();
					serializer.Serialize(file, appConfig);
				}

				this.skillTemplates_offensive.Clear();
				this.skillTemplates_defensive.Clear();
				this.skillTemplates_misc.Clear();
				this.cdTrackerForm.Close();
			}
			catch (Exception ex)
			{
				lblStatus.Text = ex.Message;
			}


			lblStatus.Text = "Plugin Exited";
		}

		void oFormActMain_OnCombatStart(bool isImport, CombatToggleEventArgs encounterInfo)
		{
			this.cdTrackerForm.resetForm();
		}

		void oFormActMain_AfterCombatAction(bool isImport, CombatActionEventArgs actionInfo)
		{
			//lblStatus.Text = actionInfo.combatAction.ToString(); //the whole log line
			string skillName = actionInfo.combatAction.AttackType;
			string caster = actionInfo.combatAction.Attacker;
			this.checkSkillToTrack(caster, skillName);
		}

		void oFormActMain_OnLogLineRead(bool isImport, LogLineEventArgs logInfo)
		{
			if (logInfo.logLine.Length < 300)
            {
				string tempLine = logInfo.logLine;
				tempLine = tempLine.Replace("uses", "use");
				//tempLine = tempLine.Replace(".", "");
				foreach (ffxiv_spell spell in this.skillTemplates_offensive)
                {
					string tempString = " use " + spell.varName + ".";
					if (tempLine.Contains(tempString))
					{
						string[] tempExplode = Regex.Split(tempLine, ":");
						tempLine = tempExplode[tempExplode.Length - 1];
						tempExplode = Regex.Split(tempLine, tempString);
						string caster = tempExplode[0];
						string skillName = spell.varName;
						this.checkSkillToTrack(caster, skillName);
					}
				}
				foreach (ffxiv_spell spell in this.skillTemplates_defensive)
				{
					string tempString = " use " + spell.varName + ".";
					if (tempLine.Contains(tempString))
					{
						string[] tempExplode = Regex.Split(tempLine, ":");
						tempLine = tempExplode[tempExplode.Length - 1];
						tempExplode = Regex.Split(tempLine, tempString);
						string caster = tempExplode[0];
						string skillName = spell.varName;
						this.checkSkillToTrack(caster, skillName);
					}
				}
                foreach (ffxiv_spell spell in this.skillTemplates_misc)
                {
                    string tempString = " use " + spell.varName + ".";
                    if (tempLine.Contains(tempString))
                    {
                        string[] tempExplode = Regex.Split(tempLine, ":");
                        tempLine = tempExplode[tempExplode.Length - 1];
                        tempExplode = Regex.Split(tempLine, tempString);
                        string caster = tempExplode[0];
                        string skillName = spell.varName;
                        this.checkSkillToTrack(caster, skillName);
                    }
                }

            }

		}

		public void checkSkillToTrack(string caster, string skillName)
		{
			foreach (ffxiv_spell spell in this.skillTemplates_offensive)
			{
				if (skillName == spell.varName && caster != "Unknown" && caster != "Carbuncle")
				{
					this.cdTrackerForm.useSpell(caster, spell, true);
					break;
				}
			}
			foreach (ffxiv_spell spell in this.skillTemplates_defensive)
			{
				if (skillName == spell.varName && caster != "Unknown" && caster != "Carbuncle")
				{
					this.cdTrackerForm.useSpell(caster, spell, true);
					break;
				}
			}
            foreach (ffxiv_spell spell in this.skillTemplates_misc)
            {
                if (skillName == spell.varName && caster != "Unknown" && caster != "Carbuncle")
                {
                    this.cdTrackerForm.useSpell(caster, spell, true);
                    break;
                }
            }
        }
		public void loadConfigJson()
		{
			this.appConfig = Array.Empty<string>();
			try
			{
				using (StreamReader r = new StreamReader(skillListFile))
				{
					string json = r.ReadToEnd();
					this.appConfig = JsonConvert.DeserializeObject(json);
					this.skillTemplates_offensive.Clear(); //clean skill templates
					this.skillTemplates_defensive.Clear();
                    this.skillTemplates_misc.Clear();
                    int tempX = this.appConfig.loc_x;
					int tempY = this.appConfig.loc_y;
					int tempPicSize = this.appConfig.pic_size;
					int tempPicMargin = this.appConfig.pic_spacing;
					int tempAutoreset = this.appConfig.rawmode_autoreset;
					cdTrackerForm.autoreset = tempAutoreset;
					cdTrackerForm.SetDesktopLocation(tempX, tempY);
                    cdTrackerForm.picSize = tempPicSize;


                    this.lblSize.Text = tempPicSize.ToString();
					this.trackBar1.Value = tempPicSize;

                    foreach (var item in this.appConfig.skills_offensive)
					{
						String tempName = item.name;
						String skillInfo = item.notes;
						int tempDuration = item.duration;
						int tempCd = item.cd;

						FileStream tempStream = new FileStream(skillImgFolder + item.img, FileMode.Open, FileAccess.Read);
						Image tempImg = Image.FromStream(tempStream);
						tempStream.Close();
						//Image tempImg = Image.FromFile(this.pluginPath + "\\applbotv2\\imgs\\" + item.img);
						this.skillTemplates_offensive.Add(new ffxiv_spell(tempName, tempDuration, tempCd, tempImg, 0, skillInfo));
					}

					foreach (var item in this.appConfig.skills_defensive)
					{
						String tempName = item.name;
						String skillInfo = item.notes;
						int tempDuration = item.duration;
						int tempCd = item.cd;

						FileStream tempStream = new FileStream(skillImgFolder + item.img, FileMode.Open, FileAccess.Read);
						Image tempImg = Image.FromStream(tempStream);
						tempStream.Close();
						//Image tempImg = Image.FromFile(this.pluginPath + "\\applbotv2\\imgs\\" + item.img);
						this.skillTemplates_defensive.Add(new ffxiv_spell(tempName, tempDuration, tempCd, tempImg, 1, skillInfo));
					}

                    foreach (var item in this.appConfig.skills_misc)
                    {
                        String tempName = item.name;
                        String skillInfo = item.notes;
                        int tempDuration = item.duration;
                        int tempCd = item.cd;

                        FileStream tempStream = new FileStream(skillImgFolder + item.img, FileMode.Open, FileAccess.Read);
                        Image tempImg = Image.FromStream(tempStream);
                        tempStream.Close();
                        //Image tempImg = Image.FromFile(this.pluginPath + "\\applbotv2\\imgs\\" + item.img);
                        this.skillTemplates_misc.Add(new ffxiv_spell(tempName, tempDuration, tempCd, tempImg, 2, skillInfo));
                    }

                    if (this.appConfig.rawmode_autoreset > 0)
					{
						ActGlobals.oFormActMain.OnLogLineRead += new LogLineEventDelegate(oFormActMain_OnLogLineRead);
					}
					else
					{
						//ActGlobals.oFormActMain.AfterCombatAction += new CombatActionDelegate(oFormActMain_AfterCombatAction); 
						ActGlobals.oFormActMain.BeforeCombatAction += new CombatActionDelegate(oFormActMain_AfterCombatAction);
					}

				}
			}
			catch (Exception ex)
			{
				lblStatus.Text = ex.Message;
			}
		}

        private void btnResetPos_Click(object sender, EventArgs e)
        {
			cdTrackerForm.SetDesktopLocation(100, 100);
		}

		private void trackBar1_Scroll(object sender, EventArgs e)
		{
			this.lblSize.Text = this.trackBar1.Value.ToString();
			this.cdTrackerForm.picSize = this.trackBar1.Value;
            this.cdTrackerForm.resetForm();

        }
	}
}
