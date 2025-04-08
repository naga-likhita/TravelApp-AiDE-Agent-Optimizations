using BenchmarkDotNet.Running;
using BenchReports;
using System.Diagnostics;
using TravelApp;

var summary = BenchmarkRunner.Run<PerformanceTest_NoCache>();