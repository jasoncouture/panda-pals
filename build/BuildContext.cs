using Cake.Common;
using Cake.Common.Tools.DotNet.Build;
using Cake.Common.Tools.DotNet.Publish;
using Cake.Common.Tools.DotNet.Restore;
using Cake.Common.Tools.DotNet.Test;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;

public class BuildContext : FrostingContext
{
    public string MsBuildConfiguration { get; init; }
    public string RuntimeIdentifier { get; init; }
    public bool SelfContained { get; init; }
    public bool PackageRelease { get; init; }
    public bool RunTests { get; init; }
    public bool Restore { get; init; }
    public string Project { get; init; }
    public string WorkingDirectory { get; init; }
    public BuildContext(ICakeContext context)
        : base(context)
    {

        MsBuildConfiguration = context.Argument("configuration", "Release");
        RuntimeIdentifier = context.Argument("runtime", "linux-x64");
        SelfContained = context.Argument("self-contained", true);
        PackageRelease = context.Argument("createRelease", true);
        RunTests = context.Argument("run-tests", true);
        Restore = context.Argument("restore-packages", true);
        Project = context.Argument("project", "panda-pals.sln");
        WorkingDirectory = context.Argument("working-directory", System.IO.Path.GetFullPath(".."));
    }

    public DotNetBuildSettings GetDotNetBuildSettings() =>
        new()
        {
            WorkingDirectory = WorkingDirectory,
            Configuration = MsBuildConfiguration,
            NoRestore = true,
            NoLogo = true
        };

    public DotNetRestoreSettings GetDotNetRestoreSettings() =>
        new()
        {
            ArgumentCustomization = builder =>
                builder.AppendSwitch("-r", RuntimeIdentifier)
                    .AppendSwitch("-property:Configuration", "=", MsBuildConfiguration),
            WorkingDirectory = WorkingDirectory
        };

    public DotNetTestSettings GetDotNetTestSettings() =>
        new()
        {
            Configuration = MsBuildConfiguration,
            WorkingDirectory = WorkingDirectory,
            NoBuild = true,
            NoRestore = true,
            NoLogo = true
        };

    public DotNetPublishSettings GetDotNetPublishSettings() =>
        new()
        {
            Configuration = MsBuildConfiguration,
            Runtime = RuntimeIdentifier,
            WorkingDirectory = WorkingDirectory,
            OutputDirectory = System.IO.Path.Combine(WorkingDirectory, "output"),
            Force = true
        };
}