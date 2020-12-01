using System;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace WebApplication
{
    public class MemeWork
    {
        public WorkStatus Status;
        public Task Worker;
        public Meme Meme;
        public int Percentage;

        public MemeWork(Meme meme)
        {
            Percentage = 0;
            Meme = meme;
            Status = WorkStatus.Scheduled;
            Worker = new Task( () => DoWork() );
        }

        public void StartWork()
        {
            Worker.Start();
        }

        public async void DoWork()
        {
                IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Utils.InputFile);
                IStream videoStream = inputFile.VideoStreams.FirstOrDefault();
                IStream audioStream = inputFile.AudioStreams.FirstOrDefault();

                var conv = FFmpeg.Conversions.New()
                    .AddStream(videoStream)
                    .AddStream(audioStream)
                    .AddParameter($"-vf \"" +
                                  $"drawtext=text='{Meme.CatText}': fontfile='Impact' : x=300 : y=650 : fontcolor=black : fontsize=120," +
                                  $"drawtext=text='{Meme.DrummerText}': fontfile='Impact' : x=1100 : y=520 : fontcolor=white : fontsize=65," +
                                  $"drawtext=text='{Meme.DrumText}': fontfile='Impact' : x=1180 : y=880: fontcolor=white: fontsize=65:\"")
                    .SetOutput(Meme.FilePath);
                conv.OnProgress += (sender, args) =>
                {
                    Percentage = args.Percent;
                    Console.Write("\r                ");
                    Console.Write($"\r{Percentage:D2}");
                };

                await conv.Start();
                Status = WorkStatus.Done;
        }
    }

    public enum WorkStatus
    {
        Canceled,
        Working,
        Done,
        Scheduled
    }
}