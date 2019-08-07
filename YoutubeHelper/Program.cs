using System;
using System.IO;
using System.Threading.Tasks;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Enums;
using YoutubeExplode;
using YoutubeExplode.Models.MediaStreams;

namespace YoutubeHelper
{
    class Program
    {
        private static readonly IYoutubeClient _client = new YoutubeClient();
        static async Task Main(string[] args)
        {
            Console.WriteLine("Done");
            var takeMusicDir = Directory.CreateDirectory("TakeMusic");
            var holdMusicDir = Directory.CreateDirectory("HoldMusic");
            
            var takeSongsFile = new FileInfo("Youtube_Take.txt");
            var holdSongsFile = new FileInfo("Youtube_Hold.txt");
            Console.WriteLine("Downloading Take Songs...");
            //Loop over each line in the take song file
            foreach (var line in File.ReadAllLines(takeSongsFile.FullName))
            {
                //Extract parts of line
                var url = line.Split(null)[0];
                var outname = line.Split(null)[1];
                var outputMp3File = new FileInfo(Path.Combine(takeMusicDir.FullName, outname + ".mp3"));
                //Skip song if already downloaded
                await DownloadMusic(url, outputMp3File);
            }

            Console.WriteLine("Downloading Hold Songs...");
            foreach (var line in File.ReadAllLines(holdSongsFile.FullName))
            {
                var url = line.Split(null)[0];
                var outname = line.Split(null)[1];
                var outputMp3File = new FileInfo(Path.Combine(holdMusicDir.FullName, outname + ".mp3"));
                await DownloadMusic(url, outputMp3File);
            }
            Console.WriteLine("\nDone. Press any key to exit...");
            Console.Read();
        }

        public static async Task DownloadMusic(string url, FileInfo desinationFile)
        {
            if (desinationFile.Exists)
            {
                Console.WriteLine($"\t{desinationFile.Name} already downloaded. Skipping...");
                return;
            }
            Console.Write($"\t-Downloading {desinationFile.Name}...");
            //Get video
            var vidId = YoutubeClient.ParseVideoId(url);
            var streamInfo = await _client.GetVideoMediaStreamInfosAsync(vidId);
            var stream = streamInfo.Audio.WithHighestBitrate();
            var ext = stream.Container.GetFileExtension();
            var videoDownloadFile = Path.ChangeExtension(Path.GetTempFileName(), ext);
            await _client.DownloadMediaStreamAsync(stream, videoDownloadFile);
            //Write video to file
            Console.Write($"Done. Converting to mp3...");
            //Extract mp3 from video
            var result = await Conversion.Convert(videoDownloadFile, desinationFile.FullName)
                .Start();
            Console.WriteLine($"Done");
        }
    }
}
