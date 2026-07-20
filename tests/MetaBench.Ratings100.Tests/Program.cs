using System;
using MetaBench.Ratings100;

static void Equal<T>(T expected, T actual, string name)
{
    if (!Equals(expected, actual))
    {
        throw new InvalidOperationException($"{name}: expected '{expected}', got '{actual}'.");
    }
}

Equal(0, RatingScale.To100(0f), "zero");
Equal(5, RatingScale.To100(1f), "minimum normal rating");
Equal(75, RatingScale.To100(15f), "middle rating");
Equal(83, RatingScale.To100(16.5f), "half rating rounds away from zero");
Equal(100, RatingScale.To100(20f), "maximum rating");
Equal(100, RatingScale.To100(25f), "clamps high values");
Equal(80, RatingScale.PotentialTo100(0.8f), "normalized potential");
Equal(75, RatingScale.PotentialTo100(15f), "legacy 20-scale potential fallback");
Equal("75", RatingScale.ScaleRenderedText("15"), "plain attribute");
Equal("75 / 100", RatingScale.ScaleRenderedText("15 / 20"), "range");
Equal("<color=#00ff00>+3</color>", RatingScale.ScaleRenderedText("<color=#00ff00>+0.5</color>"), "rounded rich-text delta");
Equal("81", RatingScale.ScaleRenderedText("16.24"), "fractional attribute is rounded");
Equal("2029-05-12", RatingScale.ScaleRenderedText("2029-05-12"), "date is untouched");
Equal("84%", RatingScale.ScaleRenderedText("84%"), "large percentage is untouched");
Equal("?", RatingScale.ScaleRenderedText("?"), "hidden value");

foreach (RatingRole role in Enum.GetValues(typeof(RatingRole)))
{
    Equal(1f, MathF.Round(RoleRatingModel.GetWeightTotal(role), 4), $"{role} weights sum to one");
    Equal(80, RatingScale.To100(RoleRatingModel.Score(_ => 16f, role)), $"{role} uses the common scale");
}

var weakLeadership = new System.Collections.Generic.Dictionary<RatingAttribute, float>();
foreach (RatingAttribute attribute in Enum.GetValues(typeof(RatingAttribute)))
{
    weakLeadership[attribute] = 19f;
}
weakLeadership[RatingAttribute.Leader] = 4f;
weakLeadership[RatingAttribute.AWP] = 12f;

var supportRifler = RoleRatingModel.Calculate(a => weakLeadership[a], RatingRole.Rifler, RatingRole.Support);
Equal(95, RatingScale.To100(supportRifler), "support rifler ignores irrelevant leadership and AWP");

var awper = RoleRatingModel.Calculate(a => weakLeadership[a], RatingRole.AWPer);
Equal(true, RatingScale.To100(awper) < 90, "AWPer is penalized for weak AWP");

var igl = RoleRatingModel.Calculate(a => weakLeadership[a], RatingRole.Rifler, null, isIgl: true);
Equal(true, RatingScale.To100(igl) < RatingScale.To100(supportRifler), "IGL is penalized for weak leadership");

var zweihScreenshot = new System.Collections.Generic.Dictionary<RatingAttribute, float>
{
    [RatingAttribute.Skill] = 20f,
    [RatingAttribute.AWP] = 13.4f,
    [RatingAttribute.Rifle] = 20f,
    [RatingAttribute.Pistol] = 17.8f,
    [RatingAttribute.Grenades] = 17.4f,
    [RatingAttribute.Reaction] = 19.8f,
    [RatingAttribute.Eyesight] = 19.6f,
    [RatingAttribute.Strength] = 19f,
    [RatingAttribute.Endurance] = 19.4f,
    [RatingAttribute.Creativity] = 18.6f,
    [RatingAttribute.Clutch] = 19f,
    [RatingAttribute.Tactic] = 18.2f,
    [RatingAttribute.Leader] = 4f,
    [RatingAttribute.Teamwork] = 19.4f,
    [RatingAttribute.Productivity] = 20f,
    [RatingAttribute.StressResistance] = 19.2f
};
Equal(95, RatingScale.To100(RoleRatingModel.Calculate(a => zweihScreenshot[a], RatingRole.Rifler, RatingRole.Support)), "zweih screenshot profile");

var iglBaseline = new System.Collections.Generic.Dictionary<RatingAttribute, float>();
foreach (RatingAttribute attribute in Enum.GetValues(typeof(RatingAttribute)))
{
    iglBaseline[attribute] = 10f;
}
var baseIgl = RoleRatingModel.Calculate(a => iglBaseline[a], RatingRole.Rifler, null, isIgl: true);
iglBaseline[RatingAttribute.Leader] = 20f;
var leaderGain = RoleRatingModel.Calculate(a => iglBaseline[a], RatingRole.Rifler, null, isIgl: true) - baseIgl;
iglBaseline[RatingAttribute.Leader] = 10f;
iglBaseline[RatingAttribute.Rifle] = 20f;
var rifleGain = RoleRatingModel.Calculate(a => iglBaseline[a], RatingRole.Rifler, null, isIgl: true) - baseIgl;
Equal(true, leaderGain > rifleGain * 2f, "IGL leadership matters substantially more than rifle skill");

Equal(100f, MarketRatingModel.ReplaceSkillComponent(100f, 15f, 15f), "same market rating keeps native component");
Equal(204f, MathF.Round(MarketRatingModel.ReplaceSkillComponent(100f, 14.2f, 18f)), "zweih-like market uplift");
Equal(250f, MarketRatingModel.ReplaceSkillComponent(100f, 5f, 20f), "market uplift is capped");
Equal(40f, MarketRatingModel.ReplaceSkillComponent(100f, 20f, 1f), "market reduction is capped");

Console.WriteLine("MetaBench.Ratings100 pure tests passed.");
