using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MetaBench.Ratings100;

internal static class RatingTextOverlay
{
    private const string LabelName = "MetaBench.Ratings100.Value";

    internal static void Show(
        Image? image,
        float rawValue,
        TMP_Text? template,
        bool isPotential = false,
        bool unknownWhenZero = true)
    {
        if (image == null)
        {
            return;
        }

        try
        {
            var overlayRoot = image.transform.parent ?? image.transform;
            // Position the number in the exact rectangle previously occupied by
            // the filled star layer. The parent is used only to find and hide
            // the remaining empty/background star graphics.
            var label = GetOrCreateLabel(image.transform, template);
            if (label == null)
            {
                return;
            }

            var unknown = unknownWhenZero && rawValue <= 0f;
            var displayedValue = isPotential
                ? RatingScale.PotentialTo100(rawValue)
                : RatingScale.To100(rawValue);
            label.text = unknown ? "?" : displayedValue.ToString();
            label.color = unknown ? new Color(0.72f, 0.72f, 0.72f, 1f) : ColorFor(displayedValue);
            label.gameObject.SetActive(true);
            label.transform.SetAsLastSibling();

            HideStarGraphics(image, overlayRoot);
        }
        catch (Exception exception)
        {
            Ratings100Plugin.ModLog.LogWarning($"[MetaBench Ratings100] failed to replace a star image: {exception.Message}");
        }
    }

    private static TextMeshProUGUI? GetOrCreateLabel(Transform overlayRoot, TMP_Text? template)
    {
        var existing = overlayRoot.Find(LabelName);
        if (existing != null)
        {
            return existing.GetComponent<TextMeshProUGUI>();
        }

        var labelObject = new GameObject(LabelName);
        var rect = labelObject.AddComponent<RectTransform>();
        rect.SetParent(overlayRoot, false);
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
        label.enableAutoSizing = false;
        label.fontSize = 18f;

        if (template != null)
        {
            label.font = template.font;
            label.fontSize = Math.Max(18f, Math.Min(30f, template.fontSize * 1.3f));
        }
        else if (TMP_Settings.defaultFontAsset != null)
        {
            label.font = TMP_Settings.defaultFontAsset;
        }

        return label;
    }

    private static void HideStarGraphics(Image primaryImage, Transform overlayRoot)
    {
        var images = overlayRoot.GetComponentsInChildren<Image>(true);
        foreach (var candidate in images)
        {
            if (candidate == null || candidate.transform == null)
            {
                continue;
            }

            if (candidate == primaryImage || LooksLikeStar(candidate))
            {
                candidate.enabled = false;
            }
        }

        // Some prefabs use an Image directly on the star container.
        var containerImage = overlayRoot.GetComponent<Image>();
        if (containerImage != null && LooksLikeStar(containerImage))
        {
            containerImage.enabled = false;
        }
    }

    private static bool LooksLikeStar(Image image)
    {
        var objectName = image.gameObject.name?.ToLowerInvariant() ?? string.Empty;
        var spriteName = image.sprite != null
            ? image.sprite.name?.ToLowerInvariant() ?? string.Empty
            : string.Empty;

        return objectName.Contains("star")
            || objectName.Contains("rating")
            || objectName.Contains("potential")
            || spriteName.Contains("star")
            || spriteName.Contains("rating")
            || spriteName.Contains("potential");
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
