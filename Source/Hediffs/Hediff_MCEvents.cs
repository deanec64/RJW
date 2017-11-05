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
		public override void PostAdd(DamageInfo? dinfo)
		{
			Messages.Message("FeltOrgasm".Translate(new object[] { this.pawn.LabelIndefinite() }).CapitalizeFirst(), MessageSound.Standard);
		}

		public override bool TryMergeWith(Hediff other)
		{
			if (other == null || other.def != this.def || other.Part != this.Part)
			{
				return false;
			}
			this.Severity += other.Severity;
			this.ageTicks = 0;
			return true;
		}
	}
	class Hediff_TransportCums : HediffWithComps
	{
		public override void PostAdd(DamageInfo? dinfo)
		{
			Messages.Message("CumsTransported".Translate(new object[] { this.pawn.LabelIndefinite() }).CapitalizeFirst(), MessageSound.Standard);

			pawn.health.RemoveHediff(this);
		}

	}
}
