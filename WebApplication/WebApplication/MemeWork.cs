using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Xabe.FFmpeg;

namespace WebApplication
{
    [Serializable]
    public class MemeWork
    {
        public WorkStatus Status;
        [XmlIgnore, JsonIgnore]
        public Task Worker;
        [XmlIgnore, JsonIgnore]
        public Meme Meme;
        public int Percentage;

        public MemeWork(){}
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
            Status = WorkStatus.Working;
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
                    Debug.Write("\r                ");
                    Debug.Write($"\r{Percentage:D2}");
                };

                await conv.Start();
                Status = WorkStatus.Done;
        }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum WorkStatus
    {
        Canceled,
        Working,
        Done,
        Scheduled
    }
}