using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace rjw
{
	internal class Hediff_InsectEgg : HediffWithComps
	{
		/*public override void Tick()
		{
			base.Tick();
			Log.Message("[RJW]Hediff_InsectEgg::Tick() - InsectEgg growing");
		}*/

		protected int bornTick
		{
			get
			{
				return ((HediffDef_InsectEgg)this.def).bornTick;
			}
		}

		protected int abortTick
		{
			get
			{
				return ((HediffDef_InsectEgg)this.def).abortTick;
			}
		}

		public Pawn father;
		public Pawn mother;

		public string parentDef
		{
			get
			{
				return ((HediffDef_InsectEgg)def).parentDef;
			}
		}

		public List<string> parentDefs
		{
			get
			{
				return ((HediffDef_InsectEgg)def).parentDefs;
			}
		}

		public override void PostAdd(DamageInfo? dinfo)
		{
			//Log.Message("[RJW]Hediff_InsectEgg::PostAdd() - added parentDef:" + parentDef+"");
			base.PostAdd(dinfo);
		}

		public override void Tick()
		{
			this.ageTicks++;
			if (this.pawn.IsHashIntervalTick(1000))
			{
				if (father != null)
				{
					if (this.ageTicks >= bornTick)
					{
						BirthBaby();
					}
				}
				else
				{
					if (this.ageTicks >= abortTick)
					{
						Messages.Message("EggDead".Translate(new object[] { this.pawn.LabelIndefinite() }).CapitalizeFirst(), MessageSound.Standard);
						pawn.health.RemoveHediff(this);
					}
				}
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_References.Look<Pawn>(ref this.father, "father", false);
			Scribe_References.Look<Pawn>(ref this.mother, "mother", false);
		}

		public void BirthBaby()
		{
			Log.Message("[RJW]Hediff_InsectEgg::BirthBaby() - Egg of " + parentDef + " in " + pawn.ToString() + " birth!");
			PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDef.Named(parentDef), Faction.OfInsects, PawnGenerationContext.NonPlayer, pawn.Map.Tile, false, true, false, false, false, false, 0, false, true, true, false, false, null, 0, 0, null, null, null);

			//Log.Message("Hediff_GenericPregnancy::DoBirthSpawn( " + mother_name + ", " + father_name + ", " + chance_successful + " ) - spawning baby");
			Pawn baby = PawnGenerator.GeneratePawn(request);
			if (PawnUtility.TrySpawnHatchedOrBornPawn(baby, pawn))
			{
				if (pawn.RaceProps.IsFlesh)
				{
					pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, mother);
					if (father != null)
					{
						pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, father);
					}
				}
			}
			else
			{
				Find.WorldPawns.PassToWorld(baby, PawnDiscardDecideMode.Discard);
			}

			if (this.Visible && baby != null)
			{
				Messages.Message("MessageGaveBirth".Translate(new object[] { this.pawn.LabelIndefinite() }).CapitalizeFirst(), baby, MessageSound.Standard);
			}
			pawn.health.RemoveHediff(this);
		}

		public void Fertilize(Pawn f)
		{
			if (father != null)
			{
				father = (Rand.Range(0, 1) == 0) ? father : f;
			}
			else
			{
				father = f;
				Log.Message("[RJW]Hediff_InsectEgg::Fertilize() - Egg in " + pawn.ToString() + " is fertilized by " + f.ToString());
			}
		}

		public override bool TryMergeWith(Hediff other)
		{
			return false;
		}

		public bool IsParent(string defnam)
		{
			return parentDef == defnam || parentDefs.Contains(defnam);
		}

		public override string DebugString()
		{
			return base.DebugString() + " Age:" + this.ageTicks + "\nFertilized:" + (father != null).ToString();
		}
	}
}