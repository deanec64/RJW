using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace rjw
{
	public class JobDriver_RapeEnemy : JobDriver_Rape
	{
		public override void roll_to_hit(Pawn rapist, Pawn p)
		{
			if (!Mod_Settings.prisoner_beating)
			{
				return;
			}

			float rand_value = Rand.Value;
			float victim_pain = p.health.hediffSet.PainTotal;

			float beating_chance = xxx.config.base_chance_to_hit_prisoner * (xxx.is_bloodlust(rapist) ? 1.25f : 1.0f);
			float beating_threshold = xxx.is_psychopath(rapist) ? xxx.config.extreme_pain_threshold : xxx.config.significant_pain_threshold;

			if ((victim_pain < beating_threshold && rand_value < beating_chance) || (rand_value < (beating_chance / 2)))
			{
				if (InteractionUtility.TryGetRandomVerbForSocialFight(rapist, out Verb v))
				{
					rapist.meleeVerbs.TryMeleeAttack(p, v);
				}
			}
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			duration = (int)(2000.0f * Rand.Range(0.50f, 0.90f));
			ticks_between_hearts = Rand.RangeInclusive(70, 130);
			ticks_between_hits = Rand.Range(xxx.config.min_ticks_between_hits, xxx.config.max_ticks_between_hits);
			ticks_between_thrusts = 100;
			bool pawnHasPenis = Genital_Helper.has_penis(pawn);

			if (xxx.is_bloodlust(pawn))
				ticks_between_hits = (int)(ticks_between_hits * 0.75);
			if (xxx.is_brawler(pawn))
				ticks_between_hits = (int)(ticks_between_hits * 0.90);

			this.FailOnDespawnedNullOrForbidden(iTarget);
			this.FailOn(() => (!Target.health.capacities.CanBeAwake)); // || (!comfort_Targets.is_designated (Target)));
			this.FailOn(() => !pawn.CanReserve(Target, comfort_prisoners.max_rapists_per_prisoner, 0)); // Fail if someone else reserves the Target before the pawn arrives
			yield return Toils_Goto.GotoThing(iTarget, PathEndMode.OnCell);

			var rape = new Toil();
			rape.initAction = delegate
			{
				pawn.Reserve(Target, comfort_prisoners.max_rapists_per_prisoner, 0);
				if (!pawnHasPenis)
					Target.Drawer.rotator.Face(pawn.DrawPos);
				var dri = Target.jobs.curDriver as JobDriver_GettinRaped;
				if (dri == null)
				{
					var gettin_raped = new Job(xxx.gettin_raped);
					Target.jobs.StartJob(gettin_raped, JobCondition.InterruptForced, null, false, true, null);
					(Target.jobs.curDriver as JobDriver_GettinRaped).increase_time(duration);
				}
				else
				{
					dri.rapist_count += 1;
					dri.increase_time(duration);
				}
			};
			rape.tickAction = delegate
			{
				if (pawn.IsHashIntervalTick(ticks_between_hearts))
					MoteMaker.ThrowMetaIcon(pawn.Position, pawn.Map, ThingDefOf.Mote_Heart);
				if (pawn.IsHashIntervalTick(ticks_between_thrusts))
					xxx.sexTick(pawn, Target);
				if (pawn.IsHashIntervalTick(ticks_between_hits))
					roll_to_hit(pawn, Target);
			};
			rape.AddFinishAction(delegate
			{
				//// Trying to add some interactions and social logs
				xxx.processAnalSex(pawn, Target, ref isAnalSex, pawnHasPenis);

				if ((Target.jobs != null) &&
					(Target.jobs.curDriver != null) &&
					(Target.jobs.curDriver as JobDriver_GettinRaped != null))
				{
					(Target.jobs.curDriver as JobDriver_GettinRaped).rapist_count -= 1;
				}
			});
			rape.defaultCompleteMode = ToilCompleteMode.Delay;
			rape.defaultDuration = duration;
			yield return rape;

			yield return new Toil
			{
				initAction = delegate
				{
					aftersex(pawn, Target, true, isAnalSex: isAnalSex);
					pawn.mindState.canLovinTick = Find.TickManager.TicksGame + xxx.generate_min_ticks_to_next_lovin(pawn);
					if (!Target.Dead)
					{
						Target.mindState.canLovinTick = Find.TickManager.TicksGame + xxx.generate_min_ticks_to_next_lovin(Target);
					}
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}

		public override void aftersex(Pawn pawn, Pawn part, bool violent = false, bool isCoreLovin = false, bool isAnalSex = false)
		{
			var pawn_name = (pawn != null) ? pawn.NameStringShort : "NULL";
			var part_name = (pawn != null) ? part.NameStringShort : "NULL";
			//--Log.Message("[ABF]JobDriver_RapeEnemy::aftersex( " + pawn_name + ", " + part_name + " ) called");
			pawn.Drawer.rotator.Face(part.DrawPos);
			pawn.Drawer.rotator.FaceCell(part.Position);

			part.Drawer.rotator.Face(pawn.DrawPos);
			part.Drawer.rotator.FaceCell(pawn.Position);

			if (violent)
			{
				pawn.Drawer.Notify_MeleeAttackOn(part);
			}

			if (xxx.config.sounds_enabled)
			{
				SoundDef.Named("Cum").PlayOneShot(new TargetInfo(part.Position, pawn.Map, false));
			}

			bool pawnIsNotHuman = xxx.is_animal(pawn) || xxx.is_mechanoid(pawn);
			bool partIsNotHuman = xxx.is_animal(part) || xxx.is_mechanoid(pawn);
			//--Log.Message("[ABF]JobDriver_RapeEnemy::aftersex( " + pawn_name + ", " + part_name + " ) - applying cum effect");
			if (xxx.config.cum_enabled)
			{
				int pawn_cum = pawnIsNotHuman ? 4 : Math.Min((int)(pawn.RaceProps.lifeExpectancy / pawn.ageTracker.AgeBiologicalYears), 2);
				int part_cum = partIsNotHuman ? 4 : Math.Min((int)(part.RaceProps.lifeExpectancy / part.ageTracker.AgeBiologicalYears), 2);
				if (pawn.gender == Gender.Female)
					pawn_cum /= 2;
				if (part.gender == Gender.Female)
					part_cum /= 2;
				FilthMaker.MakeFilth(pawn.PositionHeld, pawn.MapHeld, xxx.cum, pawn.LabelIndefinite(), pawn_cum);
				if (!isCoreLovin)
				{
					FilthMaker.MakeFilth(part.PositionHeld, part.MapHeld, xxx.cum, part.LabelIndefinite(), part_cum);
				}
			}

			//--Log.Message("[ABF]JobDriver_RapeEnemy::aftersex( " + pawn_name + ", " + part_name + " ) - checking satisfaction");
			xxx.satisfy(pawn, part, violent, isCoreLovin);
			//--Log.Message("[ABF]JobDriver_RapeEnemy::aftersex( " + pawn_name + ", " + part_name + " ) - checking thoughts");
			think_after_sex(pawn, part, violent, isCoreLovin);

			if (!isAnalSex)
			{
				//--Log.Message("[ABF]JobDriver_RapeEnemy::aftersex( " + pawn_name + ", " + part_name + " ) - checking pregnancy");
				xxx.impregnate(pawn, part);
			}

			if (pawnIsNotHuman || partIsNotHuman) return;
			//--Log.Message("[ABF]JobDriver_RapeEnemy::aftersex( " + pawn_name + ", " + part_name + " ) - checking disease");
			std.roll_to_catch(pawn, part);
		}

		public override void think_after_sex(Pawn pawn, Pawn part, bool violent = false, bool isCoreLovin = false)
		{
			//--Log.Message("[ABF]JobDriver_RapeEnemy::think_after_sex( " + pawn.NameStringShort + ", " + part.NameStringShort + ", " + violent + " ) called");
			//--Log.Message("[ABF]JobDriver_RapeEnemy::think_after_sex( " + pawn.NameStringShort + ", " + part.NameStringShort + ", " + violent + " ) - setting part thoughts");

			// partner thoughts
			if (!xxx.is_animal(part) && violent && !part.Dead && !isCoreLovin)
			{
				if (xxx.is_animal(pawn) || xxx.is_mechanoid(pawn))
				{
					var part_thought = (xxx.is_masochist(part)) ? xxx.masochist_got_raped_by_animal : xxx.got_raped_by_animal;
					part.needs.mood.thoughts.memories.TryGainMemory(part_thought);
				}
				else
				{
					var part_thought = (xxx.is_masochist(part)) ? xxx.masochist_got_raped : xxx.got_raped;
					part.needs.mood.thoughts.memories.TryGainMemory(part_thought);

					var part_thought_about_rapist = (!xxx.is_masochist(part)) ? xxx.hate_my_rapist : xxx.kinda_like_my_rapist;
					part.needs.mood.thoughts.memories.TryGainMemory(part_thought_about_rapist, pawn);
				}
			}

			//--Log.Message("[ABF]JobDriver_RapeEnemy::think_after_sex( " + pawn.NameStringShort + ", " + part.NameStringShort + ", " + violent + " ) - setting disease thoughts");
			// check for visible diseases
			if (xxx.is_human(pawn) && xxx.is_human(part))
			{
				// Add negative relation for visible diseases on the genitals
				var pawn_rash_severity = std.genital_rash_severity(pawn) - std.genital_rash_severity(part);
				ThoughtDef pawn_thought_about_rash = null;
				if (pawn_rash_severity == 1) pawn_thought_about_rash = xxx.saw_rash_1;
				else if (pawn_rash_severity == 2) pawn_thought_about_rash = xxx.saw_rash_2;
				else if (pawn_rash_severity >= 3) pawn_thought_about_rash = xxx.saw_rash_3;
				if (pawn_thought_about_rash != null)
				{
					var memory = (Thought_Memory)ThoughtMaker.MakeThought(pawn_thought_about_rash);
					pawn.needs.mood.thoughts.memories.TryGainMemory(memory, part);
				}

				if (!part.Dead && !isCoreLovin)
				{
					var part_rash_severity = std.genital_rash_severity(part) - std.genital_rash_severity(pawn);
					ThoughtDef part_thought_about_rash = null;
					if (part_rash_severity == 1) part_thought_about_rash = xxx.saw_rash_1;
					else if (part_rash_severity == 2) part_thought_about_rash = xxx.saw_rash_2;
					else if (part_rash_severity >= 3) part_thought_about_rash = xxx.saw_rash_3;
					if (part_thought_about_rash != null)
					{
						var memory = (Thought_Memory)ThoughtMaker.MakeThought(part_thought_about_rash);
						part.needs.mood.thoughts.memories.TryGainMemory(memory, pawn);
					}
					if (!violent)
					{
						var memory = (Thought_Memory)ThoughtMaker.MakeThought(ThoughtDefOf.GotSomeLovin);
						pawn.needs.mood.thoughts.memories.TryGainMemory(memory, part);
						part.needs.mood.thoughts.memories.TryGainMemory(memory, pawn);
					}
				}
			}
		}
	}
}