﻿
using System;
using System.Linq;
using System.Collections.Generic;

using Verse;
using RimWorld;

namespace rjw {
	public class CompHoloCryptoStamped : ThingComp {
		public string name;
		public string key;

		public string random_hex_byte ()
		{
			var rv = Rand.RangeInclusive (0x00, 0xFF);
			var padding = (rv < 0x10) ? "0" : "";
			return padding + rv.ToString ("X");
		}

		public override void Initialize (CompProperties pro)
		{
			name = NameGenerator.GenerateName (RulePackDef.Named ("EngravedName"));
			key = "";
			for (int i = 0; i < 16; ++i)
				key += random_hex_byte ();
		}

		public override void PostExposeData ()
		{
			base.PostExposeData ();
			Scribe_Values.Look<string> (ref name, "engraved_name");
			Scribe_Values.Look<string> (ref key, "cryptostamp");
		}

		public override bool AllowStackWith (Thing t)
		{
			return false;
		}

		public override string CompInspectStringExtra ()
		{
			var inspect_engraving = "Engraved with the name \"" + name + "\"";
			var inspect_key = "Cryptostamp: " + key;
			return base.CompInspectStringExtra () + inspect_engraving + "\n" + inspect_key;
		}

		public override string TransformLabel (string lab)
		{
			return lab + " \"" + name + "\"";
		}

		public bool matches (CompHoloCryptoStamped other)
		{
			return String.Equals (key, other.key);
		}

		public void copy_stamp_from (CompHoloCryptoStamped other)
		{
			name = other.name;
			key = other.key;
		}
	}

	public class CompProperties_HoloCryptoStamped : CompProperties {
		public CompProperties_HoloCryptoStamped ()
		{
			compClass = typeof(CompHoloCryptoStamped);
		}
	}
}
