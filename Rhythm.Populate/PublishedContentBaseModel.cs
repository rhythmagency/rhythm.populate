using Umbraco.Core.Models;

namespace Rhythm.Populate
{
    public abstract class PublishedContentBaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public IPublishedContent PublishedContent { get; set; }

        //prevent JSON.Net serializer
        //see: http://james.newtonking.com/json/help/index.html?topic=html/ConditionalProperties.htm
        //Using this method instead of [JsonIgnore] so as not to take a dependency on JSON.Net
        public bool ShouldSerializePublishedContent()
        {
            return false;
        }
    }
}