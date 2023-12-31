using Daprify.Commands;
using Daprify.Models;
using Daprify.Services;
using Daprify.Settings;
using Daprify.Templates;
using Daprify.Validation;
using DaprifyTests.Mocks;
using FluentAssertions;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace DaprifyTests.Commands
{
    [TestClass]
    public class TryGenAllCommandTests
    {
        private readonly StringWriter _consoleOutput = new();
        private readonly GenAllService _service;
        private readonly GenAllSettings _settings = new();
        private readonly OptionDictionaryValidator _validator;
        private readonly MockServiceProvider _serviceProvider = new();
        private readonly TemplateFactory _templateFactory;
        private readonly DirectoryInfo _startingDir = new(Directory.GetCurrentDirectory());

        private const string FILE_EXT = ".yml", DAPR_DIR = "Dapr", COMPOSE_FILE = "docker-compose.yml", DOCKER_DIR = "Docker/",
                            CONFIG_FILE = "config.yml", CONFIG_DIR = "Config/", CERT_DIR = "Certs/",
                            ENV_FILE = "Dapr.Env", ENV_DIR = "Env/", COMPONENT_DIR = "Components/";
        private readonly MyPath _confPath;
        private readonly OptionValues componentArgs = new(new Key("components"), ["bindings", "configstore", "crypto", "lock", "pubsub", "secretstore", "statestore"]);
        private readonly OptionValues composeArgs = new(new Key("components"), ["dashboard", "placement", "rabbitmq", "redis", "sentry", "zipkin"]);
        private readonly OptionValues settingArgs = new(new Key("settings"), ["https", "logging", "metric", "middleware", "mtls", "tracing"]);
        private readonly OptionValues certFiles = new(new Key("certificates"), ["ca.crt", "issuer.crt", "issuer.key"]);

        public TryGenAllCommandTests()
        {
            MyPathValidator myPathValidator = new();
            OptionValuesValidator optionValuesValidator = new();
            _validator = new(myPathValidator, optionValuesValidator);

            _templateFactory = new(_serviceProvider.Object);
            MockIQuery mockIQuery = new();
            MockIProjectProvider mockIProjectProvider = new();
            CertificateService certificateService = new();
            ComponentService componentService = new(_templateFactory);
            ComposeService composeService = new(mockIQuery.Object, mockIProjectProvider.Object, _templateFactory);
            ConfigService configService = new(_templateFactory);
            DockerfileService dockerfileService = new(mockIQuery.Object, mockIProjectProvider.Object, _templateFactory);
            _service = new(certificateService, componentService, composeService, configService, dockerfileService);
            Console.SetOut(_consoleOutput);

            string testDir = DirectoryService.FindDirectoryUpwards("CommandTest").FullName;
            _confPath = MyPath.Combine(testDir, "../Utils/Mocks/config-mock.json");
            Directory.SetCurrentDirectory(DirectoryService.CreateTempDirectory().ToString());
            Environment.SetEnvironmentVariable("isTestProject", "true");
        }


        [TestMethod]
        public void Expected_Certificates_Generated()
        {
            // Arrange
            string[] argument = [_settings.CommandName,
                                 GenAllSettings.SettingOptionName[0],
                                 .. settingArgs.GetStringEnumerable(),];
            DaprifyCommand<GenAllService, GenAllSettings> sut = new(_service, _settings, _validator);

            // Act
            sut.Parse(argument).Invoke();

            // Assert
            Directory.SetCurrentDirectory(MyPath.Combine(DirectoryService.GetCurrentDirectory().ToString(), DAPR_DIR).ToString());
            string consoleOutput = _consoleOutput.ToString();

            foreach (string crt in certFiles.GetStringEnumerable())
            {
                consoleOutput.Should().Contain(crt);
                File.Exists(CERT_DIR + crt).Should().BeTrue($"File {crt} should exist but was not found.");
            }
        }


        [TestMethod]
        public void Expected_Env_File_Generated()
        {
            // Arrange
            string[] argument = [_settings.CommandName,
                                 GenAllSettings.SettingOptionName[0],
                                 .. settingArgs.GetStringEnumerable(),];
            DaprifyCommand<GenAllService, GenAllSettings> sut = new(_service, _settings, _validator);

            // Act
            sut.Parse(argument).Invoke();

            // Assert
            Directory.SetCurrentDirectory(MyPath.Combine(DirectoryService.GetCurrentDirectory().ToString(), DAPR_DIR).ToString());
            File.Exists(ENV_DIR + ENV_FILE).Should().BeTrue($"File {ENV_FILE} should exist but was not found.");
        }


        [TestMethod]
        public void Expected_Component_Generated()
        {
            // Arrange
            string[] argument = [_settings.CommandName, GenAllSettings.ComponentOptionName[0],
                                .. componentArgs.GetStringEnumerable()];
            DaprifyCommand<GenAllService, GenAllSettings> sut = new(_service, _settings, _validator);

            // Act
            sut.Parse(argument).Invoke();

            // Assert
            Directory.SetCurrentDirectory(MyPath.Combine(DirectoryService.GetCurrentDirectory().ToString(), DAPR_DIR).ToString());
            string consoleOutput = _consoleOutput.ToString();

            foreach (string comp in componentArgs.GetStringEnumerable())
            {
                consoleOutput.Should().Contain(comp);
                File.Exists(COMPONENT_DIR + comp + FILE_EXT).Should().BeTrue($"File {comp + FILE_EXT} should exist but was not found.");
            }
        }


        [TestMethod]
        public void Expected_Config_Generated()
        {
            // Arrange
            string[] argument = [_settings.CommandName];
            DaprifyCommand<GenAllService, GenAllSettings> sut = new(_service, _settings, _validator);

            // Act
            sut.Parse(argument).Invoke();

            // Assert
            Directory.SetCurrentDirectory(MyPath.Combine(DirectoryService.GetCurrentDirectory().ToString(), DAPR_DIR).ToString());
            string consoleOutput = _consoleOutput.ToString();

            consoleOutput.Should().Contain("config");
            File.Exists(CONFIG_DIR + CONFIG_FILE).Should().BeTrue($"File {CONFIG_FILE} should exist but was not found.");
        }

        [TestMethod]
        public void Expected_Compose_Generated()
        {
            // Arrange
            string[] argument = [_settings.CommandName,
                                GenAllSettings.ComponentOptionName[0],
                                .. composeArgs.GetStringEnumerable(),
                                GenAllSettings.SettingOptionName[0],
                                .. settingArgs.GetStringEnumerable()];
            DaprifyCommand<GenAllService, GenAllSettings> sut = new(_service, _settings, _validator);

            // Act
            sut.Parse(argument).Invoke();

            // Assert
            Directory.SetCurrentDirectory(MyPath.Combine(DirectoryService.GetCurrentDirectory().ToString(), DAPR_DIR).ToString());
            File.Exists(DOCKER_DIR + COMPOSE_FILE).Should().BeTrue($"File {COMPOSE_FILE} should exist but was not found.");
        }

        [TestMethod]
        public void Expected_Config_File_Generates_All()
        {
            // Arrange
            MyPath destDir = MyPath.Combine(DirectoryService.GetCurrentDirectory().ToString(), "Mocks");
            Directory.CreateDirectory(destDir.ToString());

            MyPath destPath = MyPath.Combine(destDir.ToString(), "config-mock.json");
            File.Copy(_confPath.ToString(), destPath.ToString(), true);
            string[] argument = [_settings.CommandName, GenAllSettings.ConfigOptionName[0], destPath.ToString()];
            DaprifyCommand<GenAllService, GenAllSettings> sut = new(_service, _settings, _validator);

            // Act
            sut.Parse(argument).Invoke();

            // Assert
            OptionDictionary options = OptionDictionary.PopulateFromJson(destPath.ToString());
            Directory.SetCurrentDirectory(MyPath.Combine(DirectoryService.GetCurrentDirectory().ToString(), DAPR_DIR).ToString());
            string consoleOutput = _consoleOutput.ToString();

            foreach (string crt in certFiles.GetStringEnumerable())
            {
                consoleOutput.Should().Contain(crt);
                File.Exists(CERT_DIR + crt).Should().BeTrue($"File {crt} should exist but was not found.");
            }

            Key key = new(GenAllSettings.ComponentOptionName[1].Replace("-", ""));
            foreach (string comp in options.GetAllPairValues(key).GetStringEnumerable())
            {
                consoleOutput.Should().Contain(comp.ToString());
                File.Exists(COMPONENT_DIR + comp + FILE_EXT).Should().BeTrue($"File {comp + FILE_EXT} should exist but was not found.");
            }

            File.Exists(ENV_DIR + ENV_FILE).Should().BeTrue($"File {ENV_FILE} should exist but was not found.");
            File.Exists(CONFIG_DIR + CONFIG_FILE).Should().BeTrue($"File {CONFIG_FILE} should exist but was not found.");
            File.Exists(DOCKER_DIR + COMPOSE_FILE).Should().BeTrue($"File {COMPOSE_FILE} should exist but was not found.");
        }


        [TestCleanup]
        public void Cleanup()
        {
            _consoleOutput.GetStringBuilder().Clear();
            DirectoryService.DeleteDirectory(DirectoryService.GetCurrentDirectory());
            Directory.SetCurrentDirectory(_startingDir.FullName);
        }
    }
}