using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RR_PawnBadge
{
	public class Mod : Verse.Mod
	{
		public Settings Settings;

		public override string SettingsCategory()
		{
			return "PawnBadge.ModName".Translate();
		}

		public Mod(ModContentPack content) : base(content)
		{
			this.Settings = GetSettings<Settings>();
			var harmony = HarmonyInstance.Create("SaucyPigeon.PawnBadge");
			harmony.PatchAll();
		}

		public override void DoSettingsWindowContents(Rect canvas)
		{
			Settings.DoWindowContents(canvas);
		}
	}
}
