using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using LowLevelDesign.Hexify;

namespace WebmFun
{
    class Program
    {
        static void Main(string[] args)
        {
            var video = Prompt("Enter webm video path: ");
            video = video.Trim();
            if(!File.Exists(video) || !video.EndsWith(".webm")) throw new Exception("You entered a wrong video path or it's not a webm file.");
            Console.WriteLine("Converting video to hex..");
            var hexVideo = Hex.ToHexString(File.ReadAllBytes(video));
            var timeStamp = Hex.ToHexString(new byte[] {0x2A, 0xD7, 0xB1});
            var durationStamp = Hex.ToHexString(new byte[] {0x44, 0x89});
            var neededPart = hexVideo.Substring(hexVideo.IndexOf(timeStamp, StringComparison.Ordinal));
            neededPart = neededPart.Substring(neededPart.IndexOf(durationStamp, StringComparison.Ordinal));
            var duration = new string(neededPart.Skip(4).Take(8).ToArray());
            Console.WriteLine("Old duration (hex): " + duration);
            var newDuration = duration.Replace(new string(duration.Skip(2).ToArray()), "000000");
            Console.WriteLine("New duration (hex): " + newDuration);
            Console.WriteLine("Replacing old hex with new duration..");
            var regex = new Regex(Regex.Escape(duration));
            var newText = regex.Replace(neededPart, newDuration, 1);
            hexVideo = hexVideo.Replace(neededPart, newText);
            Console.WriteLine("Writing result to the file..");
            var newFilePath = Path.Combine(Path.GetDirectoryName(video)!, Path.GetFileNameWithoutExtension(video) + "_edited.webm");
            File.Create(newFilePath).Close();
            File.WriteAllBytes(newFilePath, Hex.FromHexString(hexVideo));
            Console.WriteLine("Done!");
        }
        //Same as input("prompt") in python
        static string Prompt(string message)
        {
            Console.Write(message);
            return Console.ReadLine() ?? throw new ArgumentNullException();
        }
    }
}