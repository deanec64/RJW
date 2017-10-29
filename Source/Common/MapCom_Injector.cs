
using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

using Verse;
using Verse.AI;
using RimWorld;

namespace rjw {
	public class MapCom_Injector : MapComponent {
		
		public bool injected_designator = false;
		
		public bool triggered_after_load = false;
		
		public MapCom_Injector (Map m) : base (m) {}
		
		public override void MapComponentUpdate () {}
		
		public override void MapComponentTick () {}
		
		public override void MapComponentOnGUI ()
		{
			if (! triggered_after_load) {
				triggered_after_load = true;
                Log.Message("[RJW]MapCom_Injector::triggered after load");
				
				if (Genital_Helper.pawns_require_sexualization())
                {
                    Log.Message("[RJW]MapCom_Injector::sexualize everyone");
                    Genital_Helper.sexualize_everyone();
                }
			}
			
			var currently_visible = Find.VisibleMap == map;
			
			if ((! injected_designator) && currently_visible) {
				Find.ReverseDesignatorDatabase.AllDesignators.Add (new Designator_ComfortPrisoner ());
				injected_designator = true;
				
			} else if (injected_designator && (! currently_visible))
				injected_designator = false;
		}
		
		public override void ExposeData () {}
		
	}
}
