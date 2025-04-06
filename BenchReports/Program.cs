using BenchmarkDotNet.Running;
using BenchReports;
using System.Diagnostics;
using TravelApp;

var summary = BenchmarkRunner.Run<PerformanceTests>();

//var sw = new Stopwatch();
//sw.Start();
//await new NotificationService().ReminderUsersAsync();
//sw.Stop();
//Console.WriteLine(sw.Elapsed.ToString());
//Console.ReadLine();