using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MetaBench.Ratings100;

public static class RatingScale
{
    public const float Multiplier = 5f;

    private static readonly Regex NumberPattern = new(
        @"(?<![\w.])(?<sign>[+-]?)(?<number>\d+(?:[.,]\d+)?)(?![\w.])",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex DatePattern = new(
        @"\b\d{4}[-./]\d{1,2}[-./]\d{1,2}\b",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static int To100(float value)
    {
        if (float.IsNaN(value) || float.IsInfinity(value))
        {
            return 0;
        }

        var scaled = (int)Math.Round(value * Multiplier, MidpointRounding.AwayFromZero);
        return Math.Max(0, Math.Min(100, scaled));
    }

    public static string ScaleRenderedText(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text ?? string.Empty;
        }

        if (DatePattern.IsMatch(text))
        {
            return text;
        }

        return NumberPattern.Replace(text, match =>
        {
            var numberText = match.Groups["number"].Value;
            var normalized = numberText.Replace(',', '.');
            if (!double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
            {
                return match.Value;
            }

            // Attribute UI is based on 0–20. Larger numbers are percentages,
            // dates, prices or already-converted values and must stay intact.
            if (value < 0d || value > 20d)
            {
                return match.Value;
            }

            var scaled = value * Multiplier;
            var result = scaled.ToString("0.##", CultureInfo.InvariantCulture);
            if (numberText.Contains(','))
            {
                result = result.Replace('.', ',');
            }

            return match.Groups["sign"].Value + result;
        });
    }
}
