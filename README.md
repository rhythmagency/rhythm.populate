# Rhythm Populate
### A library for mapping Umbraco documents to POCOs

## Basic Usage

### Document Types

Assuming the following `DocumentType`s exist for example purposes:

#### BlogPost

A `DocumentType` with properties:
* Title (textfield)
* Body (richtext)
* Header (image picker)
* Related Posts (multi-node content picker)

#### BlogComment

A `DocumentType` with properties
* Author (textfield)
* Date (datepicker)
* Comment (richtext)

`BlogComment`'s can be children nodes of `BlogPost` (yes this is terrible design)

### POCOs

The following POCO classes can be created to represent the `DocumentType`s. (Note: the layout of the POCO class does not need to match the `DocumentType` exactly.  The names of properties can differ, and the POCO can be more or less complex than the `DocumentType`. Populate is flexible enough to handle these scenarios through the use of `ComponentMapping`s.)  POCOs can also inherit, or be interfaces of, other mapped POCOs, and Populate will understand the hierarchy and map the full chain of POCOs.

#### BlogPost

```c#
public class BlogPost : PublishedContentBaseModel
{
    public string Title { get; set; }
    public string Body { get; set; }
    public Image Header { get; set; }
    public IEnumerable<BlogPost> RelatedPosts { get; set; }
    public IEnumerable<BlogComment> Comments { get; set; }
}
```

(Note) Inherting from `PublishedContentBaseModel` is not required, but will automatically give you access to some built-in Umbraco properties such as Id, Name an Url, as well as access the the underlying `PublishedContent`.

#### BlogComment

```c#
public class BlogComment : PublishedContenBaseModel
{
    public string Author { get; set; }
    public DateTime Date { get; set; }
    public string Comment { get; set; }
    public BlogPost Parent { get; set; }
}
```

#### Image

```c#
public class Image : PublishedContentBaseModel
{
    public int Width { get; set; }
    public int Height { get; set; }
}
```

### Mappings

To map the Umbraco `DocumentType`s to the POCOs, the following mapping classes need to be created:

#### BlogPostMap

```c#
public class BlogPostMap : ContentMapping<BlogPost>
{
    public BlogPostMap()
    {
        Alias("BlogPost"); //the alias assigned to this DocumentType by Umbraco

        Map.Property(x => x.Title); //optional second parameter to specify the alias if it differs from the property name
        Map.Property(x => x.Body);
        Map.Node(x => x.Header).AsMedia();
        Map.NodeCollection(x => x.RelatedPosts).Eager(); //collections default to "lazy" loading, but this can be overriden here in the mapping.
        Map.Children(x => x.Comments, "BlogComment");
    }
}
```

(Note) Usage of Map.* seems redundant, but is necessary to have a hook for extention methods to support additional mapping types for Archetype which is distributed in a separate DLL.

#### BlogCommentMap

```c#
public class BlogCommentMap : ContentMapping<BlogComment>
{
    public BlogCommentMap()
    {
        Alias("BlogComment");

        Map.Property(x => x.Author);
        Map.Property(x => x.Date);
        Map.Property(x => x.Comment);
        Map.Parent(x => x.Parent);
    }
}
```

#### ImageMap

```c#
public class ImageMap : ContentMapping<ImageMap>
{
    public ImageMap()
    {
        Map.Property(x => x.Width, Umbraco.Core.Constants.Conventions.Media.Width);
        Map.Property(x => x.Height, Umbraco.Core.Constants.Conventions.Media.Height);
    }
}
```

### Initialization

To let Populate know about the mappings, somewhere in your app startup include the following:

`Populate.AddMappingsFromAssemblyOf<BlogPostMap>();`

Alternatively you can call

`Populate.AddMappingFromAssembly()` and pass in the Assembly that contains your mappings

### Populating Data

All access to data starts via a `MappingSession`.  The session acts as a cache and identity map for all populated data.  This prevents issues related to circular references, and maintains reference identity when comparing objects.  It's recommended to use one session per web request.

Inside your template page CSHTML, or some other location with a valid `UmbracoContext`, get the current session by calling:

`var session = Populate.GetCurrentSessionForRequest();`

If we are currently on the template page for a `BlogPost` we can then call:

`var blogPost = session.Map<BlogPost>(Model.Content).Single();`

which will map the passed in `PublishedContent` to our `BlogPost` model.  The `Single()` method is used when the content is not a collection, otherwise use `List()`.

In the above mapping the returned `BlogPost` will have the `RelatedPost` property mapped automatically since in the mapping we set it as `Eager()`.  However the `Comments` property will be `NULL` since by default Populate will not load collections.  (This prevents scenarios where it's easy to pull in large sections of your content tree depending on how related they are).  To specifically request that the `Comments` property also be mapped we can alter the call to `session.Map` to be:

```c#
var blogPost = session.Map<BlogPost>(Model.Content)
                .Include(x => x.Comments)
                .Single();
```

`Session.Map()` can take various types of parameters.  It can be an `IPublishedContent`, or an `int` nodeId, or an `IEnumerable<IPublishedContent>`.  In advanced scenarios, the mapping can also contain a "Content Source" which can tell Populate how to find all instances of a particular `DocumentType`.  In that case the call can be reduced to `session.Map<BlogPost>.List()` and it will find and populate the data for all `BlogPost`s.



