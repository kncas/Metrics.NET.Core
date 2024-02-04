using CommandLine;
using Metrics.Core;
using Metrics.Sampling;
using Metrics.Utils;

namespace Metrics.StupidBenchmarks
{
    internal class CommonOptions
    {
        [Option('c', HelpText = "Max Threads")]
        public int MaxThreads { get; set; } = 32;

        [Option('s', HelpText = "Seconds")]
        public int Seconds { get; set; } = 5;

        [Option('d', HelpText = "Number of threads to decrement each step")]
        public int Decrement { get; set; } = 4;
    }

    [Verb("Counter", HelpText = "")]
    internal class Counter : CommonOptions
    { }

    [Verb("Meter", HelpText = "")]
    internal class Meter : CommonOptions
    { }

    [Verb("Histogram", HelpText = "")]
    internal class Histogram : CommonOptions
    { }

    [Verb("Timer", HelpText = "")]
    internal class Timer : CommonOptions
    { }

    [Verb("EWMA", HelpText = "")]
    internal class Ewma : CommonOptions
    { }

    [Verb("EDR", HelpText = "")]
    internal class Edr : CommonOptions
    { }

    [Verb("hdr", HelpText = "")]
    internal class Hdr : CommonOptions
    { }

    [Verb("hdrtimer", HelpText = "")]
    internal class HdrTimer : CommonOptions
    { }

    [Verb("hdrsync", HelpText = "")]
    internal class HdrSync : CommonOptions
    { }

    [Verb("hdrsynctimer", HelpText = "")]
    internal class HdrSyncTimer : CommonOptions
    { }

    [Verb("Uniform", HelpText = "")]
    internal class Uniform : CommonOptions
    { }

    [Verb("Sliding", HelpText = "")]
    internal class Sliding : CommonOptions
    { }

    [Verb("TimerImpact", HelpText = "")]
    internal class TimerImpact : CommonOptions
    { }

    [Verb("NoOp", HelpText = "")]
    internal class NoOp : CommonOptions
    { }

    internal class Program
    {
        private static CommonOptions targetOptions;

        private static void Main(string[] args)
        {

            var result = Parser.Default.ParseArguments<Counter, Meter, Histogram, Timer, Ewma, Edr, Hdr, Uniform, HdrTimer, Sliding, TimerImpact, NoOp>(args)
                .WithParsed<Counter>(o =>
                {
                    var counter = new CounterMetric();
                    BenchmarkRunner.DefaultTotalSeconds = o.Seconds;
                    BenchmarkRunner.DefaultMaxThreads = o.MaxThreads;
                    BenchmarkRunner.Run("Counter", () => counter.Increment());
                })
                .WithParsed<Meter>(o =>
                {
                    var meter = new MeterMetric();
                    BenchmarkRunner.DefaultTotalSeconds = o.Seconds;
                    BenchmarkRunner.DefaultMaxThreads = o.MaxThreads;
                    BenchmarkRunner.Run("Meter", () => meter.Mark());
                })
                .WithParsed<Histogram>(o =>
                {
                    var histogram = new HistogramMetric();
                    BenchmarkRunner.DefaultTotalSeconds = targetOptions.Seconds;
                    BenchmarkRunner.DefaultMaxThreads = targetOptions.MaxThreads;
                    BenchmarkRunner.Run("Histogram", () => histogram.Update(137));
                })
                .WithParsed<Timer>(o =>
                {
                    var timer = new TimerMetric();
                    BenchmarkRunner.DefaultTotalSeconds = o.Seconds;
                    BenchmarkRunner.DefaultMaxThreads = o.MaxThreads;
                    BenchmarkRunner.Run("Timer", () => timer.Record(1, TimeUnit.Milliseconds));
                })
                .WithParsed<Ewma>(o =>
                {
                    var ewma = EWMA.OneMinuteEWMA();
                    BenchmarkRunner.DefaultTotalSeconds = o.Seconds;
                    BenchmarkRunner.DefaultMaxThreads = o.MaxThreads;
                    BenchmarkRunner.Run("EWMA", () => ewma.Update(1));
                })
                .WithParsed<Edr>(o =>
                {
                    var edr = new ExponentiallyDecayingReservoir();
                    BenchmarkRunner.DefaultTotalSeconds = o.Seconds;
                    BenchmarkRunner.DefaultMaxThreads = o.MaxThreads;
                    BenchmarkRunner.Run("EDR", () => edr.Update(1));
                })
                .WithParsed<Hdr>(o =>
                {
                    var hdrReservoir = new HdrHistogramReservoir();
                    BenchmarkRunner.DefaultTotalSeconds = o.Seconds;
                    BenchmarkRunner.DefaultMaxThreads = o.MaxThreads;
                    BenchmarkRunner.Run("HDR Recorder", () => hdrReservoir.Update(1));
                })
                .WithParsed<Uniform>(o =>
                {
                    var uniform = new UniformReservoir();
                    BenchmarkRunner.DefaultTotalSeconds = o.Seconds;
                    BenchmarkRunner.DefaultMaxThreads = o.MaxThreads;
                    BenchmarkRunner.Run("Uniform", () => uniform.Update(1));
                })
                .WithParsed<HdrTimer>(o =>
                {
                    var hdrTimer = new TimerMetric(new HdrHistogramReservoir());
                    BenchmarkRunner.DefaultTotalSeconds = o.Seconds;
                    BenchmarkRunner.DefaultMaxThreads = o.MaxThreads;
                    BenchmarkRunner.Run("HDR Timer", () => hdrTimer.Record(1, TimeUnit.Milliseconds));
                })
                .WithParsed<Sliding>(o =>
                {
                    var sliding = new SlidingWindowReservoir();
                    BenchmarkRunner.DefaultTotalSeconds = o.Seconds;
                    BenchmarkRunner.DefaultMaxThreads = o.MaxThreads;
                    BenchmarkRunner.Run("Sliding", () => sliding.Update(1));
                })
                .WithParsed<TimerImpact>(o =>
                {
                    var load = new WorkLoad();
                    BenchmarkRunner.DefaultTotalSeconds = o.Seconds;
                    BenchmarkRunner.DefaultMaxThreads = o.MaxThreads;
                    BenchmarkRunner.Run("WorkWithoutTimer", () => load.DoSomeWork(), iterationsChunk: 10);
                    BenchmarkRunner.Run("WorkWithTimer", () => load.DoSomeWorkWithATimer(), iterationsChunk: 10);
                })
                .WithParsed<NoOp>(o =>
                {
                    BenchmarkRunner.DefaultTotalSeconds = o.Seconds;
                    BenchmarkRunner.DefaultMaxThreads = o.MaxThreads;
                    BenchmarkRunner.Run("Noop", () => { });
                });
        }
    }
}