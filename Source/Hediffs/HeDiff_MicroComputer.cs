using System.Collections.Generic;
using Verse;

namespace rjw
{
	internal class Hediff_MicroComputer : Hediff_MechImplants
	{
		protected int nextEventTick = 60000;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.nextEventTick, "nextEventTick", 60000, false);
		}

		public override void PostMake()
		{
			base.PostMake();
			nextEventTick = Rand.Range(mcDef.minEventInterval, mcDef.maxEventInterval);
		}

		public override void Tick()
		{
			base.Tick();
			if (this.pawn.IsHashIntervalTick(1000))
			{
				if (this.ageTicks >= nextEventTick)
				{
					HediffDef randomEffectDef = DefDatabase<HediffDef>.GetNamed(randomEffect);
					if (randomEffectDef != null)
					{
						pawn.health.AddHediff(randomEffectDef);
					}
					else
					{
						Logger.Message("[RJW]" + this.GetType().ToString() + "::Tick() - There is no Random Effect");
					}
					this.ageTicks = 0;
				}
			}
		}

		protected HediffDef_MechImplants mcDef
		{
			get { return ((HediffDef_MechImplants)def); }
		}

		protected List<string> randomEffects
		{
			get { return mcDef.randomHediffDefs; }
		}

		protected string randomEffect
		{
			get { return randomEffects.RandomElement<string>(); }
		}
	}
}