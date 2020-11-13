using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NYoutubeDL;
using Youtube_DL.Model;

namespace Youtube_DL.Core
{
    public static class YoutubeDownloader
    {
        public static YoutubeDL ydlClient { get; }

        static YoutubeDownloader()
        {
            ydlClient = new YoutubeDL(@"Resources\youtube-dl.exe");
        }
    }
}
