using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace rjw
{
	public class JobDriver_ViolateCorpse : JobDriver
	{
		private int duration;

		private int ticks_between_hearts;

		private int ticks_between_hits = 50;

		private int ticks_between_thrusts;

		protected TargetIndex iprisoner = TargetIndex.A;

		//private List<Apparel> worn_apparel; // Edited by nizhuan-jjr: No Dropping Clothes on attackers!

		// Same as in JobDriver_Lovin
		private static readonly SimpleCurve LovinIntervalHoursFromAgeCurve = new SimpleCurve
		{
			new CurvePoint(1f,  12f),
			new CurvePoint(16f, 6f),
			new CurvePoint(22f, 9f),
			new CurvePoint(30f, 12f),
			new CurvePoint(50f, 18f),
			new CurvePoint(75f, 24f)
		};

		protected Corpse corpse
		{
			get
			{
				return (Corpse)(job.GetTarget(iprisoner));
			}
		}

		public static void sexTick(Pawn pawn, Thing corpse)
		{
			pawn.rotationTracker.Face(corpse.DrawPos);

			if (xxx.config.sounds_enabled)
			{
				SoundDef.Named("Sex").PlayOneShot(new TargetInfo(pawn.Position, pawn.Map, false));
			}

			pawn.Drawer.Notify_MeleeAttackOn(corpse);
			pawn.rotationTracker.FaceCell(corpse.Position);
		}

		public override bool TryMakePreToilReservations()
		{
			return this.pawn.Reserve(this.corpse, this.job, 1, -1, null);
		}
		protected override IEnumerable<Toil> MakeNewToils()
		{
			//--Log.Message("[RJW] JobDriver_ViolateCorpse::MakeNewToils() called");
			duration = (int)(2000.0f * Rand.Range(0.50f, 0.90f));
			ticks_between_hearts = Rand.RangeInclusive(70, 130);
			ticks_between_hits = Rand.Range(xxx.config.min_ticks_between_hits, xxx.config.max_ticks_between_hits);
			ticks_between_thrusts = 100;

			if (xxx.is_bloodlust(pawn))
				ticks_between_hits = (int)(ticks_between_hits * 0.75);
			if (xxx.is_brawler(pawn))
				ticks_between_hits = (int)(ticks_between_hits * 0.90);

			//this.FailOnDespawnedNullOrForbidden (iprisoner);
			//this.FailOn (() => (!Prisoner.health.capacities.CanBeAwake) || (!comfort_prisoners.is_designated (Prisoner)));
			this.FailOn(() => !pawn.CanReserve(corpse, 1, 0));  // Fail if someone else reserves the prisoner before the pawn arrives
																//--Log.Message("[RJW] JobDriver_ViolateCorpse::MakeNewToils() - moving towards corpse");
			yield return Toils_Goto.GotoThing(iprisoner, PathEndMode.OnCell);
			Messages.Message(pawn.NameStringShort + " is trying to rape a corpse.", pawn, MessageTypeDefOf.NeutralEvent);

			var rape = new Toil();
			rape.initAction = delegate
			{
				//--Log.Message("[RJW] JobDriver_ViolateCorpse::MakeNewToils() - reserving corpse");
				//pawn.Reserve(corpse, 1, 0); // corpse rapin seems like a solitary activity

				// Try to take off the attacker's clothing
				//--Log.Message("[RJW] JobDriver_ViolateCorpse::MakeNewToils() - stripping necro lover");
				/* Edited by nizhuan-jjr: No Dropping Clothes on attackers!
						worn_apparel = pawn.apparel.WornApparel.ListFullCopy<Apparel>();
						while (pawn.apparel != null && pawn.apparel.WornApparelCount > 0) {
							Apparel apparel = pawn.apparel.WornApparel.RandomElement<Apparel>();
							pawn.apparel.Remove(apparel);
						}
				*/

				// Strip the corpse
				//--Log.Message("[RJW] JobDriver_ViolateCorpse::MakeNewToils() - stripping corpse");
				corpse.Strip();

				//pawn.apparel.WornApparel.RemoveAll(null);
			};
			rape.tickAction = delegate
			{
				if (pawn.IsHashIntervalTick(ticks_between_hearts))
					MoteMaker.ThrowMetaIcon(pawn.Position, pawn.Map, ThingDefOf.Mote_Heart);
				if (pawn.IsHashIntervalTick(ticks_between_thrusts))
					sexTick(pawn, corpse);
				/*
				if (pawn.IsHashIntervalTick (ticks_between_hits))
					roll_to_hit (pawn, Corpse);
                    */
			};
			rape.AddFinishAction(delegate
			{
				//--Log.Message("[RJW] JobDriver_ViolateCorpse::MakeNewToils() - finished violating");
				/*
				if ((Prisoner.jobs != null) &&
			    	(Prisoner.jobs.curDriver != null) &&
			    	(Prisoner.jobs.curDriver as JobDriver_GettinRaped != null))
	               		(Prisoner.jobs.curDriver as JobDriver_GettinRaped).rapist_count -= 1;
                        */
			});
			rape.defaultCompleteMode = ToilCompleteMode.Delay;
			rape.defaultDuration = duration;
			yield return rape;

			yield return new Toil
			{
				initAction = delegate
				{
					//--Log.Message("[RJW] JobDriver_ViolateCorpse::MakeNewToils() - creating aftersex toil");
					//Addded by nizhuan-jjr: Try to apply an aftersex process for the pawn and the corpse
					if (corpse.InnerPawn != null)
					{
						xxx.aftersex(pawn, corpse, true);//It's a function overloading: aftersex(Pawn p, Corpse c, bool violent =true)
					}
					pawn.mindState.canLovinTick = Find.TickManager.TicksGame + xxx.generate_min_ticks_to_next_lovin(pawn);
					//if (!Corpse.Dead) {
					//	xxx.aftersex (Corpse, pawn, pawn);
					//    Corpse.mindState.canLovinTick = Find.TickManager.TicksGame + xxx.generate_min_ticks_to_next_lovin (Corpse);
					//}

					//--Log.Message("[RJW] JobDriver_ViolateCorpse::MakeNewToils() - putting clothes back on");
					/* Edited by nizhuan-jjr: No Dropping Clothes on attackers!
							if (pawn.apparel != null) {
								foreach (Apparel apparel in worn_apparel) {
									pawn.apparel.Wear(apparel);//  WornApparel.Add(apparel);
								}
							}
					*/
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}
	}
}