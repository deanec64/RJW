﻿
using System;
using System.Collections.Generic;

using Verse;
using RimWorld;

namespace rjw {
	public class Hediff_RegularPrivatesImplant : Hediff_Implant {

		public override bool Visible {
			get {
				return xxx.config.show_regular_dick_and_vag;
			}
		}
		
	}
}
