using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JyGein.Elestrals
{
    internal class RandomDroneMoveLocaleFix
    {
        public static Elestrals Instance => Elestrals.Instance;
        public static void ApplyPatches(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(DB), nameof(DB.SetLocale)),
                postfix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(DB_SetLocale_Postfix))
            );
        }

        private static void DB_SetLocale_Postfix(string locale, bool useHiRes)
        {
            DB.currentLocale.strings.TryAdd("action.droneMoveRandom.desc", Instance.Localizations.Localize(["action", "DroneMoveRandom", "description"]));
            DB.currentLocale.strings.TryAdd("action.droneMoveRandom.name", Instance.Localizations.Localize(["action", "DroneMoveRandom", "name"]));
        }
    }
}
