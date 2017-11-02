
using Verse;
using RimWorld;

namespace rjw {
	public class Designator_ComfortPrisoner : Designator {
        private static readonly MiscTranslationDef MTdef = DefDatabase<MiscTranslationDef>.GetNamedSilentFail("DesignatorComfortPrisoner");

		
		public Designator_ComfortPrisoner ()
		{
			defaultLabel = MTdef.label;
			defaultDesc = MTdef.description;

			icon = comfort_prisoner_invisible_tex.gizmo;

			// TODO: Can this be null?
			hotKey = KeyBindingDefOf.Misc12;
			
			// These don't matter but set them just in case
			soundDragSustain = SoundDefOf.DesignateDragStandard;
			soundDragChanged = SoundDefOf.DesignateDragStandardChanged;
			useMouseIcon = false;
			soundSucceeded = SoundDefOf.DesignateClaim;			
		}
		
		public override AcceptanceReport CanDesignateCell (IntVec3 c) { return false; }
		
		public override void DesignateSingleCell (IntVec3 c) { }
		
		public override AcceptanceReport CanDesignateThing (Thing t)
		{
            var p = t as Pawn;
			//return (p != null) && (p.IsPrisonerOfColony || p.IsColonist || p.Faction == Faction.OfPlayer) && xxx.can_get_raped(p) && (!comfort_prisoners.is_designated(p));
			return (p != null) && (p.IsPrisonerOfColony) && (!comfort_prisoners.is_designated(p)); //comfor prisoner button will only appear on prisoners
		}
		
		public override void DesignateThing (Thing t)
		{
            DesignationDef designation_def = comfort_prisoners.designation_def_no_sticky;
            if (xxx.config.rape_me_sticky_enabled) {
                designation_def = comfort_prisoners.designation_def;
				//comfort_prisoners.designation_def = comfort_prisoners.designation_def_no_sticky;
            }

            base.Map.designationManager.AddDesignation (new Designation (t, designation_def));
		}
	}
}
