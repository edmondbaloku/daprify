using Microsoft.Extensions.DependencyInjection;

namespace Daprify.Templates
{
    public interface ITemplateFactory
    {
        string CreateTemplate<Temp>() where Temp : TemplateBase;
    }


    public class TemplateFactory(IServiceProvider serviceProvider) : ITemplateFactory
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public string CreateTemplate<Temp>() where Temp : TemplateBase
        {
            Temp _template = _serviceProvider.GetRequiredService<Temp>();
            return _template.GetTemplate();
        }

        public Temp GetTemplateService<Temp>() where Temp : TemplateBase
        {
            return _serviceProvider.GetRequiredService<Temp>();
        }
    }
}