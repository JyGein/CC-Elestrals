using HarmonyLib;
using JyGein.Elestrals.ExternalAPI;
using Microsoft.Extensions.Logging;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JyGein.Elestrals;

internal sealed class DuoApis
{
    internal bool RegisterDuos = true;
    internal IDuoArtifactsApi? DuoArtifactsApi;
    internal IDynaApi? dynaApi;
    internal EchoesOfTheFutureSudoApi? echoesOfTheFutureApi;
    internal RandallSudoApi? randallApi;
    internal DaveSudoApi? daveApi;
    internal RuhigSudoApi? ruhigApi;
    internal IJackApi? jackApi;
    internal ICleoApi? cleoApi;
    internal JesterSudoApi? jesterApi;
    internal IDestinyApi? destinyApi;
    internal IBucketApi? bucketApi;
    internal TwosCompanySudoApi? twosCompanyApi;
    public DuoApis(IModHelper helper)
    {
        DuoArtifactsApi = helper.ModRegistry.GetApi<IDuoArtifactsApi>("Shockah.DuoArtifacts");
        if (DuoArtifactsApi == null) RegisterDuos = false;
        dynaApi = helper.ModRegistry.GetApi<IDynaApi>("Shockah.Dyna");
        echoesOfTheFutureApi = EchoesOfTheFutureSudoApi.TryLoadMod(helper);
        randallApi = RandallSudoApi.TryLoadMod(helper);
        daveApi = DaveSudoApi.TryLoadMod(helper);
        ruhigApi = RuhigSudoApi.TryLoadMod(helper);
        jackApi = helper.ModRegistry.GetApi<IJackApi>("Jack");
        cleoApi = helper.ModRegistry.GetApi<ICleoApi>("Flipbop.Cleo");
        jesterApi = JesterSudoApi.TryLoadMod(helper);
        destinyApi = helper.ModRegistry.GetApi<IDestinyApi>("Shockah.Destiny");
        bucketApi = helper.ModRegistry.GetApi<IBucketApi>("TheJazMaster.Bucket");
        twosCompanyApi = TwosCompanySudoApi.TryLoadMod(helper);
    }
}

internal sealed class EchoesOfTheFutureSudoApi
{
    internal IStatusEntry AngderIsMissing = null!;
    internal ICardEntry Fireball = null!;
    internal IDeckEntry AngderDeck = null!;
    internal IDeckEntry D26Deck = null!;
    internal IDeckEntry GrunanDeck = null!;
    internal IDeckEntry KobretteDeck = null!;
    public static EchoesOfTheFutureSudoApi? TryLoadMod(IModHelper helper)
    {
        EchoesOfTheFutureSudoApi mod = new();
        string EchoesOfTheFuture = "Angder.EchoesOfTheFuture";
        IDeckEntry? potentialAndgerDeck = helper.Content.Decks.LookupByUniqueName($"{EchoesOfTheFuture}::AngderDeck");
        if (potentialAndgerDeck == null) return null;
        mod.AngderDeck = potentialAndgerDeck;
        mod.D26Deck = helper.Content.Decks.LookupByUniqueName($"{EchoesOfTheFuture}::ButlerDeck")!;
        mod.GrunanDeck = helper.Content.Decks.LookupByUniqueName($"{EchoesOfTheFuture}::GrunanDeck")!;
        mod.KobretteDeck = helper.Content.Decks.LookupByUniqueName($"{EchoesOfTheFuture}::KobretteDeck")!;
        mod.AngderIsMissing = helper.Content.Characters.V2.LookupByDeck(potentialAndgerDeck.Deck)!.MissingStatus;
        mod.Fireball = helper.Content.Cards.LookupByUniqueName($"{EchoesOfTheFuture}::Fireball")!;
        return mod;
    }
}

internal sealed class RandallSudoApi
{
    internal IDeckEntry RandallDeck = null!;
    internal ICardTraitEntry Synergy = null!;
    public static RandallSudoApi? TryLoadMod(IModHelper helper)
    {
        RandallSudoApi mod = new();
        string RandallMod = "Arin.Randall";
        IDeckEntry? potentialRandallDeck = helper.Content.Decks.LookupByUniqueName($"{RandallMod}::Randall");
        if (potentialRandallDeck == null) return null;
        mod.RandallDeck = potentialRandallDeck;
        mod.Synergy = helper.Content.Cards.LookupTraitByUniqueName($"{RandallMod}::Synergized")!;
        return mod;
    }
}

