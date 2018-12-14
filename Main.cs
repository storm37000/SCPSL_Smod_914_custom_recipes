using Smod2;
using Smod2.Attributes;
using Smod2.Events;
using Smod2.EventHandlers;

namespace SCP914CustomRecipes
{
    [PluginDetails(
        author = "storm37000",
        name = "Custom SCP 914 Item Recipies",
        description = "Custom SCP-914 Item Recipies",
        id = "s37k.custom914recip",
        version = "1.0.4",
        SmodMajor = 3,
        SmodMinor = 1,
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
			string[] hosts = { "https://storm37k.com/addons/", "http://74.91.115.126/addons/" };
			ushort version = ushort.Parse(this.Details.version.Replace(".", string.Empty));
			bool fail = true;
			string errorMSG = "";
			foreach (string host in hosts)
			{
				using (UnityEngine.WWW req = new UnityEngine.WWW(host + this.Details.name + ".ver"))
				{
					while (!req.isDone) { }
					errorMSG = req.error;
					if (string.IsNullOrEmpty(req.error))
					{
						ushort fileContentV = 0;
						if (!ushort.TryParse(req.text, out fileContentV))
						{
							errorMSG = "Parse Failure";
							continue;
						}
						if (fileContentV > version)
						{
							this.Error("Your version is out of date, please visit the Smod discord and download the newest version");
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
            this.AddEventHandler(typeof(IEventHandlerRoundStart), new EventHandler(this), Priority.Highest);
        }
    }
}
