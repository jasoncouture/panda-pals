using System.Text;
using Cake.Common.IO;
using Cake.Common.Tools.GitVersion;
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
        context.Zip(rootDirectory, releaseFile, "*.*");
        base.Run(context);
    }

    private string BuildFileName(BuildContext context)
    {
        StringBuilder releaseZipNameBuilder = new StringBuilder("release");
        var gitVersion = context.GitVersion();

        if (IsMainBranch(gitVersion.BranchName))
        {
            releaseZipNameBuilder.AppendFormat("-main");
        }

        releaseZipNameBuilder.AppendFormat("-{0}", gitVersion.Sha.Substring(0, 7));
        releaseZipNameBuilder.AppendFormat("-{0}", gitVersion.MajorMinorPatch);
        releaseZipNameBuilder.Append(".zip");

        return releaseZipNameBuilder.ToString();

    }

    private bool IsMainBranch(string branchName)
    {
        return branchName switch
        {
            "main" or "master" => true,
            _ => false
        };
    }
}