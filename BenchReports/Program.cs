using BenchmarkDotNet.Running;
using BenchReports;

var summary = BenchmarkRunner.Run<PerformanceTest_Cache>();