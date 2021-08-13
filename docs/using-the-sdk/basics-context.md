# Advanced PnPContext use

Requesting a `PnPContext` is something each application does, using the `PnPContextFactory` as explained in the [overview article](readme.md). Whereas the overview article describes how to get a `PnPContext` there are additional possibilities when requesting and working with a `PnPContext`.

## Loading additional IWeb and ISite properties when creating a PnPContext

When a `PnPContext` is created two calls are issued to SharePoint Online. In a first call the `IWeb` is loaded with following properties: `Id`, `Url` and `RegionalSettings`. In the second call `ISite` is loaded with the `Id` and `GroupId` properties. If your application needs additional `IWeb` or `ISite` properties you can optimize the number of server roundtrips by adding the extra needed properties to the already planned requests for loading `IWeb` and `ISite`. To do this you can provide a `PnPContextOptions` object specifying the additional `IWeb` and `ISite` properties to load.

```csharp
var options = new PnPContextOptions()
{
    AdditionalSitePropertiesOnCreate = new Expression<Func<ISite, object>>[] { s => s.Url, s => s.HubSiteId },
    AdditionalWebPropertiesOnCreate = new Expression<Func<IWeb, object>>[] { w => w.ServerRelativeUrl }
};

using (var context = await pnpContextFactory.CreateAsync("SiteToWorkWith", options))
{
    // the created context has besides the default properties also the Url and HubSiteId ISite properties loaded + 
    // the ServerRelativeUrl IWeb property.
}
```

While above sample just loaded some basic `IWeb` and `ISite` properties you can also do more complex LINQ expressions.

```csharp
var options = new PnPContextOptions()
{
    AdditionalSitePropertiesOnCreate = 
        new Expression<Func<ISite, object>>[] { s => s.Url, s => s.HubSiteId,
                                                s => s.Features },
    AdditionalWebPropertiesOnCreate = 
        new Expression<Func<IWeb, object>>[] 
        {   
            w => w.ServerRelativeUrl,
            w => w.Fields, w => w.Features,
            w => w.Lists.QueryProperties(r => r.Title,
                r => r.RootFolder.QueryProperties(p => p.ServerRelativeUrl)) 
        }
};

using (var context = await pnpContextFactory.CreateAsync("SiteToWorkWith", options))
{
    // the created context has besides the default properties also the Url, HubSiteId and Features ISite 
    // properties loaded + the ServerRelativeUrl, Fields, Features, Lists with RootFolder IWeb properties.
}
```

Any expression that you'd normally would write to retrieve data can also be specified to be included in the default context creation.

## Cloning a PnPContext

Once you've created a `PnPContext` you might want to clone it for the same site or for a new web.

### Cloning a PnPContext for a new web

When you clone contexts for a new web the context settings (e.g. `GraphFirst`) are copied over. If you've specified additional properties to load on context creation these properties will also be loaded for the cloned context. So the cloned context will be exactly configured like the original site, but then for the other web url.

```csharp
using (var context = await pnpContextFactory.CreateAsync("SiteToWorkWith"))
{
    // Clone this context using a configuration
    using (var contextClone = await context.CloneAsync("OtherSite"))
    {
        // Use the cloned context
    }

    // Clone this context using a URL
    using (var contextClone = await context.CloneAsync(new Uri("https://contoso.sharepoint.com/sites/anothersite")))
    {
        // Use the cloned context
    }
}
```

### Cloning a PnPContext for the same web

Sometimes you want to use a new `PnPContext` for the same web. You can obviously create a new `PnPContext` via the context factory, but then you do get the initialization cost again. When you clone a `PnPContext` the initialization data will be copied across contexts, just like the settings, so you'll get a functioning `PnPContext` without calling back into SharePoint Online. If you've specified additional properties to load on context creation then only the extra properties which are basic (so no model classes) are copied into the cloned context.

```csharp
var options = new PnPContextOptions()
{
    AdditionalSitePropertiesOnCreate = 
        new Expression<Func<ISite, object>>[] { s => s.Url, s => s.HubSiteId,
                                                s => s.Features },
    AdditionalWebPropertiesOnCreate = 
        new Expression<Func<IWeb, object>>[] 
        {   
            w => w.ServerRelativeUrl,
            w => w.Fields, w => w.Features,
            w => w.Lists.QueryProperties(r => r.Title,
                r => r.RootFolder.QueryProperties(p => p.ServerRelativeUrl)) 
        }
};

using (var context = await pnpContextFactory.CreateAsync("SiteToWorkWith" , options))
{
    // Clone this context using a configuration
    using (var contextClone = await context.CloneAsync("SiteToWorkWith"))
    {
        // Use the cloned context. Without going to the server the default IWeb and ISite properties are loaded
        // + the ISite Url and HubSiteId properties
        // + the IWeb ServerRelativeUrl property
        // Other extra initialization properties are model classes and are not copied over
    }
}
```
