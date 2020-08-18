using System.Collections.Generic;
using System.Linq;
using Verse;
using UnityEngine;

namespace RR_PawnBadge
{
	[StaticConstructorOnStartup]
    public static class Controller
    {
		public static readonly Texture2D GreyTex = NewSolidColorTexture(Color.gray);

		public static Texture2D NewSolidColorTexture(Color color)
		{
			if (!UnityData.IsInMainThread)
			{
				Log.Error("Tried to create a texture from a different thread.", false);
				return null;
			}
			Texture2D texture2D = new Texture2D(35, 35);
			texture2D.name = "RR_PawnBadge-SolidColorTex-" + color;

			var fillColorArray = texture2D.GetPixels();
			for (var i = 0; i < fillColorArray.Length; ++i)
			{
				fillColorArray[i] = color;
			}
			texture2D.SetPixels(fillColorArray);
			texture2D.Apply();
			return texture2D;
		}

		public static void EditDefs()
		{
			IEnumerable<ThingDef> things = (
				from def in DefDatabase<ThingDef>.AllDefs
				where def.race?.intelligence == Intelligence.Humanlike
				select def
			);
			foreach (ThingDef t in things)
			{
				// add badge tab to pawns
				if (t.inspectorTabsResolved == null)
				{
					t.inspectorTabsResolved = new List<InspectTabBase>(1);
				}
				t.inspectorTabsResolved.Add(InspectTabManager.GetSharedInstance(typeof(ITab_Pawn_Badge)));

				// add Badge component property to all humanlike pawns
				if (t.comps == null)
				{
					t.comps = new List<CompProperties>(1);
				}
				t.comps.Add(new CompProperties_Badge());
			}
		}

		static Controller()
		{
			EditDefs();
		}
	}
}
