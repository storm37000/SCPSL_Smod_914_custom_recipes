using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;

namespace SCP914CustomRecipes
{
	class EventHandler : IEventHandlerWaitingForPlayers
	{
		private Main plugin;
		private bool initialload = true;

		public EventHandler(Main plugin)
		{
			this.plugin = plugin;
		}

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			if (!plugin.UpToDate)
			{
				plugin.outdatedmsg();
			}

			if (initialload)
			{
				ParseRecipies(0);
			}
			ParseRecipies(1);
		}

		private void ParseRecipies(byte mode)
		{
			Scp914 objectOfType = UnityEngine.Object.FindObjectOfType<Scp914>();
			if ((UnityEngine.Object)objectOfType == (UnityEngine.Object)null)
			{
				this.plugin.Error("Couldnt find SCP-914");
			}
			else
			{
				string createText = "THIS IS NOT THE CONFIG FILE, JUST A REFERENCE!" + System.Environment.NewLine;
				for (byte index1 = 0; index1 < objectOfType.recipes.Length; ++index1) //item id
				{
					if (mode == 0) { createText = createText + "==== Recipe for: " + (ItemType)index1 + " ====" + System.Environment.NewLine; }
					for (byte index2 = 0; index2 < objectOfType.recipes[index1].outputs.Count; ++index2) //knob setting id
					{
						if (mode == 1) {
							string[] configsetting = this.plugin.GetConfigList((ItemType)index1 + "__" + (KnobSetting)index2);
							if (configsetting.Length !=0){
								System.Collections.Generic.List<int> bak = objectOfType.recipes[index1].outputs[index2].outputs;
								objectOfType.recipes[index1].outputs[index2].outputs.Clear();
								foreach (string confitm in configsetting)
								{
									this.plugin.Debug("converting/checking: " + confitm);
									sbyte result;
									bool error = false;
									if (System.SByte.TryParse(confitm, out result)) {
										//they used an itemid
										this.plugin.Debug("used an itemid of " + result);
										if (!System.Enum.IsDefined(typeof(ItemType), (int)result))
										{
											error = true;
										}

									}
									else if (System.Enum.IsDefined(typeof(ItemType), confitm))
									{
										//they used an enum
										this.plugin.Debug(confitm + " is a valid itemENUM");
										result = (sbyte)(ItemType)System.Enum.Parse(typeof(ItemType), confitm);
									} else
									{
										error = true;
									}
									if (error)
									{
										objectOfType.recipes[index1].outputs[index2].outputs = bak;
										this.plugin.Error("Config for " + (ItemType)index1 + "__" + (KnobSetting)index2 + " Contains an illegal value of " + confitm + ",  It must be an ItemID or a ItemENUM that exist.  The default value backup was restored.");
									} else
									{
										objectOfType.recipes[index1].outputs[index2].outputs.Add(result);
									}
								}
							}
						}

						foreach (sbyte itm in objectOfType.recipes[index1].outputs[index2].outputs) //output item id
						{
							if (mode == 0) { createText = createText + (KnobSetting)index2 + ": " + (ItemType)itm + System.Environment.NewLine; }
						}
						if (mode == 0) { this.plugin.AddConfig(new Smod2.Config.ConfigSetting((ItemType)index1 + "__" + (KnobSetting)index2, new string[] { }, Smod2.Config.SettingType.LIST, true, "")); }
					}
					if (mode == 0) { createText = createText + System.Environment.NewLine; }
				}
				if (mode == 0) {
					initialload = false;
					System.IO.File.WriteAllText("./914.txt", createText);
				}
			}
		}
	}
}
