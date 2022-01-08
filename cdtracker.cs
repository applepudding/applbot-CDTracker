﻿using System;
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
					ActGlobals.oFormActMain.AfterCombatAction -= oFormActMain_AfterCombatAction;
				}

				//save CD list
				appConfig.loc_x = this.cdTrackerForm.DesktopLocation.X;
				appConfig.loc_y = this.cdTrackerForm.DesktopLocation.Y;
				appConfig.rawmode_autoreset = this.cdTrackerForm.autoreset;
				using (StreamWriter file = File.CreateText(skillListFile))
				{
					JsonSerializer serializer = new JsonSerializer();
					serializer.Serialize(file, appConfig);
				}

				this.skillTemplates_offensive.Clear();
				this.skillTemplates_defensive.Clear();
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
			//lblStatus.Text = logInfo.logLine;
			string tempLine = logInfo.logLine;
			if (tempLine.Contains("use") || tempLine.Contains("uses"))
            {
				tempLine = tempLine.Replace("uses", "use");
				tempLine = tempLine.Replace(".", "");
				string[] tempExplode = Regex.Split(tempLine, ":");
				tempLine = tempExplode[tempExplode.Length-1];
				tempExplode = Regex.Split(tempLine, " use ");
				string caster = tempExplode[0];
				string skillName = tempExplode[1];
				this.checkSkillToTrack(caster, skillName);
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
					int tempX = this.appConfig.loc_x;
					int tempY = this.appConfig.loc_y;
					int tempPicSize = this.appConfig.pic_size;
					int tempPicMargin = this.appConfig.pic_spacing;
					int tempAutoreset = this.appConfig.rawmode_autoreset;
					cdTrackerForm.autoreset = tempAutoreset;
					cdTrackerForm.SetDesktopLocation(tempX, tempY);

					foreach (var item in this.appConfig.skills_offensive)
					{
						String tempName = item.name;
						int tempDuration = item.duration;
						int tempCd = item.cd;

						FileStream tempStream = new FileStream(skillImgFolder + item.img, FileMode.Open, FileAccess.Read);
						Image tempImg = Image.FromStream(tempStream);
						tempStream.Close();
						//Image tempImg = Image.FromFile(this.pluginPath + "\\applbotv2\\imgs\\" + item.img);
						this.skillTemplates_offensive.Add(new ffxiv_spell(tempName, tempDuration, tempCd, tempImg, 0));
					}

					foreach (var item in this.appConfig.skills_defensive)
					{
						String tempName = item.name;
						int tempDuration = item.duration;
						int tempCd = item.cd;

						FileStream tempStream = new FileStream(skillImgFolder + item.img, FileMode.Open, FileAccess.Read);
						Image tempImg = Image.FromStream(tempStream);
						tempStream.Close();
						//Image tempImg = Image.FromFile(this.pluginPath + "\\applbotv2\\imgs\\" + item.img);
						this.skillTemplates_defensive.Add(new ffxiv_spell(tempName, tempDuration, tempCd, tempImg, 1));
					}

					if (this.appConfig.rawmode_autoreset > 0)
					{
						ActGlobals.oFormActMain.OnLogLineRead += new LogLineEventDelegate(oFormActMain_OnLogLineRead);
					}
					else
					{
						ActGlobals.oFormActMain.AfterCombatAction += new CombatActionDelegate(oFormActMain_AfterCombatAction);
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
    }
}