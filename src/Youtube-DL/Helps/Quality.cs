﻿using System;

namespace Youtube_DL.Helps
{
    public enum VideoQualityPreference
    {
        Minimum,
        Low,
        Medium,
        High,
        Maximum
    }

    public static class VideoQualityPreferenceExtensions
    {
        public static string GetDisplayName(this VideoQualityPreference preference)
        {
            return preference switch
            {
                VideoQualityPreference.Minimum => "Minimum",
                VideoQualityPreference.Low => "Low (up to 480p)",
                VideoQualityPreference.Medium => "Medium (up to 720p)",
                VideoQualityPreference.High => "High (up to 1080p)",
                VideoQualityPreference.Maximum => "Maximum",
                _ => throw new ArgumentOutOfRangeException(nameof(preference))
            };
        }
    }
}