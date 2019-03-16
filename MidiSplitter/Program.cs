using Melanchall.DryWetMidi.Smf;
using Melanchall.DryWetMidi.Smf.Interaction;
using Melanchall.DryWetMidi.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiSplitter
{
    class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<string> globFiles = GlobClass.Glob("D:\\data\\folkdataset\\*.mid");
            Console.Write(globFiles.LongCount());
            var count = 0;            

            foreach (string filename in globFiles)
            {
                var split_filename = filename.Split('\\');
                var id_filename = split_filename[split_filename.Count() - 1];
                var midiFile = MidiFile.Read(filename);
                var result = new List<MidiFile>();
                var windowSize = 4;

                for (var i = 0; ; i++)
                {
                    var startTime = new BarBeatTimeSpan(i, 0);
                    var endTime = new BarBeatTimeSpan(i + windowSize, 0);

                    var grid = new ArbitraryGrid(new[] { startTime, endTime });
                    var newFiles = midiFile.SplitByGrid(grid).ToList();

                    if (!newFiles.Any())
                        break;
                    else if (newFiles.Count == 1)
                        result.Add(newFiles[0]);
                    else
                    {
                        var secondNewFile = newFiles[1];
                        var tempoMap = secondNewFile.GetTempoMap();
                        var length = secondNewFile.GetTimedEvents().LastOrDefault()?.TimeAs<BarBeatTimeSpan>(tempoMap);

                        if (length.Bars == windowSize || (length.Bars == windowSize - 1 && length.Beats > 0 && length.Ticks > 0))
                            result.Add(secondNewFile);
                        else
                            break;
                    }
                }
                int j = 0;
                foreach (MidiFile res in result)
                {
                    res.Write("D:\\data\\folkdataset_split_4\\" + id_filename + "_" + j + ".mid");
                    j += 1;
                    count++;
                }

                //   TODO 
                if (count >= 10000)
                {
                    Console.WriteLine("Reached the alloted size");
                    return;
                }
            }

        }
    }
}
