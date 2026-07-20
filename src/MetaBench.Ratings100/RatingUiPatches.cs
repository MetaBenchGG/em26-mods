using System;
using TMPro;

namespace MetaBench.Ratings100;

internal static class RatingUiPatches
{
    internal static void SquadPlayerRowPostfix(SquadPlayerRow __instance)
    {
        if (__instance == null || __instance._player == null)
        {
            return;
        }

        var player = __instance._player;
        RatingTextOverlay.Show(__instance._rating, player.Rating, __instance._nick);
        RatingTextOverlay.Show(__instance._potential, player.Potential, __instance._nick, isPotential: true);
    }

    internal static void SquadStaffRowPostfix(SquadStaffRow __instance)
    {
        if (__instance != null && __instance._player != null)
        {
            var staff = __instance._player;
            RatingTextOverlay.Show(__instance._rating, staff.Rating, __instance._nick);
        }
    }

    internal static void PlayerOverviewPostfix(PlayerOverview __instance)
    {
        if (__instance == null || __instance._player == null)
        {
            return;
        }

        var player = __instance._player;
        RatingTextOverlay.Show(__instance._rating, player.Rating, __instance._nameTxt);
        RatingTextOverlay.Show(__instance._potential, player.Potential, __instance._nameTxt, isPotential: true);
    }

    internal static void StaffOverviewPostfix(StaffOverview __instance)
    {
        if (__instance != null && __instance._staff != null)
        {
            var staff = __instance._staff;
            RatingTextOverlay.Show(__instance._rating, staff.Rating, __instance._nameText);
        }
    }

    internal static void ScoutingRowPostfix(ScoutingRow __instance)
    {
        if (__instance == null || __instance._user == null)
        {
            return;
        }

        var user = __instance._user;
        RatingTextOverlay.Show(__instance._rating, user.Rating, __instance._nick);

        var player = user.TryCast<DataPlayer>();
        if (player != null)
        {
            RatingTextOverlay.Show(__instance._potential, player.Potential, __instance._nick, isPotential: true);
        }
    }

    internal static void TransferListMemberRowPostfix(TransferListMemberRow __instance)
    {
        if (__instance != null && __instance._user != null)
        {
            var user = __instance._user;
            RatingTextOverlay.Show(__instance._rating, user.Rating, __instance._nick);
        }
    }

    internal static void TransferRowPostfix(TransferRow __instance)
    {
        if (__instance != null && __instance._user != null)
        {
            var user = __instance._user;
            RatingTextOverlay.Show(__instance._rating, user.Rating, __instance._nick);
        }
    }

    internal static void OrganizationCeoPostfix(OrganizationCEOView __instance, DataStaff ceo)
    {
        if (__instance != null && ceo != null)
        {
            RatingTextOverlay.Show(__instance._rating, ceo.Rating, __instance._name);
        }
    }

    internal static void MemberItemPostfix(MemberItemView __instance, float rating)
    {
        if (__instance != null)
        {
            RatingTextOverlay.Show(__instance._rating, rating, __instance._nick);
        }
    }

    internal static void HomePlayerRowPostfix(HomePlayerRow __instance, int rating)
    {
        if (__instance?._dpcTxt != null)
        {
            __instance._dpcTxt.text = RatingScale.To100(rating).ToString();
        }
    }

    internal static void LegacyScoutingPostfix(PlayerInScouting __instance)
    {
        if (__instance == null)
        {
            return;
        }

        Scale(__instance.skill);
        Scale(__instance.awp);
        Scale(__instance.rifle);
        Scale(__instance.pistol);
        Scale(__instance.reaction);
        Scale(__instance.leader);
        Scale(__instance.creativity);
        Scale(__instance.teamwork);
        Scale(__instance.tactic);
        Scale(__instance.loyalty);
        Scale(__instance.rating);
    }

    internal static void AttributeLinePostfix(AttributeLine __instance)
    {
        Scale(__instance?._textBox);
    }

    internal static void AttributeTrainingShortPostfix(AttributeLineTrainingShort __instance)
    {
        if (__instance == null)
        {
            return;
        }

        Scale(__instance._value);
        Scale(__instance._changeMonth);
    }

    internal static void AttributeTrainingPopupPostfix(AttributeLineTrainingPopup __instance)
    {
        if (__instance == null)
        {
            return;
        }

        Scale(__instance._currentValue);
        Scale(__instance._changePerMonth);
    }

    internal static void AttributeComparisonPostfix(AttributeComparsion __instance)
    {
        Scale(__instance?._lastNumber);
    }

    private static void Scale(TMP_Text? text)
    {
        if (text != null)
        {
            text.text = RatingScale.ScaleRenderedText(text.text);
        }
    }
}
