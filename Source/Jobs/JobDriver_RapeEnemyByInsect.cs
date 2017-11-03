

using Verse;

namespace rjw
{
    class JobDriver_RapeEnemyByInsect : JobDriver_RapeEnemy
    {

        public override void aftersex(Pawn pawn, Pawn part, bool violent = false, bool isCoreLovin = false, bool isAnalSex = false)
        {
            base.aftersex(pawn,part,violent,isCoreLovin,isAnalSex);
            /*if (pawn.RaceProps.Animal && xxx.is_human(part))
            {
                if (pawn.def == ThingDef.Named("Megascarab"))
                {
                    Log.Message("[ABF]AddMechAfterSex::Pretfix() - Insects raped humans");
                    if (isAnalSex)
                    {
                        ImplantToAnal("InsectEggs", part);
                    }
                    else {
                        ImplantToGenital("InsectEggs", part);
                    }
                }
                if (pawn.def == ThingDef.Named("Megaspider"))
                {
                    Log.Message("[ABF]AddMechAfterSex::Pretfix() - Insects raped humans");
                    if (isAnalSex)
                    {
                        ImplantToAnal("InsectEggs", part);
                    }
                    else {
                        ImplantToGenital("InsectEggs", part);
                    }
                }
                if (pawn.def == ThingDef.Named("Spelopede"))
                {
                    Log.Message("[ABF]AddMechAfterSex::Pretfix() - Insects raped humans");
                    if (isAnalSex)
                    {
                        ImplantToAnal("InsectEggs", part);
                    }
                    else {
                        ImplantToGenital("InsectEggs", part);
                    }
                }
            }*/
        }
    }
}
