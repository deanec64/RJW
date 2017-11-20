using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	public class JobDriver_GettinRaped : JobDriver
	{
		private int ticks_between_hearts;

		private int ticks_remaining = 10;

		public int rapist_count = 1; // Defaults to 1 so the first rapist doesn't have to add themself

		private bool was_laying_down;

		internal RJW_DummyContainer dummy_container;

		public void increase_time(int min_ticks_remaining)
		{
			if (min_ticks_remaining > ticks_remaining)
				ticks_remaining = min_ticks_remaining;
		}

		public override bool TryMakePreToilReservations()
		{
			return true;
		}
		protected override IEnumerable<Toil> MakeNewToils()
		{
			ticks_between_hearts = Rand.RangeInclusive(70, 130);
			was_laying_down = (pawn.jobs.curDriver != null) && pawn.jobs.curDriver.layingDown != Verse.AI.LayingDownState.NotLaying;

			var get_raped = new Toil();
			get_raped.defaultCompleteMode = ToilCompleteMode.Never;
			get_raped.initAction = delegate
			{
				pawn.pather.StopDead();
				pawn.jobs.curDriver.layingDown = Verse.AI.LayingDownState.NotLaying;
				pawn.jobs.curDriver.asleep = false;
				//pawn.mindState.awokeVoluntarily = false;
				// Added by nizhuan-jjr: human pawns get raped will put on clothes again.
				if (!xxx.is_animal(pawn))
				{
					dummy_container = new RJW_DummyContainer();
					foreach (Apparel apparel in pawn.apparel.GetDirectlyHeldThings().OfType<Apparel>().ToList<Apparel>())
					{
						if (!(apparel is rjw.bondage_gear))
						{
							pawn.apparel.GetDirectlyHeldThings().TryTransferToContainer(apparel, dummy_container.GetDirectlyHeldThings(), true);
							if ((pawn.outfits != null) && pawn.outfits.forcedHandler.IsForced(apparel))
							{
								dummy_container.forcedApparel.Add(apparel);
							}
						}
					}
				}
				Messages.Message("GetinRapedNow".Translate(new object[] { pawn.LabelIndefinite() }).CapitalizeFirst(), pawn, MessageTypeDefOf.NegativeEvent);

			};
			get_raped.tickAction = delegate
			{
				--ticks_remaining;
				/*
                if ((ticks_remaining <= 0) || (rapist_count <= 0))
                    ReadyForNextToil();
                */
				if ((rapist_count > 0) && (pawn.IsHashIntervalTick(ticks_between_hearts / rapist_count)))
					MoteMaker.ThrowMetaIcon(pawn.Position, pawn.Map, xxx.mote_noheart);
			};
			get_raped.AddEndCondition(new Func<JobCondition>(() =>
			{
				if ((ticks_remaining <= 0) || (rapist_count <= 0))
					return JobCondition.Succeeded;
				return JobCondition.Ongoing;
			}));
			get_raped.AddFinishAction(delegate
			{
				pawn.jobs.curDriver.layingDown = was_laying_down ? Verse.AI.LayingDownState.NotLaying : Verse.AI.LayingDownState.LayingInBed;

				// Added by nizhuan-jjr: human pawns get raped will put on clothes again.
				if (!xxx.is_animal(pawn))
				{
					if (dummy_container != null)
					{
						foreach (Apparel apparel in dummy_container.GetDirectlyHeldThings().OfType<Apparel>().ToList<Apparel>())
						{
							if (!(apparel is rjw.bondage_gear))
							{
								dummy_container.GetDirectlyHeldThings().TryTransferToContainer(apparel, pawn.apparel.GetDirectlyHeldThings(), true);
								if (dummy_container.forcedApparel.Contains(apparel) && (pawn.outfits != null))
								{
									pawn.outfits.forcedHandler.SetForced(apparel, true);
								}
							}
						}
					}
				}
			});
			get_raped.socialMode = RandomSocialMode.Off;
			yield return get_raped;

			/*
            var after_rape = new Toil();
            after_rape.defaultCompleteMode = ToilCompleteMode.Delay;
            after_rape.defaultDuration = 150;
            after_rape.socialMode = RandomSocialMode.Off;
            after_rape.AddFinishAction(delegate {
                pawn.jobs.curDriver.layingDown = was_laying_down ? Verse.AI.LayingDownState.NotLaying : Verse.AI.LayingDownState.LayingInBed;

                // Added by nizhuan-jjr: human pawns get raped will put on clothes again.
                if (!xxx.is_animal(pawn))
                {
                    if (dummy_container != null)
                    {
                        foreach (Apparel apparel in dummy_container.GetDirectlyHeldThings().OfType<Apparel>().ToList<Apparel>())
                        {
                            if (!(apparel is rjw.bondage_gear))
                            {
                                dummy_container.GetDirectlyHeldThings().TryTransferToContainer(apparel, pawn.apparel.GetDirectlyHeldThings(), true);
                                if (dummy_container.forcedApparel.Contains(apparel) && (pawn.outfits != null))
                                {
                                    pawn.outfits.forcedHandler.SetForced(apparel, true);
                                }
                            }
                        }
                    }
                }
            });
            yield return after_rape;
            */
		}
	}

	//create a dummy container to solve the problem of storage of taken-off apparels
	public class RJW_DummyContainer : IExposable, IThingHolder, ILoadReferenceable
	{
		public List<Apparel> forcedApparel;
		private ThingOwner innerContainer;
		private string UID;

		public RJW_DummyContainer()
		{
			forcedApparel = new List<Apparel>();
			innerContainer = new ThingOwner<Apparel>(this, false, LookMode.Deep);
			UID = "RJW_DUMMY_CONTAINER" + Find.TickManager.TicksGame;
		}

		public void GetChildHolders(List<IThingHolder> outChildren)
		{
			ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
		}

		public ThingOwner GetDirectlyHeldThings()
		{
			return innerContainer;
		}

		public IThingHolder ParentHolder
		{
			get
			{
				return ParentHolder;
			}
		}

		public void ExposeData()
		{
			innerContainer.ExposeData();
			Scribe_Collections.Look<Apparel>(ref forcedApparel, "forcedApparel", LookMode.Reference, new object[0]);
			Scribe_Values.Look<String>(ref UID, "UID", "", false);
		}

		public string GetUniqueLoadID()
		{
			return UID;
		}
	}
}