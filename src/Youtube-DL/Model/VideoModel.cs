using System;
using System.Collections.Generic;
using System.Linq;
using Youtube_DL.Helps;
using YoutubeExplode.Videos.Streams;

namespace Youtube_DL.Model
{
    public partial class VideoDownloadOption
    {
        public bool IsPlaylist { get; }
        public string Format { get; }
        public string Label { get; }
        public string Size { get; set; }

        public IReadOnlyList<IStreamInfo>? StreamInfos { get; }

        public VideoQuality? Quality =>
            StreamInfos?.OfType<IVideoStreamInfo>()
                .Select(s => s.VideoQuality)
                .OrderByDescending(q => q)
                .FirstOrDefault();

        public VideoQualityPreference QualityPreference { get; }

        public Framerate? Framerate =>
            StreamInfos?.OfType<IVideoStreamInfo>()
                .Select(s => s.Framerate)
                .OrderByDescending(f => f)
                .FirstOrDefault();

        public VideoDownloadOption(
            string format,
            string label,
            string size,
            IReadOnlyList<IStreamInfo> streamInfos)
        {
            Size = size;
            Format = format;
            Label = label;
            StreamInfos = streamInfos;
        }

        public VideoDownloadOption(VideoQualityPreference quality)
        {
            IsPlaylist = true;
            QualityPreference = quality;
            Format = "";
            Size = "";
            Label = quality.GetDisplayName();
        }

        public VideoDownloadOption(
            string format,
            string label,
            string size,
            params IStreamInfo[] streamInfos)
            : this(format, label, size, (IReadOnlyList<IStreamInfo>)streamInfos) { }

        public override string ToString() => $"{Label}  {Format}  {Size}";
    }

    public partial class VideoDownloadOption : IEquatable<VideoDownloadOption>
    {
        public bool Equals(VideoDownloadOption? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                StringComparer.OrdinalIgnoreCase.Equals(Format, other.Format) &&
                StringComparer.OrdinalIgnoreCase.Equals(Label, other.Label);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return Equals((VideoDownloadOption)obj);
        }

        public override int GetHashCode() => HashCode.Combine(
            StringComparer.OrdinalIgnoreCase.GetHashCode(Format),
            StringComparer.OrdinalIgnoreCase.GetHashCode(Label)
        );
    }
}
