using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	public class ThinkNode_ConditionalTrait : ThinkNode_Conditional
	{
		private TraitDef trait;
		public override ThinkNode DeepCopy(bool resolve = true)
		{
			ThinkNode_ConditionalTrait thinkNode_ConditionalTrait = (ThinkNode_ConditionalTrait)base.DeepCopy(resolve);
			thinkNode_ConditionalTrait.trait = this.trait;
			return thinkNode_ConditionalTrait;
		}
		protected override bool Satisfied(Pawn pawn)
		{
			if (trait == null)
			if (pawn.story != null)
			{
				Log.Message(pawn.NameStringShort+" has trait" + this.trait.defName + ":" + pawn.story.traits.HasTrait(this.trait));
				return pawn.story.traits.HasTrait(this.trait);
			}
			return false;
		}
	}
}
