using System;
using System.Text;
using UnityEngine;

using Verse;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;

namespace rjw
{
	class Hediff_MicroComputer : Hediff_MechImplants
	{
		//protected int nextEventTick = 60000;
		protected int nextEventTick = 2000;
		public override void ExposeData()
		{
			Scribe_Values.Look<int>(ref this.nextEventTick, "nextEventTick", 60000, false);
		}
		public override void Tick()
		{
			if (this.pawn.IsHashIntervalTick(nextEventTick))
			{
				Log.Warning("Something happen");
				pawn.health.AddHediff(DefDatabase<HediffDef>.GetNamed(randomEffect));
				nextEventTick = Rand.Range(mcDef.minEventInterval, mcDef.maxEventInterval);
			}
			base.Tick();
		}
		protected HediffDef_MicroComputer mcDef
		{
			get { return ((HediffDef_MicroComputer)def); }
		}
		protected List<string> randomEffects
		{
			get{ return ((HediffDef_MicroComputer)def).randomHediffDefs; }
		}
		protected string randomEffect
		{
			get { return randomEffects[Rand.Range(0, randomEffects.Count)]; }
		}
	}

	[StaticConstructorOnStartup]
	class HediffDef_MicroComputer : HediffDef_EnemyImplants
	{
		public List<string> randomHediffDefs = new List<string>();
		public int minEventInterval = 30000;
		public int maxEventInterval = 90000;
	}
}
