using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MetaBench.Ratings100;

internal static class RatingTextOverlay
{
    private const string LabelName = "MetaBench.Ratings100.Value";

    internal static void Show(Image? image, float rawValue, TMP_Text? template, bool unknownWhenZero = true)
    {
        if (image == null)
        {
            return;
        }

        try
        {
            var label = GetOrCreateLabel(image, template);
            if (label == null)
            {
                return;
            }

            var unknown = unknownWhenZero && rawValue <= 0f;
            label.text = unknown ? "?" : RatingScale.To100(rawValue).ToString();
            label.color = unknown ? new Color(0.72f, 0.72f, 0.72f, 1f) : ColorFor(RatingScale.To100(rawValue));
            label.gameObject.SetActive(true);

            // Disable only the star graphic. Its GameObject stays active and
            // continues to control layout and visibility for the numeric child.
            image.enabled = false;
        }
        catch (Exception exception)
        {
            Ratings100Plugin.ModLog.LogWarning($"[MetaBench Ratings100] failed to replace a star image: {exception.Message}");
        }
    }

    private static TextMeshProUGUI? GetOrCreateLabel(Image image, TMP_Text? template)
    {
        var existing = image.transform.Find(LabelName);
        if (existing != null)
        {
            return existing.GetComponent<TextMeshProUGUI>();
        }

        var labelObject = new GameObject(LabelName);
        var rect = labelObject.AddComponent<RectTransform>();
        rect.SetParent(image.transform, false);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;

        var label = labelObject.AddComponent<TextMeshProUGUI>();
        label.alignment = TextAlignmentOptions.Center;
        label.enableWordWrapping = false;
        label.overflowMode = TextOverflowModes.Overflow;
        label.raycastTarget = false;
        label.fontStyle = FontStyles.Bold;
        label.enableAutoSizing = true;
        label.fontSizeMin = 9f;
        label.fontSizeMax = 32f;

        if (template != null)
        {
            label.font = template.font;
            label.fontSizeMax = Math.Max(12f, Math.Min(36f, template.fontSize * 1.2f));
        }
        else if (TMP_Settings.defaultFontAsset != null)
        {
            label.font = TMP_Settings.defaultFontAsset;
        }

        return label;
    }

    private static Color ColorFor(int rating)
    {
        if (rating >= 90)
        {
            return new Color(1f, 0.78f, 0.2f, 1f);
        }

        if (rating >= 80)
        {
            return new Color(0.35f, 1f, 0.45f, 1f);
        }

        if (rating >= 65)
        {
            return Color.white;
        }

        return new Color(0.82f, 0.82f, 0.82f, 1f);
    }
}
