
using System;
using System.Collections.Generic;

using Verse;
using Verse.AI;
using RimWorld;

namespace rjw
{
    public class ThinkNode_ConditionalWhore : ThinkNode_Conditional
    {

        protected override bool Satisfied(Pawn p)
        {
            return xxx.config.whore_beds_enabled && p.IsFreeColonist && xxx.is_whore(p) ;
        }

    }
}
