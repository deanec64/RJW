using System;
using System.Text;
using UnityEngine;

using Verse;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;

namespace rjw
{
	class Hediff_Orgasm : HediffWithComps
	{
		public override void Tick()
		{
			this.ageTicks++;
			if (this.pawn.IsHashIntervalTick(1000))
			{
				pawn.health.RemoveHediff(this);
			}
		}
	}
}
