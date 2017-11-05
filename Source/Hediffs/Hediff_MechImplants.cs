using System;
using System.Text;
using UnityEngine;

using Verse;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;

namespace rjw
{
	class Hediff_MechImplants : HediffWithComps
	{
		public override bool TryMergeWith(Hediff other)
		{
			return false;
		}
	}

	[StaticConstructorOnStartup]
	class HediffDef_MechImplants : HediffDef_EnemyImplants
	{
	}
}
