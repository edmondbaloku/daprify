using Daprify.Models;
using Serilog;
using System.Diagnostics;
using System.Reflection;

namespace Daprify.Services
{
    public static class DirectoryService
    {
        const string DAPR = "Dapr";

        public static DirectoryInfo FindDirectoryUpwards(string directoryName)
        {
            DirectoryInfo executingDir = new(Assembly.GetExecutingAssembly().Location);
            while (executingDir.Name != directoryName)
            {
                executingDir = executingDir.Parent ?? throw new DirectoryNotFoundException($"Could not find directory: {directoryName}");
            }
            return executingDir;
        }

        public static MyPath SetDirectory(MyPath projectPath, string dirPath)
        {
            Log.Verbose("Setting working directory...");
            MyPath baseDir = CheckProjectType(projectPath);
            MyPath workingDir = MyPath.Combine(baseDir.ToString(), DAPR, dirPath);

            bool dirExists = Directory.Exists(workingDir.ToString());
            Log.Verbose(dirExists ? "Working directory already exists: {workingDir}" : "Creating working directory: {workingDir}", workingDir);

            if (!dirExists)
            {
                Directory.CreateDirectory(workingDir.ToString());
                Log.Verbose("Working directory successfully created: {workingDir}", workingDir);
            }
            return workingDir;
        }

        public static MyPath CheckProjectType(IPath projectPath)
        {
            Log.Verbose("Checking where the program is executed from...");
            string? isTestProject = Environment.GetEnvironmentVariable("isTestProject");
            if (isTestProject == "true")
            {
                Log.Verbose("Program is running from a test project.");
                return GetCurrentDirectory();
            }
            else
            {
                Log.Verbose("Program executing normally.");
                return GetGitRootDirectory(projectPath);
            }
        }

        /// <summary>
        /// Retrieves the root directory of the Git repository.
        /// If no Git repository is found, the parent directory of the current directory is returned.
        /// </summary>
        /// <returns>The root directory of the Git repository.</returns>
        public static MyPath GetGitRootDirectory(IPath path)
        {
            Log.Verbose("Getting git root directory...");
            if (path.HasFileExtension())
            {
                Log.Verbose("Path is a file. Setting directory path...");
                path.SetDirectoryPath();
            }

            string gitRoot = RunGitCommand(path.ToString());
            if (string.IsNullOrWhiteSpace(gitRoot))
            {
                Log.Warning("No Git root directory found.");
                var dir = Directory.GetParent(GetCurrentDirectory().ToString())!;
                Log.Verbose("Setting working directory: {dir}", Path.GetFullPath(dir.ToString()));
                return new(dir.FullName);
            }
            else
            {
                Log.Verbose("Git root directory found: {gitRoot}", gitRoot);
                return new(gitRoot.Trim());
            }
        }

        private static string RunGitCommand(string path)
        {
            Log.Verbose("Starting search from {path}", string.IsNullOrEmpty(path) ? GetCurrentDirectory() : path);
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = "rev-parse --show-toplevel",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = path
                }
            };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return process.ExitCode == 0 ? output : string.Empty;
        }

        public static MyPath CreateTempDirectory(IEnumerable<IPath>? paths = null)
        {
            MyPath tempPath = new(Path.GetTempPath());
            tempPath = paths?.Aggregate(tempPath, (current, next) => MyPath.Combine(current.ToString(), next.ToString())) ?? tempPath;

            Directory.CreateDirectory(tempPath.ToString());
            return tempPath;
        }


        public static void DeleteDirectory(IPath directoryPath)
        {
            if (Directory.Exists(directoryPath.ToString()))
            {
                Directory.Delete(directoryPath.ToString(), true);
            }
        }

        public static void WriteFile(IPath workingDir, string fileName, string content)
        {
            MyPath filePath = MyPath.Combine(workingDir.ToString(), fileName);
            Log.Verbose("Writing content to file: {filePath}", filePath);
            try
            {
                File.WriteAllText(filePath.ToString(), content);
                Log.Verbose("Content successfully written to file.");
            }
            catch (Exception ex)
            {
                Log.Error("Error writing content to file: {filePath}. Exception: {ex}", filePath, ex);
                throw;
            }
        }

        public static void AppendFile(IPath workingDir, string fileName, string content)
        {
            MyPath filePath = MyPath.Combine(workingDir.ToString(), fileName);
            Log.Verbose("Appending content to existing file: {filePath}", filePath);
            try
            {
                File.AppendAllText(filePath.ToString(), content);
                Log.Verbose("Content successfully appended to existing file.");
            }
            catch (Exception ex)
            {
                Log.Error("Error appending content to existing file: {filePath}. Exception: {ex}", filePath, ex);
                throw;
            }
        }

        public static MyPath GetCurrentDirectory()
        {
            return new(Directory.GetCurrentDirectory());
        }
    }
}