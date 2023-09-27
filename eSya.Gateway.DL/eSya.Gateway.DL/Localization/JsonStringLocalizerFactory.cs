using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;

namespace eSya.Gateway.DL.Localization
{
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private IHostingEnvironment _environment;

        public JsonStringLocalizerFactory(IHostingEnvironment environment)
        {
            _environment = environment;
        }
        public IStringLocalizer Create(Type resourceSource)
        {
            return new JsonStringLocalizer(_environment);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new JsonStringLocalizer(_environment);
        }
    }
}
