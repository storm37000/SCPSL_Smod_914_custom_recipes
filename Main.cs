using Smod2;
using Smod2.Attributes;
using System.Collections.Generic;
using UnityEngine.Networking;
using MEC;
using System.Linq;

namespace SCP914CustomRecipes
{
	[PluginDetails(
		author = "storm37000",
		name = "Custom SCP 914 Item Recipies",
		description = "Custom SCP-914 Item Recipies",
		id = "s37k.custom914recip",
		version = "1.0.6",
		SmodMajor = 3,
		SmodMinor = 2,
		SmodRevision = 0
		)]
	class Main : Plugin
	{
		public override void OnDisable()
		{
			this.Info(this.Details.name + " has been disabled.");
		}
		public override void OnEnable()
		{
			this.Info(this.Details.name + " has been enabled.");
		}

		public bool UpToDate { get; private set; } = true;

		public void outdatedmsg()
		{
			this.Error("Your version is out of date, please update the plugin and restart your server when it is convenient for you.");
		}

		IEnumerator<float> UpdateChecker()
		{
			string[] hosts = { "https://storm37k.com/addons/", "http://74.91.115.126/addons/" };
			bool fail = true;
			string errorMSG = "";
			foreach (string host in hosts)
			{
				using (UnityWebRequest webRequest = UnityWebRequest.Get(host + this.Details.name + ".ver"))
				{
					// Request and wait for the desired page.
					yield return Timing.WaitUntilDone(webRequest.SendWebRequest());

					if (webRequest.isNetworkError || webRequest.isHttpError)
					{
						errorMSG = webRequest.error;
					}
					else
					{
						if (webRequest.downloadHandler.text != this.Details.version)
						{
							outdatedmsg();
							UpToDate = false;
						}
						fail = false;
						break;
					}
				}
			}
			if (fail)
			{
				this.Error("Could not fetch latest version txt: " + errorMSG);
			}
		}

		public override void Register()
		{
			this.AddEventHandlers(new EventHandler(this));

			string confdir = Smod2.ConfigManager.Manager.Config.GetConfigPath();
			int index = confdir.LastIndexOf("/");
			if (index > 0)
			{
				confdir = confdir.Substring(0, index); // or index + 1 to keep slash
			}
			string file = System.IO.Directory.GetFiles(confdir, "s37k_g_disableVcheck*", System.IO.SearchOption.TopDirectoryOnly).FirstOrDefault();
			if (file == null)
			{
				Timing.RunCoroutine(UpdateChecker());
			}
			else
			{
				this.Info("Version checker is disabled.");
			}
		}
	}
}
