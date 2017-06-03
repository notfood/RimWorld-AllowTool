﻿using HugsLib.Utils;
using RimWorld;
using Verse;

namespace AllowTool {
	/// <summary>
	/// Designates things for urgent hauling.
	/// </summary>
	public class Designator_HaulUrgently : Designator_SelectableThings {
		public Designator_HaulUrgently(ThingDesignatorDef def)
			: base(def) {
		}

		protected override void FinalizeDesignationSucceeded() {
			base.FinalizeDesignationSucceeded();
			if (HugsLibUtility.ShiftIsHeld) {
				foreach (var colonist in Find.VisibleMap.mapPawns.FreeColonists) {
					colonist.jobs.CheckForJobOverride();
				}
			}
		}

		public override AcceptanceReport CanDesignateThing(Thing t) {
			return ThingIsRelevant(t) && !t.HasDesignation(AllowToolDefOf.HaulUgentlyDesignation);
		}

		public override void DesignateSingleCell(IntVec3 cell) {
			var map = Find.VisibleMap;
			numThingsDesignated = 0;

			var cellThings = map.thingGrid.ThingsListAt(cell);
			for (var i = 0; i < cellThings.Count; i++) {
				var thing = cellThings[i];
				if (!ThingIsRelevant(thing)) continue;
				numThingsDesignated++;
				DesignateThing(thing);
			}
		}

		public override void DesignateThing(Thing thing) {
			if (thing.def.designateHaulable) {
				// for things that require explicit hauling desingation, such as rock chunks
				thing.ToggleDesignation(DesignationDefOf.Haul, true);
			}
			thing.ToggleDesignation(AllowToolDefOf.HaulUgentlyDesignation, true);
			// unforbid for convenience
			thing.SetForbidden(false, false);
		}

		private bool ThingIsRelevant(Thing thing) {
			if (thing.def == null || thing.Position.Fogged(thing.Map)) return false;
			return (thing.def.alwaysHaulable || thing.def.EverHaulable) && !thing.IsInValidBestStorage();
		}
	}
}