using System;
using CommandLine;
using CommandLine.Text;
using Metrics.Core;
using Metrics.Sampling;
using Metrics.Utils;
namespace Metrics.StupidBenchmarks
{
    class CommonOptions
    {
        [Option('c', HelpText = "Max Threads", Default = 32)]
        public int MaxThreads { get; set; }

        [Option('s', HelpText = "Seconds", Default = 5)]
        public int Seconds { get; set; }

        [Option('d', HelpText = "Number of threads to decrement each step", Default = 4)]
        public int Decrement { get; set; }

        //[HelpOption]
        //public string GetUsage()
        //{
        //    return HelpText.AutoBuild(this);
        //}
    }


        [Verb("Counter")]
        class Counter { }

        [Verb("Meter")]
        class Meter { }

        [Verb("Histogram")]
        class Histogram {  }

        [Verb("Timer")]
        class Timer {  }

        [Verb("EWMA")]
    class Ewma { }

        [Verb("EDR")]
    class Edr {  }

        [Verb("hdr")]
    class Hdr {  }

        [Verb("hdrtimer")]
    class HdrTimer {  }

        [Verb("hdrsync")]
    class HdrSync { }

        [Verb("hdrsynctimer")]
    class HdrSyncTimer { }

        [Verb("Uniform")]
    class Uniform {  }

        [Verb("Sliding")]
    class Sliding { }

        [Verb("TimerImpact")]
    class TimerImpact {  }

        [Verb("NoOp")]
    class NoOp { }

        //[HelpVerb]
        //public string GetUsage(string verb)
        //{
        //    return HelpText.AutoBuild(this);
        //}

    class Program
    {
        private static string target;
        private static CommonOptions targetOptions;

        static void Main(string[] args)
        {
   
            if (Parser.Default.ParseArguments<Counter, Meter, Histogram, Timer, Ewma, Edr, Hdr,HdrHistogramReservoir, HdrSync, HdrSyncTimer, HdrTimer,Sliding,TimerImpact, NoOp>(args, options, (t, o) => { target = t; targetOptions = o as CommonOptions; }))
            {
                Console.WriteLine(new CommonOptions().GetUsage());
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }

            BenchmarkRunner.DefaultTotalSeconds = targetOptions.Seconds;
            BenchmarkRunner.DefaultMaxThreads = targetOptions.MaxThreads;

            //Metric.Config.WithHttpEndpoint("http://localhost:1234/");

            switch (target)
            {
                case "noop":
                    BenchmarkRunner.Run("Noop", () => { });
                    break;
                case "counter":
                    var counter = new CounterMetric();
                    BenchmarkRunner.Run("Counter", () => counter.Increment());
                    break;
                case "meter":
                    var meter = new MeterMetric();
                    BenchmarkRunner.Run("Meter", () => meter.Mark());
                    break;
                case "histogram":
                    var histogram = new HistogramMetric();
                    BenchmarkRunner.Run("Histogram", () => histogram.Update(137));
                    break;
                case "timer":
                    var timer = new TimerMetric();
                    BenchmarkRunner.Run("Timer", () => timer.Record(1, TimeUnit.Milliseconds));
                    break;
                case "hdrtimer":
                    var hdrTimer = new TimerMetric(new HdrHistogramReservoir());
                    BenchmarkRunner.Run("HDR Timer", () => hdrTimer.Record(1, TimeUnit.Milliseconds));
                    break;
                case "ewma":
                    var ewma = EWMA.OneMinuteEWMA();
                    BenchmarkRunner.Run("EWMA", () => ewma.Update(1));
                    break;
                case "edr":
                    var edr = new ExponentiallyDecayingReservoir();
                    BenchmarkRunner.Run("EDR", () => edr.Update(1));
                    break;
                case "hdr":
                    var hdrReservoir = new HdrHistogramReservoir();
                    BenchmarkRunner.Run("HDR Recorder", () => hdrReservoir.Update(1));
                    break;
                case "uniform":
                    var uniform = new UniformReservoir();
                    BenchmarkRunner.Run("Uniform", () => uniform.Update(1));
                    break;
                case "sliding":
                    var sliding = new SlidingWindowReservoir();
                    BenchmarkRunner.Run("Sliding", () => sliding.Update(1));
                    break;
                case "timerimpact":
                    var load = new WorkLoad();
                    BenchmarkRunner.Run("WorkWithoutTimer", () => load.DoSomeWork(), iterationsChunk: 10);
                    BenchmarkRunner.Run("WorkWithTimer", () => load.DoSomeWorkWithATimer(), iterationsChunk: 10);
                    break;
            }
        }
    }
}
