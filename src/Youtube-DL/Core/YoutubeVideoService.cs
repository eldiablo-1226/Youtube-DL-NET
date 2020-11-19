﻿#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Youtube_DL.Helps;
using YoutubeExplode.Converter;
using Youtube_DL.Model;
using YoutubeExplode;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace Youtube_DL.Core
{

    public partial class YoutubeVideoService
    {
        private readonly YoutubeClient _youtube = new YoutubeClient();

        public bool IsLoading;

        public async Task DownloadAsync(VideoDownloadOption? videoOption, Video downloaVideo, Progress<double> progress, CancellationToken cancellationToken)
        {
            if (videoOption == null)
                throw new InvalidOperationException($"Video '{downloaVideo.Id}' contains no streams.");

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                Path.ChangeExtension(downloaVideo.Title, videoOption.Format));
            var conversion = new ConversionRequestBuilder(path)
                .SetFormat(videoOption.Format)
                .SetPreset(ConversionPreset.Medium)
                .Build();
            IsLoading = true;

            await _youtube.Videos.DownloadAsync(
                videoOption.StreamInfos,
                conversion,
                progress,
                cancellationToken
            );
        }
        public async Task<IReadOnlyList<VideoDownloadOption>> GetVideoDownloadOptionsAsync(string videoId)
        {
            var streamManifest = await new YoutubeClient().Videos.Streams.GetManifestAsync(videoId);

            // Using a set ensures only one download option per format/quality is provided
            var options = new HashSet<VideoDownloadOption>();

            // Video+audio options
            var videoStreams = streamManifest
                .GetVideo()
                .OrderByDescending(v => v.VideoQuality)
                .ThenByDescending(v => v.Framerate);

            foreach (var streamInfo in videoStreams)
            {
                var format = streamInfo.Container.Name;
                var label = streamInfo.VideoQualityLabel;
                var frameRate = streamInfo.Framerate.ToString();
                var videoSize = streamInfo.Size.TotalBytes.SizeSuffix();

                // Muxed streams are standalone
                if (streamInfo is MuxedStreamInfo)
                {
                    options.Add(new VideoDownloadOption(format, label, streamInfo));
                    continue;
                }

                // Get audio with matching format, if possible
                var audioStreamInfo =
                    (IStreamInfo?)
                    streamManifest
                        .GetAudioOnly()
                        .OrderByDescending(s => s.Container == streamInfo.Container)
                        .ThenByDescending(s => s.Bitrate)
                        .FirstOrDefault() ??
                    streamManifest
                        .GetMuxed()
                        .OrderByDescending(s => s.Container == streamInfo.Container)
                        .ThenByDescending(s => s.Bitrate)
                        .FirstOrDefault();

                if (audioStreamInfo != null)
                {
                    options.Add(new VideoDownloadOption(format, label, streamInfo, audioStreamInfo));
                }
            }

            // Audio-only options
            var bestAudioOnlyStreamInfo = streamManifest
                .GetAudio()
                .OrderByDescending(s => s.Container == Container.WebM)
                .ThenByDescending(s => s.Bitrate)
                .FirstOrDefault();

            if (bestAudioOnlyStreamInfo != null)
            {
                options.Add(new VideoDownloadOption("mp3", "Audio", bestAudioOnlyStreamInfo));
                options.Add(new VideoDownloadOption("ogg", "Audio", bestAudioOnlyStreamInfo));
            }

            return options.ToArray();
        }

        public async Task<VideoDownloadOption?> TryGetBestVideoDownloadOptionAsync( string videoId, string format, VideoQualityPreference qualityPreference)
        {
            var options = await GetVideoDownloadOptionsAsync(videoId);

            
            if (string.Equals(format, "mp3", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(format, "ogg", StringComparison.OrdinalIgnoreCase))
            {
                return options.FirstOrDefault(o => string.Equals(o.Format, format, StringComparison.OrdinalIgnoreCase));
            }

            var orderedOptions = options
                .OrderBy(o => o.Quality)
                .ThenBy(o => o.Framerate)
                .ToArray();

            var preferredOption = qualityPreference switch
            {
                VideoQualityPreference.Maximum => orderedOptions
                    .LastOrDefault(o => string.Equals(o.Format, format, StringComparison.OrdinalIgnoreCase)),

                VideoQualityPreference.High => orderedOptions
                    .Where(o => o.Quality <= VideoQuality.High1080)
                    .LastOrDefault(o => string.Equals(o.Format, format, StringComparison.OrdinalIgnoreCase)),

                VideoQualityPreference.Medium => orderedOptions
                    .Where(o => o.Quality <= VideoQuality.High720)
                    .LastOrDefault(o => string.Equals(o.Format, format, StringComparison.OrdinalIgnoreCase)),

                VideoQualityPreference.Low => orderedOptions
                    .Where(o => o.Quality <= VideoQuality.Medium480)
                    .LastOrDefault(o => string.Equals(o.Format, format, StringComparison.OrdinalIgnoreCase)),

                VideoQualityPreference.Minimum => orderedOptions
                    .FirstOrDefault(o => string.Equals(o.Format, format, StringComparison.OrdinalIgnoreCase)),

                _ => throw new ArgumentOutOfRangeException(nameof(qualityPreference))
            };

            return
                preferredOption ??
                orderedOptions.FirstOrDefault(o => string.Equals(o.Format, format, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<Video[]> GetVideosAsync(string url)
        {
            VideoType typeInfo = TryParce(url);
            try
            {
                if (typeInfo == VideoType.Video)
                {
                    var videos = new[] { await _youtube.Videos.GetAsync(url) };
                    return videos;
                }
                if (typeInfo == VideoType.PlayList)
                {
                    var plaList = await _youtube.Playlists.GetVideosAsync(url);
                    return plaList.ToArray();
                }
            }
            catch (Exception)
            {
                throw new Exception("Can't get video info");
            }
            throw new AggregateException("Can't parse url");
        }
    }

    public partial class YoutubeVideoService
    {
        public static VideoType TryParce(string videoUri)
        {
            var videiId = TryParseVideoId(videoUri);
            if (videiId is not null)
                return VideoType.Video;

            var playListiId = TryParsePlaylistId(videoUri);
            if (playListiId is not null)
                return VideoType.PlayList;

            return VideoType.none;
        }

        public static bool TryParceBool(string videoUri)
        {
            var videiId = TryParce(videoUri);
            if (videiId == VideoType.none)
                return false;
            else
                return true;
        }

        public static VideoId? TryParseVideoId(string query)
        {
            try
            {
                return new VideoId(query);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        public static PlaylistId? TryParsePlaylistId(string query)
        {
            try
            {
                return new PlaylistId(query);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
    }

    public enum VideoType
    {
        Video,
        PlayList,
        none
    }
}