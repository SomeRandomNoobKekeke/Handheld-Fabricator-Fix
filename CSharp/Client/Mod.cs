using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Barotrauma.Items.Components;
using Barotrauma.Extensions;

namespace HandheldFabricatorFix
{
  public partial class Mod : IAssemblyPlugin
  {
    public Harmony harmony;

    public void Initialize()
    {
      harmony = new Harmony("fabricator.fix");

      PatchAll();
    }

    public void PatchAll()
    {
      harmony.Patch(
        original: typeof(Fabricator).GetMethod("UpdateHUDComponentSpecific", AccessTools.all),
        prefix: new HarmonyMethod(typeof(Mod).GetMethod("UpdateHUDComponentSpecific"))
      );
    }


    public static void log(object msg, Color? cl = null)
    {
      cl ??= Color.Cyan;
      LuaCsLogger.LogMessage($"{msg ?? "null"}", cl * 0.8f, cl);
    }

    public void OnLoadCompleted() { }
    public void PreInitPatching() { }
    public void Dispose() { }
  }
}