using System;
using System.IO;
using System.Threading.Tasks;
using VideoLibrary;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Enums;

namespace YoutubeHelper
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Setup
            var youTube = YouTube.Default;
            Console.Write("Downloading FFmpeg...");
            await FFmpeg.GetLatestVersion();
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
                if (outputMp3File.Exists)
                {
                    Console.WriteLine($"\t{outname} already downloaded. Skipping...");
                    continue;
                }
                Console.Write($"\t-Downloading {outname}...");
                //Get video
                var video = youTube.GetVideo(url);
                var videoDownloadFile = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
                //Write video to file
                File.WriteAllBytes(videoDownloadFile, video.GetBytes());
                Console.Write($"Done. Converting to mp3...");
                //Extract mp3 from video
                var result = await Conversion.ExtractAudio(videoDownloadFile, outputMp3File.FullName)
                    .Start();
                Console.WriteLine($"Done");
            }

            Console.WriteLine("Downloading Hold Songs...");
            foreach (var line in File.ReadAllLines(holdSongsFile.FullName))
            {
                var url = line.Split(null)[0];
                var outname = line.Split(null)[1];
                var outputMp3File = new FileInfo(Path.Combine(holdMusicDir.FullName, outname + ".mp3"));
                if (outputMp3File.Exists)
                {
                    Console.WriteLine($"\t{outname} already downloaded. Skipping...");
                    continue;
                }
                Console.Write($"\t-Downloading {outname}...");
                var video = youTube.GetVideo(url);
                string videoDownloadFile = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
                File.WriteAllBytes(videoDownloadFile, video.GetBytes());
                Console.Write($"Done. Converting to mp3...");

                var result = await Conversion.ExtractAudio(videoDownloadFile, outputMp3File.FullName)
                    .Start();

                Console.WriteLine($"Done");
            }
            Console.WriteLine("\nDone. Press any key to exit...");
            Console.Read();
        }
    }
}
