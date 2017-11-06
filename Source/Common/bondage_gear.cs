
using System;
using System.Linq;
using System.Collections.Generic;

using Verse;
using Verse.AI;
using RimWorld;

namespace rjw
{

	public static class bondage_gear_tradeability
	{
		public static void init()
		{
			// Prevents bondage gear from appearing on traders if it is disabled in the config
			if (!xxx.config.bondage_gear_enabled)
			{
				foreach (var def in DefDatabase<bondage_gear_def>.AllDefs)
					if (def.tradeability == Tradeability.Stockable)
						def.tradeability = Tradeability.Sellable;
			}
		}
	}

	public static class bondage_gear_extensions
	{
		public static bool has_lock(this Apparel app)
		{
			return (app.TryGetComp<CompHoloCryptoStamped>() != null);
		}

		public static bool is_wearing_locked_apparel(this Pawn p)
		{
			if (p.apparel != null)
				foreach (var app in p.apparel.WornApparel)
					if (app.has_lock())
						return true;
			return false;
		}

		// Tries to get p started on the job of using an item on either another pawn or on themself (if "other" is null).
		// Of course in order for this method to work, the item's useJob has to be able to handle use on another pawn. This
		// is true for the holokey and bondage gear in RJW but not the items in the core game
		public static void start_job(this CompUsable usa, Pawn p, LocalTargetInfo tar)
		{
			if (p.CanReserveAndReach(usa.parent, PathEndMode.Touch, Danger.Deadly) &&
				((tar == null) || p.CanReserveAndReach(tar, PathEndMode.Touch, Danger.Deadly)))
			{
				var comfor = usa.parent.GetComp<CompForbiddable>();
				if (comfor != null)
					comfor.Forbidden = false;
				var job = new Job(((CompProperties_Usable)usa.props).useJob, usa.parent, tar);
				p.jobs.TryTakeOrderedJob(job);
			}
		}

		// Creates a menu option to use an item. "tar" is expected to be a pawn, corpse or null if it doesn't apply (in which
		// case the pawn will presumably use the item on themself). "required_work" can also be null.
		public static FloatMenuOption make_option(this CompUsable usa, string label, Pawn p, LocalTargetInfo tar, WorkTypeDef required_work)
		{
			if ((tar != null) && (!p.CanReserve(tar)))
				return new FloatMenuOption(label + " (" + "Reserved".Translate() + ")", null, MenuOptionPriority.DisabledOption);

			else if ((tar != null) && (!p.CanReach(tar, PathEndMode.Touch, Danger.Deadly)))
				return new FloatMenuOption(label + " (" + "NoPath".Translate() + ")", null, MenuOptionPriority.DisabledOption);

			else if ((required_work != null) && p.story.WorkTypeIsDisabled(required_work))
				return new FloatMenuOption(label + " (" + "CannotPrioritizeWorkTypeDisabled".Translate(new object[] { required_work.gerundLabel }) + ")", null, MenuOptionPriority.DisabledOption);

			else
				return new FloatMenuOption(
					label,
					delegate
					{
						usa.start_job(p, tar);
					},
					MenuOptionPriority.Default);
		}
	}

	public class bondage_gear_def : ThingDef
	{
		public Type soul_type;
		public HediffDef equipped_hediff = null;
		public bool gives_bound_moodlet = false;
		public bool gives_gagged_moodlet = false;
		public bool blocks_genitals = false;
		public bool blocks_anus = false;
		public bool blocks_breasts = false;
		private bondage_gear_soul soul_ins = null;

		public bondage_gear_soul soul
		{
			get
			{
				if (soul_ins == null)
					soul_ins = (bondage_gear_soul)Activator.CreateInstance(soul_type);
				return soul_ins;
			}
		}
	}

	public class bondage_gear_soul
	{
		// Adds the bondage gear's associated HediffDef and spawns a matching holokey
		public virtual void on_wear(Pawn wearer, Apparel gear)
		{
			var def = (bondage_gear_def)gear.def;

			if (def.equipped_hediff != null)
				wearer.health.AddHediff(def.equipped_hediff);

			var gear_stamp = gear.TryGetComp<CompHoloCryptoStamped>();
			if (gear_stamp != null)
			{
				var key = ThingMaker.MakeThing(xxx.holokey);
				var key_stamp = key.TryGetComp<CompHoloCryptoStamped>();
				key_stamp.copy_stamp_from(gear_stamp);
				if (wearer.Map != null)
					GenSpawn.Spawn(key, wearer.Position, wearer.Map);
				else
					wearer.inventory.TryAddItemNotForSale(key);
			}
		}

		// Removes the gear's HediffDef
		public virtual void on_remove(Apparel gear, Pawn former_wearer)
		{
			var def = (bondage_gear_def)gear.def;
			if (def.equipped_hediff != null)
			{
				var hed = former_wearer.health.hediffSet.GetFirstHediffOfDef(def.equipped_hediff);
				if (hed != null)
					former_wearer.health.RemoveHediff(hed);
			}
		}
	}

	// Give bondage gear an extremely low score when it's not being worn so pawns never equip it on themselves and give
	// it an extremely high score when it is being worn so pawns never try to take it off to equip something "better".
	public class bondage_gear : Apparel
	{
		public override float GetSpecialApparelScoreOffset()
		{
			return (Wearer == null) ? -1e5f : 1e5f;
		}
	}

	public class armbinder : bondage_gear
	{
		// Prevents pawns in armbinders from melee attacking
		//public override bool AllowVerbCast (IntVec3 root, TargetInfo targ)
		//{
		//	return false;
		//}
	}

	public class force_off_gear_def : RecipeDef
	{
		public ThingDef removes_apparel;
		public BodyPartDef failure_affects;
		public List<BodyPartDef> destroys_one_of = null;
		public List<BodyPartDef> major_burns_on = null;
		public List<BodyPartDef> minor_burns_on = null;
	}

}