internal sealed class DaveSudoApi
{
    internal IDeckEntry DaveDeck = null!;
    public static DaveSudoApi? TryLoadMod(IModHelper helper)
    {
        DaveSudoApi mod = new();
        string DaveMod = "Dave";
        IDeckEntry? potentialDaveDeck = helper.Content.Decks.LookupByUniqueName($"{DaveMod}::Dave");
        if (potentialDaveDeck == null) return null;
        mod.DaveDeck = potentialDaveDeck;
        return mod;
    }
}

internal sealed class RuhigSudoApi
{
    internal IDeckEntry RuhigDeck = null!;
    public static RuhigSudoApi? TryLoadMod(IModHelper helper)
    {
        RuhigSudoApi mod = new();
        string RuhigMod = "havmir.RuhigMod";
        IDeckEntry? potentialRuhigDeck = helper.Content.Decks.LookupByUniqueName($"{RuhigMod}::Demo");
        if (potentialRuhigDeck == null) return null;
        mod.RuhigDeck = potentialRuhigDeck;
        return mod;
    }
}

internal sealed class JesterSudoApi
{
    internal IDeckEntry JesterDeck = null!;
    public static JesterSudoApi? TryLoadMod(IModHelper helper)
    {
        JesterSudoApi mod = new();
        string JesterMod = "rft.Jester";
        IDeckEntry? potentialJesterDeck = helper.Content.Decks.LookupByUniqueName($"{JesterMod}::rft.Jester.JesterDeck");
        if (potentialJesterDeck == null) return null;
        mod.JesterDeck = potentialJesterDeck;
        return mod;
    }
}

internal sealed class TwosCompanySudoApi
{
    internal IDeckEntry NolaDeck = null!;
    internal IDeckEntry IsabelleDeck = null!;
    internal IDeckEntry IlyaDeck = null!;
    internal IDeckEntry JostDeck = null!;
    internal IDeckEntry GaussDeck = null!;
    internal IDeckEntry SorrelDeck = null!;
    internal IStatusEntry AutocurrentStatus = null!;
    internal IStatusEntry BullettimeStatus = null!;
    internal Type AForceEnemyAttack = null!;
    public static TwosCompanySudoApi? TryLoadMod(IModHelper helper)
    {
        TwosCompanySudoApi mod = new();
        string TwosCompanyMod = "Mezz.TwosCompany";
        IDeckEntry? potentialNolaDeck = helper.Content.Decks.LookupByUniqueName($"{TwosCompanyMod}::Mezz.TwosCompany.NolaDeck");
        if (potentialNolaDeck == null) return null;
        mod.NolaDeck = potentialNolaDeck;
        mod.IsabelleDeck = helper.Content.Decks.LookupByUniqueName($"{TwosCompanyMod}::Mezz.TwosCompany.IsabelleDeck")!;
        mod.IlyaDeck = helper.Content.Decks.LookupByUniqueName($"{TwosCompanyMod}::Mezz.TwosCompany.IlyaDeck")!;
        mod.JostDeck = helper.Content.Decks.LookupByUniqueName($"{TwosCompanyMod}::Mezz.TwosCompany.JostDeck")!;
        mod.GaussDeck = helper.Content.Decks.LookupByUniqueName($"{TwosCompanyMod}::Mezz.TwosCompany.GaussDeck")!;
        mod.SorrelDeck = helper.Content.Decks.LookupByUniqueName($"{TwosCompanyMod}::Mezz.TwosCompany.SorrelDeck")!;
        mod.AutocurrentStatus = helper.Content.Statuses.LookupByUniqueName($"{TwosCompanyMod}::Mezz.TwosCompany.Autocurrent")!;
        mod.BullettimeStatus = helper.Content.Statuses.LookupByUniqueName($"{TwosCompanyMod}::Mezz.TwosCompany.BulletTime")!;
        mod.AForceEnemyAttack = AccessTools.AllAssemblies().First(a => a.FullName?.Contains("TwosCompany") ?? false).GetTypes().First(t => t.Name == "AForceAttack");
        return mod;
    }
}
