using System.IO;
using System.Linq;
using System.Text;
using Cake.Common.IO;
using Cake.Common.Tools.GitVersion;
using Cake.Core.IO;
using Cake.Frosting;

namespace Build.Tasks;

[TaskName("CreatePackage")]
[IsDependentOn((typeof(PublishTask)))]
public class CreatePackageTask : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context) => context.PackageRelease;
    public override void Run(BuildContext context)
    {
        var releaseFile = System.IO.Path.Combine(context.WorkingDirectory,
            BuildFileName(context));
        var rootDirectory = System.IO.Path.Combine(context.WorkingDirectory, "output");
        context.Zip(rootDirectory, releaseFile, context.Globber.Match(new GlobPattern($"../output/{context.RuntimeIdentifier}/**/*"), new GlobberSettings()
        {
            IsCaseSensitive = false
        }).Select(i => i.FullPath));
    }

    private string BuildFileName(BuildContext context)
    {
        StringBuilder releaseZipNameBuilder = new StringBuilder("release");
        var gitVersion = context.CurrentGitVersionData;

        releaseZipNameBuilder.AppendFormat("-{0}-{1}", gitVersion.SemVer, gitVersion.Sha[..7]);
        releaseZipNameBuilder.AppendFormat("-{0}", context.RuntimeIdentifier);
        releaseZipNameBuilder.Append(".zip");

        return releaseZipNameBuilder.ToString();
    }
}