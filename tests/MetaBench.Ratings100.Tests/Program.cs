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

Console.WriteLine("MetaBench.Ratings100 pure tests passed.");
