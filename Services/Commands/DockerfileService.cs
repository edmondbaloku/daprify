using CLI.Models;
using CLI.Templates;

namespace CLI.Services
{
    public class DockerfileService(IQuery query, IProjectProvider projectProvider, TemplateFactory templateFactory) : CommandService(DOCKER_NAME)
    {
        private readonly TemplateFactory _templateFactory = templateFactory;
        private readonly IProjectProvider _projectProvider = projectProvider;
        private const string DOCKER_NAME = "Docker";
        private const string PROJECT_OPT = "project_path";
        private const string SOLUTION_OPT = "solution_path";
        private const string DOCKERFILE_EXT = ".Dockerfile";
        private readonly IQuery _query = query;
        IEnumerable<IProject> _projects = [];


        protected override List<string> CreateFiles(OptionDictionary options, string workingDir)
        {
            GetServices(options);
            DockerfileTemplate dockerfileTemp = GetDockerfile();
            GenDockerFile(dockerfileTemp, workingDir);

            return ["Dockerfile"];
        }

        private void GenDockerFile(DockerfileTemplate dockerfileTemp, string workingDir)
        {
            foreach (IProject project in _projects)
            {
                string dockerfile = dockerfileTemp.Render(project.GetRelativeProjPath(), project.GetName());
                DirectoryService.WriteFile(workingDir, project.GetName() + DOCKERFILE_EXT, dockerfile);
            }
        }

        private DockerfileTemplate GetDockerfile()
        {
            return _templateFactory.GetTemplateService<DockerfileTemplate>();
        }

        private void GetServices(OptionDictionary options)
        {
            List<string> projectPathOpt = options.GetAllPairValues(PROJECT_OPT);
            MyPath projectRoot = projectPathOpt.Count > 0 ? new(projectPathOpt[0]) : new(string.Empty);

            IEnumerable<MyPath> solutionPaths = MyPath.FromStringList(options.GetAllPairValues(SOLUTION_OPT));
            IEnumerable<Solution> solutions = solutionPaths.Select(path => new Solution(_query, _projectProvider, path));

            _projects = SolutionService.GetDaprServicesFromSln(projectRoot, solutions);
        }
    }
}