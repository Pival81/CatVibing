using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ImageMagick;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Xabe.FFmpeg;

namespace WebApplication
{
    public class MemeWork
    {
        public WorkStatus Status;
        protected internal Task Worker;
        [JsonIgnore] public Meme Meme;
        public int Percentage;
        private CancellationTokenSource _cancellationToken;

        public MemeWork(){}
        public MemeWork(Meme meme)
        {
            Percentage = 0;
            Meme = meme;
            Status = WorkStatus.Scheduled;
            Worker = new Task(DoWork);
            _cancellationToken = new CancellationTokenSource();
        }

        public void StartWork()
        {
            Worker.Start();
            Status = WorkStatus.Working;
        }

        public void StopWork()
        {
            _cancellationToken.Cancel();
            Status = WorkStatus.Stopped;
        }

        public async void DoWork()
        {
            var catImage = new MagickImage();
            catImage.Read($"caption:{Meme.CatText}", new MagickReadSettings()
            {
                BackgroundColor = MagickColors.Transparent,
                FillColor = MagickColors.Black,
                Font = "Impact",
                Width = 800,
                Height = 800,
                TextGravity = Gravity.Center
            });
            if (Int32.Parse(catImage.GetAttribute("caption:pointsize")) > 120)
            {
                catImage.Settings.FontPointsize = 120;
                catImage.Read($"caption:{Meme.CatText}", catImage.Settings as MagickReadSettings);
            }
            catImage.Write($"./Temp/cat_{Meme.Guid}.png");

            var drummerImage = new MagickImage();
            drummerImage.Read($"caption:{Meme.DrummerText}", new MagickReadSettings()
            {
                BackgroundColor = MagickColors.Transparent,
                FillColor = MagickColors.White,
                Font = "Impact",
                Width = 700,
                Height = 460,
                TextGravity = Gravity.Center
            });
            if (Int32.Parse(drummerImage.GetAttribute("caption:pointsize")) > 90)
            {
                drummerImage.Settings.FontPointsize = 90;
                drummerImage.Read($"caption:{Meme.DrummerText}", drummerImage.Settings as MagickReadSettings);
            }
            drummerImage.Write($"./Temp/drummer_{Meme.Guid}.png");

            var drumImage = new MagickImage();
            drumImage.Read($"caption:{Meme.DrumText}", new MagickReadSettings()
            {
                BackgroundColor = MagickColors.Transparent,
                FillColor = MagickColors.White,
                Font = "Impact",
                Width = 1000,
                Height = 300,
                TextGravity = Gravity.Center
            });
            if (Int32.Parse(drumImage.GetAttribute("caption:pointsize")) > 90)
            {
                drumImage.Settings.FontPointsize = 90;
                drumImage.Read($"caption:{Meme.DrumText}", drumImage.Settings as MagickReadSettings);
            }
            drumImage.Write($"./Temp/drum_{Meme.Guid}.png");

            var conv = FFmpeg.Conversions.New();
            conv.OnProgress += (sender, args) => Percentage = args.Percent;

            try {
                await conv.Start($"-i {Utils.InputFile} " +
                     $"-i {Path.Combine(Startup.ContentRoot, "Temp", $"cat_{Meme.Guid}.png")} " +
                     $"-i {Path.Combine(Startup.ContentRoot, "Temp", $"drummer_{Meme.Guid}.png")} " +
                     $"-i {Path.Combine(Startup.ContentRoot, "Temp", $"drum_{Meme.Guid}.png")} " +
                     $"-filter_complex \"overlay={450 - (catImage.Width / 2)}:{530 - (catImage.Height / 2)} [out];" +
                     $"[out] overlay={1250 - (drummerImage.Width / 2)}:{500 - (drummerImage.Height / 2)} [out1]; " +
                     $"[out1] overlay={1300 - (drumImage.Width / 2)}:{900 - (drumImage.Height / 2)}\"" +
                     $" \"{Path.Combine(Startup.ContentRoot, "Videos", $"{Meme.Guid}.mp4")}\"",
                    _cancellationToken.Token);
                Status = WorkStatus.Done;
            } catch (OperationCanceledException ex) {
                Console.WriteLine($"{Meme.Guid} was stopped.");
                Status = WorkStatus.Stopped;
            }
            File.Delete(Path.Combine(Startup.ContentRoot, "Temp", $"cat_{Meme.Guid}.png"));
            File.Delete(Path.Combine(Startup.ContentRoot, "Temp", $"drummer_{Meme.Guid}.png"));
            File.Delete(Path.Combine(Startup.ContentRoot, "Temp", $"drum_{Meme.Guid}.png"));
        }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum WorkStatus
    {
        Stopped,
        Working,
        Done,
        Scheduled
    }
}