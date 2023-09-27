using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
//using System.Text.Json;
using System.Threading.Tasks;

namespace eSya.Gateway.DL.Localization
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        private readonly JsonSerializer _serializer = new JsonSerializer();
        private IHostingEnvironment _environment;
        public JsonStringLocalizer(IHostingEnvironment environment)
        {
            _environment = environment;
        }
        public LocalizedString this[string name]
        {
            get
            {
                var value = GetString(name);
                return new LocalizedString(name, value);
            }
        }

        public LocalizedString this[string name, params object[] agruments]
        {
            get
            {
                var actualValue = this[name];
                return !actualValue.ResourceNotFound
                    ? new LocalizedString(name, string.Format(actualValue.Value, agruments))
                    : actualValue;
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var filePath = $"Resources/{Thread.CurrentThread.CurrentCulture.Name}.json";

            //string fullFilePath = Path.Combine(this._environment.WebRootPath, filepath);

            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                StreamReader streamreader = new StreamReader(stream);
                JsonTextReader reader = new JsonTextReader(streamreader);
                while (reader.Read())
                {
                    if (reader.TokenType != JsonToken.PropertyName)

                        continue;
                    var key = reader.Value as string;
                    reader.Read();
                    var value = _serializer.Deserialize<string>(reader);
                    yield return new LocalizedString(key, value);
                }
            }
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private string GetString(string key)
        {



            var filepath = $"Resources/{Thread.CurrentThread.CurrentCulture.Name}.json";

            string fullFilePath = Path.Combine(this._environment.WebRootPath, filepath);

            if (File.Exists(fullFilePath))
            {
                var result = GetValueFromJson(key, fullFilePath);

                if (string.IsNullOrEmpty(result))
                {
                    var defaultkeyfilepath = $"Resources/{"en-US"}.json";
                    string defaultkeyfullFilePath = Path.Combine(this._environment.WebRootPath, defaultkeyfilepath);
                    var defaultresult = GetValueFromJson(key, defaultkeyfullFilePath);
                    result = defaultresult;
                }
                return result;
            }
            else
            {
                var defaultfilepath = $"Resources/{"en-US"}.json";

                string defaultfullFilePath = Path.Combine(this._environment.WebRootPath, defaultfilepath);

                var result = GetValueFromJson(key, defaultfullFilePath);


                if (string.IsNullOrEmpty(result))
                {
                    var defaultkeyfilepath = $"Resources/{"en-US"}.json";
                    string defaultkeyfullFilePath = Path.Combine(this._environment.WebRootPath, defaultkeyfilepath);
                    var defaultresult = GetValueFromJson(key, defaultkeyfullFilePath);
                    result = defaultresult;
                    result = key;
                }

                return result;
            }
            //return string.Empty;
        }

        private string GetValueFromJson(string propertyName, string filePath)
        {
            if (string.IsNullOrEmpty(propertyName) || string.IsNullOrEmpty(filePath))
            {
                return string.Empty;

            }
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                StreamReader streamreader = new StreamReader(stream);
                JsonTextReader reader = new JsonTextReader(streamreader);
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName && reader.Value as string == propertyName)
                    {
                        reader.Read();
                        return _serializer.Deserialize<string>(reader);
                    }
                }
                return string.Empty;
            }

        }
    }
}
