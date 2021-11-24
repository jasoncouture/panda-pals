using Cake.Frosting;
namespace Build.Tasks;

[TaskName("Default")]
[IsDependentOn(typeof(LogBuildArgumentsTask))]
[IsDependentOn(typeof(TestTask))]
// ReSharper disable once UnusedMember.Global
public class DefaultTask : FrostingTask
{

}