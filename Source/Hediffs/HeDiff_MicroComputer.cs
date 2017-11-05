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
		protected int nextEventTick = 60000;
		public override void ExposeData()
		{
			Scribe_Values.Look<int>(ref this.nextEventTick, "nextEventTick", 60000, false);
		}
		public override void Tick()
		{
			base.Tick();
			if (this.pawn.IsHashIntervalTick(nextEventTick))
			{
				Log.Warning("Something happen");
				pawn.health.AddHediff(new Hediff_Orgasm());
				nextEventTick = Rand.Range(30000, 90000);
			}
		}

	}
}
