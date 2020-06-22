using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RR_PawnBadge
{
    public class ITab_Pawn_Badge : ITab
    {
        private bool[] badgePainting = { false, false };

		private readonly Vector2[] scrollPositions = new[]
		{
			Vector2.zero, Vector2.zero
		};

        public ITab_Pawn_Badge()
        {
            this.size = new UnityEngine.Vector2(500f, 470f); ;
            this.labelKey = "PawnBadge.BadgeTab";
        }

        private Pawn SelPawnForBadgeInfo
        {
            get
            {
                if (base.SelPawn != null)
                {
                    return base.SelPawn;
                }
                Corpse corpse = base.SelThing as Corpse;
                if (corpse != null)
                {
                    return corpse.InnerPawn;
                }
                throw new InvalidOperationException("Badge tab on non-pawn non-corpse " + base.SelThing);
            }
        }

        protected override void FillTab()
        {
            CompBadge cb = SelPawnForBadgeInfo.GetComp<CompBadge>();
            if (cb == null) return;

            Rect rect = new Rect(0f, 0f, this.size.x, this.size.y);

            Rect[] badgeRects = new Rect[] {
                new Rect(rect.x, rect.y, rect.width, rect.height / 2),
                new Rect(rect.x, (rect.height / 2) + 1, rect.width, rect.height / 2),
            };

            GUI.BeginGroup(rect);
            Text.Font = GameFont.Small;

            GUI.color = new Color(1f, 1f, 1f, 0.5f);
            Widgets.DrawLineHorizontal(badgeRects[0].x, badgeRects[0].yMax, badgeRects[0].width);
            GUI.color = Color.white;

            for (int i = 0; i < 2; i++)
            {
				DoBadgeDisplay(i, badgeRects, cb);
            }

            GUI.EndGroup();
        }

		private static string GetBadgePositionKey(int badgeIndex)
		{
			var positionOffset = Settings.badgePosition;

			if (badgeIndex == 0)
			{
				switch (positionOffset)
				{
					case Settings.BadgePosition.Top:
						return "PawnBadge.BadgePosition_TopLeft";
					case Settings.BadgePosition.Bottom:
						return "PawnBadge.BadgePosition_BottomLeft";
					case Settings.BadgePosition.Left:
						return "PawnBadge.BadgePosition_TopLeft";
					case Settings.BadgePosition.Right:
						return "PawnBadge.BadgePosition_TopRight";
					default:
						throw new EnumValueOutOfRange(typeof(Settings.BadgePosition), positionOffset, nameof(positionOffset));
				}
			}
			else if (badgeIndex == 1)
			{

				switch (positionOffset)
				{
					case Settings.BadgePosition.Top:
						return "PawnBadge.BadgePosition_TopRight";
					case Settings.BadgePosition.Bottom:
						return "PawnBadge.BadgePosition_BottomRight";
					case Settings.BadgePosition.Left:
						return "PawnBadge.BadgePosition_BottomLeft";
					case Settings.BadgePosition.Right:
						return "PawnBadge.BadgePosition_BottomRight";
					default:
						throw new EnumValueOutOfRange(typeof(Settings.BadgePosition), positionOffset, nameof(positionOffset));
				}
			}
			else
			{
				throw new ArgumentOutOfRangeException(nameof(badgeIndex), $"Value of {nameof(badgeIndex)} ({badgeIndex}) must be either zero or one.");
			}
		}

		private void DoBadgeDisplay(int i, IList<Rect> badgeRects, CompBadge cb)
		{
			Rect outRect = new Rect(badgeRects[i].ContractedBy(12f));
			// Badge {i + 1} ({GetBadgePosition(i)})
			string badgePositionString = GetBadgePositionKey(i).Translate();
			string title = "PawnBadge.BadgeNumber".Translate(i + 1, badgePositionString);
			Vector2 textSize = Text.CalcSize(title);

			Widgets.Label(new Rect(outRect.x, outRect.y, outRect.width, textSize.y), title);
			outRect.y += textSize.y;
			outRect.height -= textSize.y;

			Rect badgeRect = outRect;
			badgeRect.width -= 16f;
			LayoutManager layout = new LayoutManager(badgeRect, 40f, 30f);
			List<BadgeDef> defs = new List<BadgeDef>(DefDatabase<BadgeDef>.AllDefsListForReading);
			defs.Insert(0, new BadgeDef("", Mod.GreyTex));

			badgeRect.height = layout.MaxHeightNeeded(defs.Count());

			Widgets.BeginScrollView(outRect, ref scrollPositions[i], badgeRect, true);





			foreach (BadgeDef def in defs)
			{
				Rect brect = layout.CurRect;

				Widgets.DrawHighlightIfMouseover(brect);
				GUI.DrawTexture(brect, def.Symbol, ScaleMode.ScaleToFit);
				Widgets.DraggableResult draggableResult = Widgets.ButtonInvisibleDraggable(brect, false);
				if (draggableResult == Widgets.DraggableResult.Dragged)
				{
					badgePainting[i] = true;
				}
				if ((badgePainting[i] && Mouse.IsOver(brect) && def.defName != cb.badges[i]) || AnyPressed(draggableResult))
				{
					cb.badges[i] = def.defName;
					SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
				}
				if (cb.badges[i] == def.defName)
				{
					Widgets.DrawBox(brect, 3);
				}
				TooltipHandler.TipRegion(brect, () => { return GetBadgeDescription(def); }, 3882382 + (int)brect.y * 17);
				layout.Next();
			}
			if (!Input.GetMouseButton(0))
			{
				badgePainting[i] = false;
			}
			Widgets.EndScrollView();
		}

		private static string GetBadgeDescription(BadgeDef badgeDef)
		{
			if (badgeDef.defName == "")
			{
				return "PawnBadge.NoBadge".Translate();
			}
			return badgeDef.description;
		}

		private static bool AnyPressed(Widgets.DraggableResult result)
        {
            return result == Widgets.DraggableResult.Pressed || result == Widgets.DraggableResult.DraggedThenPressed;
        }

        private class LayoutManager
        {
            private Rect rect;
            private float itemWidth;
            private float itemHeight;
            private float curX;
            private float curY;

            public LayoutManager(Rect rect, float itemWidth, float itemHeight)
            {
                this.rect = rect;
                this.itemWidth = itemWidth;
                this.itemHeight = itemHeight;

                this.curX = this.rect.x;
                this.curY = this.rect.y;
            }

            public Rect CurRect
            {
                get { return new Rect(this.curX, this.curY, this.itemWidth, this.itemHeight); }
            }

			public float MaxHeightNeeded(int count)
			{
				int nPerRow = (int)(rect.xMax / itemWidth);
				int columnsN = Mathf.CeilToInt(count / (float)nPerRow);
				return columnsN * itemHeight;
			}

            public void Next()
            {
                this.curX += this.itemWidth;
                if (this.curX + this.itemWidth > this.rect.xMax)
                {
                    this.curX = this.rect.x;
                    this.curY += this.itemHeight;
                }
            }
        }
    }
}
