using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace rjw
{
	public class Building_WhoreBed : Building_Bed
	{
		public int priceOfWhore = 0;

		private static readonly Color whoreFieldColor = new Color(170 / 255f, 79 / 255f, 255 / 255f);

		private static readonly Color sheetColorForWhores = new Color(121 / 255f, 55 / 255f, 89 / 255f);

		private static readonly List<IntVec3> whoreField = new List<IntVec3>();

		public Pawn CurOccupant
		{
			get
			{
				var list = Map.thingGrid.ThingsListAt(Position);
				return list.OfType<Pawn>()
					.Where(pawn => pawn.jobs.curJob != null)
					.FirstOrDefault(pawn => pawn.jobs.curJob.def == JobDefOf.LayDown && pawn.jobs.curJob.targetA.Thing == this);
			}
		}

		public override Color DrawColor
		{
			get
			{
				if (def.MadeFromStuff)
				{
					return base.DrawColor;
				}
				return DrawColorTwo;
			}
		}
		private bool PlayerCanSeeOwners
		{
			get
			{
				if (Faction == Faction.OfPlayer)
				{
					return true;
				}
				for (int i = 0; i < owners.Count; i++)
				{
					if ((owners[i].Faction == Faction.OfPlayer) || (owners[i].HostFaction == Faction.OfPlayer))
					{
						return true;
					}
				}
				return false;
			}
		}
		private Vector3 GetMultiOwnersLabelScreenPosFor(int slotIndex)
		{
			IntVec3 sleepingSlotPos = GetSleepingSlotPos(slotIndex);
			Vector3 drawPos = DrawPos;
			if (Rotation.IsHorizontal)
			{
				drawPos.z = sleepingSlotPos.z + 0.6f;
			}
			else
			{
				drawPos.x = sleepingSlotPos.x + 0.5f;
				drawPos.z += -0.4f;
			}
			Vector2 vector2 = drawPos.MapToUIPosition();
			if (!Rotation.IsHorizontal && (SleepingSlotsCount == 2))
			{
				vector2 = AdjustOwnerLabelPosToAvoidOverlapping((Vector3)vector2, slotIndex);
			}
			return (Vector3)vector2;
		}
		private Vector3 AdjustOwnerLabelPosToAvoidOverlapping(Vector3 screenPos, int slotIndex)
		{
			Text.Font = GameFont.Tiny;
			float num = Text.CalcSize(owners[slotIndex].NameStringShort).x + 1f;
			Vector2 vector = this.DrawPos.MapToUIPosition();
			float num2 = Mathf.Abs((float)(screenPos.x - vector.x));
			IntVec3 sleepingSlotPos = GetSleepingSlotPos(slotIndex);
			if (num > (num2 * 2f))
			{
				float x = 0f;
				if (slotIndex == 0)
				{
					x = GetSleepingSlotPos(1).x;
				}
				else
				{
					x = GetSleepingSlotPos(0).x;
				}
				if (sleepingSlotPos.x < x)
				{
					screenPos.x -= (num - (num2 * 2f)) / 2f;
					return screenPos;
				}
				screenPos.x += (num - (num2 * 2f)) / 2f;
			}
			return screenPos;
		}


		public new IEnumerable<Pawn> AssigningCandidates
		{
			get
			{
				if (!Spawned)
				{
					return Enumerable.Empty<Pawn>();
				}
				return from p in Find.VisibleMap.mapPawns.FreeColonists
					   where xxx.can_be_fucked(p)
					   select p; //Map.mapPawns.FreeColonists;
			}
		}

		public void TryAssignPawns(Pawn apawn)
		{
			if (apawn != null && apawn.IsFreeColonist && !apawn.Dead && xxx.can_be_fucked(apawn))
			{
				apawn.ownership.ClaimBedIfNonMedical(this);
				priceOfWhore = 0;
			}
		}

		public override void Draw()
		{
			base.Draw();
			if (Medical) Medical = false;
			if (ForPrisoners) ForPrisoners = false;
		}

		public override Color DrawColorTwo { get { return sheetColorForWhores; } }

		public override void DeSpawn()
		{
			foreach (var owner in owners.ToArray())
			{
				owner.ownership.UnclaimBed();
			}
			var room = Position.GetRoom(Map);
			base.DeSpawn();
			if (room != null)
			{
				room.Notify_RoomShapeOrContainedBedsChanged();
			}
		}

		//public override void DrawExtraSelectionOverlays()
		//{
		//    base.DrawExtraSelectionOverlays();
		//    var room = this.GetRoom();
		//    if (room == null) return;
		//    if (room.isPrisonCell) return;
		//
		//    if (room.RegionCount < 20 && !room.TouchesMapEdge)
		//    {
		//        foreach (var current in room.Cells)
		//        {
		//            whoreField.Add(current);
		//        }
		//        var color = whoreFieldColor;
		//        color.a = Pulser.PulseBrightness(1f, 0.6f);
		//        GenDraw.DrawFieldEdges(whoreField, color);
		//        whoreField.Clear();
		//    }
		//}

		public override string GetInspectString()
		{
			var stringBuilder = new StringBuilder();
			//stringBuilder.Append(base.GetInspectString());
			stringBuilder.Append(InspectStringPartsFromComps());
			stringBuilder.AppendLine();
			stringBuilder.Append("ForWhoreUse".Translate());

			stringBuilder.AppendLine();
			if (owners.Count == 0)
			{
				stringBuilder.Append("Owner".Translate() + ": " + "Nobody".Translate());
			}
			else if (owners.Count == 1)
			{
				stringBuilder.Append("Owner".Translate() + ": " + owners[0].LabelCap);
			}
			else
			{
				stringBuilder.Append("Owners".Translate() + ": ");
				bool notFirst = false;
				foreach (Pawn owner in owners)
				{
					if (notFirst)
					{
						stringBuilder.Append(", ");
					}
					notFirst = true;
					stringBuilder.Append(owner.Label);
				}
				//if(notFirst) stringBuilder.AppendLine();
			}
			return stringBuilder.ToString();
		}

		/* This is included in Building_Bed_Patch.cs where we adds a PostFix to the Building_Bed.GetGizmos method
        public override IEnumerable<Gizmo> GetGizmos()
        {
            // Get original gizmos from Building class
            var method = typeof(Building).GetMethod("GetGizmos"); //nizhuan-jjr:I think the type should be changed from Building to Building_Bed, but the Building_Bed_Patch.cs is doing the job.
            var ftn = method.MethodHandle.GetFunctionPointer();
            var func = (Func<IEnumerable<Gizmo>>)Activator.CreateInstance(typeof(Func<IEnumerable<Gizmo>>), this, ftn);

            foreach (var gizmo in func())
            {
                yield return gizmo;
            }

            if (xxx.config.whore_beds_enabled && def.building.bed_humanlike)
            {
                yield return 
                    new Command_Toggle
                    {
                        defaultLabel = "CommandBedSetAsWhoreLabel".Translate(),
                        defaultDesc = "CommandBedSetAsWhoreDesc".Translate(),
                        icon = ContentFinder<Texture2D>.Get("UI/Commands/AsWhore"),
                        isActive = () => true,
                        toggleAction = () => Swap(this),
                        hotKey = KeyBindingDefOf.Misc4
                    };
            }
        }
        */

		public override void PostMake()
		{
			base.PostMake();
			PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDef.Named("WhoreBeds"), KnowledgeAmount.Total);
		}

		public override void DrawGUIOverlay()
		{
			//if (Find.CameraMap.CurrentZoom == CameraZoomRange.Closest)
			//{
			//    if (owner != null && owner.InBed() && owner.CurrentBed().owner == owner)
			//    {
			//        return;
			//    }
			//    string text;
			//    if (owner != null)
			//    {
			//        text = owner.NameStringShort;
			//    }
			//    else
			//    {
			//        text = "Unowned".Translate();
			//    }
			//    GenWorldUI.DrawThingLabel(this, text, new Color(1f, 1f, 1f, 0.75f));
			//}
			//Added by nizhuan-jjr: I simply use the parent method because whore beds should have 3 versions.
			base.DrawGUIOverlay();
			/*Added by nizhuan-jjr: Below is copy from Building_Bed.DrawGUIOverlay(), since Whorebeds are designed to have 3 versions - Single bed,Doulbe bed, Royal bed.
            if (!Medical && ((Find.CameraDriver.CurrentZoom == CameraZoomRange.Closest) && PlayerCanSeeOwners))
            {
                Color defaultThingLabelColor = GenMapUI.DefaultThingLabelColor;
                if (!owners.Any())
                {
                    GenMapUI.DrawThingLabel(this, "Unowned".Translate(), defaultThingLabelColor);
                }
                else if (owners.Count == 1)
                {
                    if (!owners[0].InBed() || (owners[0].CurrentBed() != this))
                    {
                        GenMapUI.DrawThingLabel(this, owners[0].NameStringShort, defaultThingLabelColor);
                    }
                }
                else
                {
                    for (int i = 0; i < owners.Count; i++)
                    {
                        if ((!owners[i].InBed() || (owners[i].CurrentBed() != this)) || (owners[i].Position != GetSleepingSlotPos(i)))
                        {
                            GenMapUI.DrawThingLabel(GetMultiOwnersLabelScreenPosFor(i), owners[i].NameStringShort, defaultThingLabelColor);
                        }
                    }
                }
            }
            */
		}

	}
}
