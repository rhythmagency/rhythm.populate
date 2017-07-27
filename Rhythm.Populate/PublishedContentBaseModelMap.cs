namespace Rhythm.Populate
{
    internal class PublishedContentBaseModelMap : ContentMapping<PublishedContentBaseModel>
    {
        public PublishedContentBaseModelMap()
        {
            Map.Property(x => x.Id, c => c.Id);
            Map.Property(x => x.Name, c => c.Name);
            Map.Property(x => x.Url, c => c.Url);
            Map.Property(x => x.PublishedContent, c => c);
        }
    }
}