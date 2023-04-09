using CommandLine;
using Metrics.Core;
using Metrics.Sampling;
using Metrics.Utils;
using System;

namespace Metrics.StupidBenchmarks
{
    internal class CommonOptions
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
    internal class Counter
    { }

    [Verb("Meter")]
    internal class Meter
    { }

    [Verb("Histogram")]
    internal class Histogram
    { }

    [Verb("Timer")]
    internal class Timer
    { }

    [Verb("EWMA")]
    internal class Ewma
    { }

    [Verb("EDR")]
    internal class Edr
    { }

    [Verb("hdr")]
    internal class Hdr
    { }

    [Verb("hdrtimer")]
    internal class HdrTimer
    { }

    [Verb("hdrsync")]
    internal class HdrSync
    { }

    [Verb("hdrsynctimer")]
    internal class HdrSyncTimer
    { }

    [Verb("Uniform")]
    internal class Uniform
    { }

    [Verb("Sliding")]
    internal class Sliding
    { }

    [Verb("TimerImpact")]
    internal class TimerImpact
    { }

    [Verb("NoOp")]
    internal class NoOp
    { }

    //[HelpVerb]
    //public string GetUsage(string verb)
    //{
    //    return HelpText.AutoBuild(this);
    //}

    internal class Program
    {

        private static string target;
        private static CommonOptions targetOptions;

        private static void Main(string[] args)
        {
            //var options = new Options();
            //if (!Parser.Default.ParseArguments(args, options, (t, o) => { target = t; targetOptions = o as CommonOptions; }))
            //{
            //    Console.WriteLine(new CommonOptions().GetUsage());
            //    Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            //}

            BenchmarkRunner.DefaultTotalSeconds = targetOptions.Seconds;
            BenchmarkRunner.DefaultMaxThreads = targetOptions.MaxThreads;

            //Metric.Config.WithHttpEndpoint("http://localhost:1234/");

            Parser.Default.ParseArguments<Counter, Meter, Histogram, Timer, Ewma, Edr, Hdr, HdrHistogramReservoir, HdrSync, HdrSyncTimer, Uniform, HdrTimer, Sliding, TimerImpact, NoOp>(args)
                .WithParsed<Counter>(o =>
                {
                    var counter = new CounterMetric();
                    BenchmarkRunner.Run("Counter", () => counter.Increment());
                })
                .WithParsed<Meter>(o =>
                {
                    var meter = new MeterMetric();
                    BenchmarkRunner.Run("Meter", () => meter.Mark());
                })
                .WithParsed<Histogram>(o =>
                {
                    var histogram = new HistogramMetric();
                    BenchmarkRunner.Run("Histogram", () => histogram.Update(137));
                })
                .WithParsed<Timer>(o =>
                {
                    var timer = new TimerMetric();
                    BenchmarkRunner.Run("Timer", () => timer.Record(1, TimeUnit.Milliseconds));
                })
                .WithParsed<Ewma>(o =>
                {
                    var ewma = EWMA.OneMinuteEWMA();
                    BenchmarkRunner.Run("EWMA", () => ewma.Update(1));
                })
                .WithParsed<Edr>(o =>
                {
                    var edr = new ExponentiallyDecayingReservoir();
                    BenchmarkRunner.Run("EDR", () => edr.Update(1));
                })
                .WithParsed<Hdr>(o =>
                {
                    var hdrReservoir = new HdrHistogramReservoir();
                    BenchmarkRunner.Run("HDR Recorder", () => hdrReservoir.Update(1));
                })
                .WithParsed<HdrHistogramReservoir>(o => { })
                .WithParsed<HdrSync>(o => { })
                .WithParsed<HdrSyncTimer>(o => { })
                .WithParsed<Uniform>(o =>
                {
                    var uniform = new UniformReservoir();
                    BenchmarkRunner.Run("Uniform", () => uniform.Update(1));
                })
                .WithParsed<HdrTimer>(o =>
                {
                    var hdrTimer = new TimerMetric(new HdrHistogramReservoir());
                    BenchmarkRunner.Run("HDR Timer", () => hdrTimer.Record(1, TimeUnit.Milliseconds));
                })
                .WithParsed<Sliding>(o =>
                {
                    var sliding = new SlidingWindowReservoir();
                    BenchmarkRunner.Run("Sliding", () => sliding.Update(1));
                })
                .WithParsed<TimerImpact>(o =>
                {
                    var load = new WorkLoad();
                    BenchmarkRunner.Run("WorkWithoutTimer", () => load.DoSomeWork(), iterationsChunk: 10);
                    BenchmarkRunner.Run("WorkWithTimer", () => load.DoSomeWorkWithATimer(), iterationsChunk: 10);
                })
                .WithParsed<NoOp>(o =>
                {
                    BenchmarkRunner.Run("Noop", () => { });
                });
        }
    }
}