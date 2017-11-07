using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RimWorld;
using Verse;
using Harmony;

namespace rjw
{
	public enum RandomDialogueJobType
	{
		Rape = 0,
		Love = 1,
		Other = 2
	}
	public sealed class RandomDialoguesDef : Def
	{
		public RandomDialogueJobType JobType;
		public string RD0;
		public string RD1;
		public string RD2;
		public string RD3;
		public string RD4;
		public string RD5;
		public string RD6;
		public string RD7;
		public string RD8;
		public string RD9;
		public string RD10;
		public string RD11;
		public string RD12;
		public string RD13;
		public string RD14;
		public string RD15;
		public string RD16;
		public string RD17;
		public string RD18;
		public string RD19;
	}

	/* TO BE FINISHED
    [DefOf]
    public static class RandomDialoguesDefOf
    {
        //0 means before, 1 means during, 2 means after
        //public readonly static RandomDialoguesDef rape0;
        //public readonly static RandomDialoguesDef rape1;
        //public readonly static RandomDialoguesDef rape2;

        //public readonly static RandomDialoguesDef love0;
        //public readonly static RandomDialoguesDef love1;
        //public readonly static RandomDialoguesDef love2;
        
        //public readonly static RandomDialoguesDef other0;
        //public readonly static RandomDialoguesDef other1;
        //public readonly static RandomDialoguesDef other2;
    }
    */

	// TO BE FINISHED
	/*
    public static class RandomDialogues
    {
        public static void processBeforeDialogue(Pawn psubject,Pawn pobject,int Type = 4)
        {
            switch (Type)
            {
                case 0:
                    string traitDefName;
                    if (xxx.has_traits(psubject)&&Rand.Value<0.25){
                        traitDefName = psubject.random_pick_a_trait();
                        switch (traitDefName)
                        {
                            case "nymphomaniac":
                                break;
                            case "rapist":
                                break;
                            case "necrophiliac":
                                break;
                            case "zoophiliac":
                                break;
                            case "Bloodlust":
                                break;
                            case "Psychopath":
                                break;
                            case "Cannibal":
                                break;
                            case "masochist":
                                break;
                            case "Prosthophile":
                                break;
                            case "Prosthophobe":
                                break;
                            case "Jealous":
                                break;
                            case "Gay":
                                break;
                            default:
                                break;
                        }
                    }
                    if (xxx.has_traits(pobject))
                    {

                    }
                    return;
                case 1:
                    return;
                case 2:
                    return;
                default:
                    Log.Error("[RJW]RandomDialogues::processBeforeDialogue - Type not found");
                    return;
            }
        }

        public static void processDuringDialogue(Pawn psubject, Pawn pobject, int Type = 4)
        {
            switch (Type)
            {
                case 0:
                    return;
                case 1:
                    return;
                case 2:
                    return;
                default:
                    Log.Error("[RJW]RandomDialogues::processBeforeDialogue - Type not found");
                    return;
            }

        }

        public static void processAfterDialogue(Pawn psubject, Pawn pobject, int Type = 4)
        {
            switch (Type)
            {
                case 0:
                    return;
                case 1:
                    return;
                case 2:
                    return;
                default:
                    Log.Error("[RJW]RandomDialogues::processBeforeDialogue - Type not found");
                    return;
            }

        }
    }
    */
}
