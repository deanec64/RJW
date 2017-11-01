using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using RimWorld;

namespace rjw
{
    public class LordJob_RapeColonists : LordJob
    {
        private Faction assaulterFaction;

        private bool canKidnap = true;

        private bool canTimeoutOrFlee = true;

        private bool useAvoidGridSmart;

        private bool canSteal = true;

        private static readonly IntRange RapeTimeBeforeGiveUp = new IntRange(26000, 38000);

        public LordJob_RapeColonists()
        {
        }
        public LordJob_RapeColonists(Faction assaulterFaction, bool canKidnap = true, bool canTimeoutOrFlee = true, bool useAvoidGridSmart = false, bool canSteal = true)
        {
            this.assaulterFaction = assaulterFaction;
            this.canKidnap = canKidnap;
            this.canTimeoutOrFlee = canTimeoutOrFlee;
            this.useAvoidGridSmart = useAvoidGridSmart;
            this.canSteal = canSteal;
        }
        public override StateGraph CreateGraph()
        {
            var rapers = this.lord.ownedPawns;
            StateGraph stateGraph = new StateGraph();
            LordToil lordToil = null;
            LordToil lordToil3 = new LordToil_AssaultColony();
            if (this.useAvoidGridSmart)
            {
                lordToil3.avoidGridMode = AvoidGridMode.Smart;
            }
            stateGraph.AddToil(lordToil3);
            LordToil_ExitMap lordToil_ExitMap = new LordToil_ExitMap(LocomotionUrgency.Jog, false);
            lordToil_ExitMap.avoidGridMode = AvoidGridMode.Smart;
            stateGraph.AddToil(lordToil_ExitMap);
            if (this.assaulterFaction.def.humanlikeFaction)
            {
                if (this.canTimeoutOrFlee)
                {
                    Transition transition4 = new Transition(lordToil3, lordToil_ExitMap);
                    if (lordToil != null)
                    {
                        transition4.AddSource(lordToil);
                    }
                    transition4.AddTrigger(new Trigger_TicksPassed(LordJob_RapeColonists.RapeTimeBeforeGiveUp.RandomInRange));
                    transition4.AddPreAction(new TransitionAction_Message("MessageRaidersSatisfiedLeaving".Translate(new object[]
                    {
                        this.assaulterFaction.def.pawnsPlural.CapitalizeFirst(),
                        this.assaulterFaction.Name
                    })));
                    stateGraph.AddTransition(transition4);
                }
                if (this.canKidnap)
                {
                    LordToil startingToil = stateGraph.AttachSubgraph(new LordJob_Kidnap().CreateGraph()).StartingToil;
                    Transition transition6 = new Transition(lordToil3, startingToil);
                    if (lordToil != null)
                    {
                        transition6.AddSource(lordToil);
                    }
                    transition6.AddPreAction(new TransitionAction_Message("MessageRaidersKidnapping".Translate(new object[]
                    {
                        this.assaulterFaction.def.pawnsPlural.CapitalizeFirst(),
                        this.assaulterFaction.Name
                    })));
                    transition6.AddTrigger(new Trigger_KidnapVictimPresent());
                    stateGraph.AddTransition(transition6);
                }
                if (this.canSteal)
                {
                    LordToil startingToil2 = stateGraph.AttachSubgraph(new LordJob_Steal().CreateGraph()).StartingToil;
                    Transition transition7 = new Transition(lordToil3, startingToil2);
                    if (lordToil != null)
                    {
                        transition7.AddSource(lordToil);
                    }
                    transition7.AddPreAction(new TransitionAction_Message("MessageRaidersStealing".Translate(new object[]
                    {
                        this.assaulterFaction.def.pawnsPlural.CapitalizeFirst(),
                        this.assaulterFaction.Name
                    })));
                    transition7.AddTrigger(new Trigger_HighValueThingsAround());
                    stateGraph.AddTransition(transition7);
                }
            }
            Transition transition8 = new Transition(lordToil3, lordToil_ExitMap);
            if (lordToil != null)
            {
                transition8.AddSource(lordToil);
            }
            transition8.AddTrigger(new Trigger_BecameColonyAlly());
            transition8.AddPreAction(new TransitionAction_Message("MessageRaidersLeaving".Translate(new object[]
            {
                this.assaulterFaction.def.pawnsPlural.CapitalizeFirst(),
                this.assaulterFaction.Name
            })));
            stateGraph.AddTransition(transition8);
            return stateGraph;
        }
    }
}
