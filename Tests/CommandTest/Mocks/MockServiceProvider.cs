
using CLI.Templates;
using Moq;

namespace CLITests
{
    public class MockServiceProvider : Mock<IServiceProvider>
    {
        private readonly ComposeStartTemplate composeStartTemplate = new();
        private readonly ComposeTemplate composeTemplate = new();
        private readonly ConfigTemplate configTemplate = new();
        private readonly EnvTemplate envTemplate = new();
        private readonly MtlsCompTemplate mTlsCompTemplate = new();
        private readonly MTlsTemplate mTlsTemplate = new();
        private readonly PubSubTemplate pubSubTemplate = new();
        private readonly RabbitMqTemplate rabbitMqTemplate = new();
        private readonly RedisTemplate redisTemplate = new();
        private readonly SecretStoreTemplate secretStoreTemplate = new();
        private readonly StateStoreTemplate stateStoreTemplate = new();
        private readonly TracingTemplate tracingTemplate = new();

        public MockServiceProvider()
        {
            Setup(x => x.GetService(typeof(ComposeStartTemplate))).Returns(composeStartTemplate);
            Setup(x => x.GetService(typeof(ComposeTemplate))).Returns(composeTemplate);
            Setup(x => x.GetService(typeof(ConfigTemplate))).Returns(configTemplate);
            Setup(x => x.GetService(typeof(EnvTemplate))).Returns(envTemplate);
            Setup(x => x.GetService(typeof(MtlsCompTemplate))).Returns(mTlsCompTemplate);
            Setup(x => x.GetService(typeof(MTlsTemplate))).Returns(mTlsTemplate);
            Setup(x => x.GetService(typeof(PubSubTemplate))).Returns(pubSubTemplate);
            Setup(x => x.GetService(typeof(RabbitMqTemplate))).Returns(rabbitMqTemplate);
            Setup(x => x.GetService(typeof(RedisTemplate))).Returns(redisTemplate);
            Setup(x => x.GetService(typeof(SecretStoreTemplate))).Returns(secretStoreTemplate);
            Setup(x => x.GetService(typeof(StateStoreTemplate))).Returns(stateStoreTemplate);
            Setup(x => x.GetService(typeof(TracingTemplate))).Returns(tracingTemplate);
        }
    }
}