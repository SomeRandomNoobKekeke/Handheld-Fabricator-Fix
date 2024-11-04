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
    public static bool UpdateHUDComponentSpecific(Character character, float deltaTime, Camera cam, Fabricator __instance)
    {
      log("Guh");

      Fabricator _ = __instance;

      _.activateButton.Enabled = false;
      _.inSufficientPowerWarning.Visible = _.IsActive && !_.hasPower;

      _.ingredientHighlightTimer -= deltaTime;

      if (!_.IsActive)
      {
        if (_.selectedItem != null && _.displayingForCharacter != character)
        {
          //reselect to recreate the info based on the new user's skills
          _.SelectItem(character, _.selectedItem);
        }

        //only check ingredients if the fabricator isn't active (if it is, this is done in Update)
        if (_.refreshIngredientsTimer <= 0.0f)
        {
          _.RefreshAvailableIngredients();
          _.refreshIngredientsTimer = Fabricator.RefreshIngredientsInterval;
        }
        _.refreshIngredientsTimer -= deltaTime;
      }

      if (character != null)
      {
        foreach (GUIComponent child in _.itemList.Content.Children)
        {
          if (child.UserData is not FabricationRecipe recipe) { continue; }

          if (recipe != _.selectedItem &&
              (child.Rect.Y > _.itemList.Rect.Bottom || child.Rect.Bottom < _.itemList.Rect.Y))
          {
            continue;
          }

          bool canBeFabricated = _.CanBeFabricated(recipe, _.availableIngredients, character);
          if (recipe == _.selectedItem)
          {
            _.activateButton.Enabled = canBeFabricated;
          }

          var childContainer = child.GetChild<GUILayoutGroup>();
          childContainer.GetChild<GUITextBlock>().TextColor = Color.White * (canBeFabricated ? 1.0f : 0.5f);
          childContainer.GetChild<GUIImage>().Color = recipe.TargetItem.InventoryIconColor * (canBeFabricated ? 1.0f : 0.5f);

          var limitReachedText = child.FindChild(nameof(_.FabricationLimitReachedText));
          limitReachedText.Visible = !canBeFabricated && _.fabricationLimits.TryGetValue(recipe.RecipeHash, out int amount) && amount <= 0;
        }
      }

      return false; // = skip original
    }
  }
}