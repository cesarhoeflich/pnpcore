# Working with files

Working with files (documents) is a core aspect of working with SharePoint. Learn how to add/upload files, set metadata and download files again and much more.

In the remainder of this article you'll see a lot of `context` use: in this case this is a `PnPContext` which was obtained via the `PnPContextFactory` as explained in the [overview article](readme.md) and show below:

```csharp
using (var context = await pnpContextFactory.CreateAsync("SiteToWorkWith"))
{
    // See next chapter on how to use the PnPContext for working with files
}
```

## Getting files

In PnP Core SDK files are represented via an [IFile interface](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IFile.html). Before you get perform a file operation (e.g. like publish or download) you need to get the file as [IFile](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IFile.html). There are a number of ways to get an [IFile](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IFile.html) like loading a single file via a lookup or enumerating the files in library / folder.

### Getting a single file

If you know the name and location of a file you can get a reference to it via the [IWeb](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IWeb.html) method named [GetFileByServerRelativeUrl](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IWeb.html#collapsible-PnP_Core_Model_SharePoint_IWeb_GetFileByServerRelativeUrlAsync_System_String_Expression_Func_PnP_Core_Model_SharePoint_IFile_System_Object_____). This method takes a server relative path of the file and optionally allows you to specify which properties to load on the file.

```csharp
string documentUrl = $"{context.Uri.PathAndQuery}/Shared Documents/document.docx";

// Get a reference to the file
IFile testDocument = await context.Web.GetFileByServerRelativeUrlAsync(documentUrl);

// Get a reference to the file, loading extra properties of the IFile 
IFile testDocument = await context.Web.GetFileByServerRelativeUrlAsync(documentUrl, f => f.CheckOutType, f => f.CheckedOutByUser);
```

### Getting the file of a list item

A document in a document library is an `IListItem` holding the file metadata with an `IFile` holding the actual file. If you have an `IListItem` you can load the connected file via `File` property:

```csharp
var myList = await context.Web.Lists.GetByTitleAsync("My List");

// Load list item with id 1 with it's file
var first = await myList.Items.GetByIdAsync(1, li => li.All, li => li.File);

// Use the loaded IFile, e.g. for downloading it
byte[] downloadedContentBytes = await first.File.GetContentBytesAsync();
```

### Enumerating files

Files do live in an [IFolder](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IFolder.html), document libraries do have a [RootFolder property](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IList.html#PnP_Core_Model_SharePoint_IList_RootFolder) allowing you to enumerate files, but also the [IWeb](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IWeb.html) has a [collection of Folders](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IWeb.html#PnP_Core_Model_SharePoint_IWeb_Folders), a [RootFolder](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IWeb.html#collapsible-PnP_Core_Model_SharePoint_IWeb_RootFolder) and [GetFolderByIdAsync](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IWeb.html#collapsible-PnP_Core_Model_SharePoint_IWeb_GetFolderByIdAsync_Guid_Expression_Func_PnP_Core_Model_SharePoint_IFolder_System_Object_____) and [GetFolderByServerRelativeUrlAsync](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IWeb.html#collapsible-PnP_Core_Model_SharePoint_IWeb_GetFolderByServerRelativeUrlAsync_System_String_Expression_Func_PnP_Core_Model_SharePoint_IFolder_System_Object_____) methods. Once you've a folder you can enumerate the files inside it.

```csharp
// Get root folder of a library
IFolder folder = await context.Web.Folders.GetFirstOrDefaultAsync(f => f.Name == "SiteAssets");

// Get root folder of the web (for files living outside of a document library)
IFolder folder = (await context.Web.GetAsync(p => p.RootFolder)).RootFolder;

// Get folder collection of a web and pick the SiteAssets folder
await context.Web.LoadAsync(p => p.Folders);
var folder = context.Web.Folders.AsRequested().FirstOrDefault(p=>p.Name == "SiteAssets");

// Load files property of the folder
await folder.LoadAsync(p => p.Files);

foreach(var file in folder.Files.AsRequested())
{
    // Do something with the file
}
```

## Getting file properties

A file in SharePoint has properties which can be requested by loading them on the [IFile](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IFile.html). Below snippet shows some ways on how to load file properties.

```csharp
string documentUrl = $"{context.Uri.PathAndQuery}/Shared Documents/document.docx";

// Sample 1: Get a reference to the file
IFile testDocument = await context.Web.GetFileByServerRelativeUrlAsync(documentUrl);

// Sample 2: Get a reference to the file, loading the file Author and ModifiedBy properties
IFile testDocument = await context.Web.GetFileByServerRelativeUrlAsync(documentUrl, w => w.Author, w => w.ModifiedBy);

// Sample 3: Get files by loading it's folder and the containing files with their selected properties
var folder = await context.Web.GetFolderByServerRelativeUrlAsync($"{context.Uri.PathAndQuery}/SiteAssets", 
                    p => p.Name, p => p.Files.QueryProperties(p => p.Name, p => p.Author, p => p.ModifiedBy));
foreach(var file in folder.Files.AsRequested())
{
    // Do something with the file, properties Name, Author and ModifiedBy are loaded
}
```

### File property bag

Each file also has a so called property bag, a list key/value pairs providing more information about the file. You can read this property bag, provided via the [IFile Properties property](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IFile.html#collapsible-PnP_Core_Model_SharePoint_IFile_Properties), and add new key/value pairs to it.

```csharp
string documentUrl = $"{context.Uri.PathAndQuery}/Shared Documents/document.docx";

// Get a reference to the file, load the file property bag
IFile testDocument = await context.Web.GetFileByServerRelativeUrlAsync(documentUrl, f => f.Properties);

// Enumerate the file property bag
foreach(var property in testDocument.Properties)
{
    // Do something with the property
}

// Add a new property
testDocument["myPropertyKey"] = "Some value";
await testDocument.Properties.UpdateAsync();
```

## Publishing and un-publishing files

Publishing a file will move the file from draft into published status and increase it's major version by one. Publishing can be done using the [PublishAsync method](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IFile.html#PnP_Core_Model_SharePoint_IFile_PublishAsync_System_String_), un-publishing a file will bring the file back to draft status and can be done using the [UnPublishAsync method](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IFile.html#PnP_Core_Model_SharePoint_IFile_UnpublishAsync_System_String_).

>[!Note]
> Publishing a file requires the library to be configured to support major versions. See the [EnableVersioning property on the IList interface](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IList.html#PnP_Core_Model_SharePoint_IList_EnableVersioning).

```csharp
string documentUrl = $"{context.Uri.PathAndQuery}/Shared Documents/document.docx";

// Get a reference to the file, load the file property bag
IFile testDocument = await context.Web.GetFileByServerRelativeUrlAsync(documentUrl);

// Publish the file
await testDocument.PublishAsync("Optional publish message");

// Un-publish the file
await testDocument.UnpublishAsync("Optional un-publish message");
```

## Checking out, undoing check out and checking in files

In SharePoint a file can be checked out by a user to "lock" the file and then later on checked in again. The same can be done using code, including undoing a checked out of another user via the [CheckoutAsync](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IFile.html#PnP_Core_Model_SharePoint_IFile_CheckoutAsync), [CheckinAsync](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IFile.html#PnP_Core_Model_SharePoint_IFile_CheckinAsync_System_String_PnP_Core_Model_SharePoint_CheckinType_) and [UndoCheckout](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IFile.html#PnP_Core_Model_SharePoint_IFile_UndoCheckoutAsync) methods.

>[!Note]
> Publishing a file requires the library to be configured to support major versions. See the [ForceCheckout property on the IList interface](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IList.html#collapsible-PnP_Core_Model_SharePoint_IList_ForceCheckout).

```csharp
string documentUrl = $"{context.Uri.PathAndQuery}/Shared Documents/document.docx";

// Get a reference to the file
IFile testDocument = await context.Web.GetFileByServerRelativeUrlAsync(documentUrl);

// Check out the file
await testDocument.CheckoutAsync();

// Check in the file
await testDocument.CheckinAsync();
```

Undoing a checkout:

```csharp
// Get the default document library root folder
string documentUrl = $"{context.Uri.PathAndQuery}/Shared Documents/document.docx";

// Get a reference to the file, load the check out information as that can be needed before undoing the check out
IFile testDocument = await context.Web.GetFileByServerRelativeUrlAsync(documentUrl, f => f.CheckOutType, f => f.CheckedOutByUser);

// Checkout the file
await testDocument.UndoCheckoutAsync();
```

## Deleting and recycling files

You can delete a file (permanent operation) or move it to the site's recycle bin (the file can be restored). Deleting a file is done using the typical Delete methods like [DeleteAsync](https://pnp.github.io/pnpcore/api/PnP.Core.Model.IDataModelDelete.html#PnP_Core_Model_IDataModelDelete_DeleteAsync), recycling is done via [RecycleAsync](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IFile.html#PnP_Core_Model_SharePoint_IFile_RecycleAsync).

```csharp
string documentUrl = $"{context.Uri.PathAndQuery}/Shared Documents/document.docx";

// Get a reference to the file
IFile testDocument = await context.Web.GetFileByServerRelativeUrlAsync(documentUrl);

// Recycle the file
await testDocument.RecycleAsync();

// Delete the file
await testDocument.DeleteAsync();
```

## Adding files (=uploading)

Adding a file comes down to create a file reference and uploading the file's bytes and this can be done via the [AddAsync method on a Files collection](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IFileCollection.html#PnP_Core_Model_SharePoint_IFileCollection_AddAsync_System_String_Stream_System_Boolean_). This method takes a stream of bytes as input for the file contents.

>[!Note]
> See the [working with large files](files-large.md) page for some more complete file upload/download samples.

```csharp
// Get a reference to a folder
IFolder siteAssetsFolder = await context.Web.Folders.Where(f => f.Name == "SiteAssets").FirstOrDefaultAsync();

// Upload a file by adding it to the folder's files collection
IFile addedFile = await siteAssetsFolder.Files.AddAsync("test.docx", 
                  System.IO.File.OpenRead($".{Path.DirectorySeparatorChar}TestFilesFolder{Path.DirectorySeparatorChar}test.docx"));
```

## Updating file metadata

The library in which you've uploaded a file might have additional columns to store metadata about the file. To update this metadata you first need to load the `IListItem` linked to the added file via the `ListItemAllFields` property, followed by setting the metadata and updating the `IListItem`.

>[!Note]
> See the [working with list items](listitems-intro.md) page for information on how to update an `IListItem`.

```csharp
// Get a reference to a folder
IFolder documentsFolder = await context.Web.Folders.Where(f => f.Name == "Documents").FirstOrDefaultAsync();

// Upload a file by adding it to the folder's files collection
IFile addedFile = await documentsFolder.Files.AddAsync("test.docx", 
                  System.IO.File.OpenRead($".{Path.DirectorySeparatorChar}TestFilesFolder{Path.DirectorySeparatorChar}test.docx"));
// Load the corresponding ListItem
await addedFile.ListItemAllFields.LoadAsync();
// Set the metadata
addedFile.ListItemAllFields["Field1"] = "Hi there";
addedFile.ListItemAllFields["Field2"] = true;
// Persist the ListItem changes
await addedFile.ListItemAllFields.UpdateAsync();
```

## Downloading files

If you want to download a file you do need to use either the [GetContentAsync method](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IFile.html#PnP_Core_Model_SharePoint_IFile_GetContentAsync_System_Boolean_) if you prefer a Stream as result type or [GetContentBytesAsync](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IFile.html#collapsible-PnP_Core_Model_SharePoint_IFile_GetContentBytesAsync) if you prefer a byte array.

>[!Note]
> See the [working with large files](files-large.md) page for some more complete file upload/download samples.

```csharp
string documentUrl = $"{context.Uri.PathAndQuery}/Shared Documents/document.docx";

// Get a reference to the file
IFile testDocument = await context.Web.GetFileByServerRelativeUrlAsync(documentUrl);

// Download the file as stream
Stream downloadedContentStream = await testDocument.GetContentAsync();

// Download the file as an array of bytes
byte[] downloadedContentBytes = await testDocument.GetContentBytesAsync();
```

## Copying and moving files

A file can be copied or moved into another SharePoint location and this can be done using the [CopyToAsync](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IFile.html#PnP_Core_Model_SharePoint_IFile_CopyToAsync_System_String_System_Boolean_PnP_Core_Model_SharePoint_MoveCopyOptions_) and [MoveToAsync](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IFile.html#PnP_Core_Model_SharePoint_IFile_MoveToAsync_System_String_PnP_Core_Model_SharePoint_MoveOperations_PnP_Core_Model_SharePoint_MoveCopyOptions_) methods.

```csharp
string documentUrl = $"{context.Uri.PathAndQuery}/Shared Documents/document.docx";

// Get a reference to the file
IFile testDocument = await context.Web.GetFileByServerRelativeUrlAsync(documentUrl);

// Copy the file, overwrite if existing on the destination
await testDocument.CopyToAsync($"{context.Uri.PathAndQuery}/MyDocuments/document.docx", true);

// Move the file, overwrite if needed
await testDocument.MoveToAsync($"{context.Uri.PathAndQuery}/MyDocuments/document.docx", MoveOperations.Overwrite);
```

## Getting file versions

When versioning on a file is enabled a file can have multiple versions and PnP Core SDK can be used to work with the older file versions. Each file version is represented via an [IFileVersion](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IFileVersion.html) in an [IFileVersionCollection](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IFileVersionCollection.html). Loading file versions can be done by requesting the [Versions property](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IFile.html#PnP_Core_Model_SharePoint_IFile_Versions) of the file. Once you've an [IFileVersion](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IFileVersion.html) you can also download that specific version of the file by using one of the GetContent methods as shown in the example.

>[!Note]
> For a file to have versions the library needs to be configured to support major versions and/or minor versions. See the [EnableVersioning](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IList.html#PnP_Core_Model_SharePoint_IList_EnableVersioning) and [EnableMinorVersions](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IList.html#PnP_Core_Model_SharePoint_IList_EnableMinorVersions) properties on the IList interface.

```csharp
string documentUrl = $"{context.Uri.PathAndQuery}/Shared Documents/document.docx";

// Get a reference to the file, also request the Versions property
IFile testDocument = await context.Web.GetFileByServerRelativeUrlAsync(documentUrl, f => f.Versions);

foreach(var fileVersion in testDocument.Versions)
{
    // Download the file version content as stream
    Stream fileVersionContent = await fileVersion.GetContentAsync();
}
```

## Getting file IRM settings

A SharePoint document library can be configured with an [Information Rights Management (IRM) policy](https://docs.microsoft.com/en-us/microsoft-365/compliance/set-up-irm-in-sp-admin-center?view=o365-worldwide) which then stamps an IRM policy on the documents obtained from that library. Use the [InformationRightsManagementSettings](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IFile.html#PnP_Core_Model_SharePoint_IFile_InformationRightsManagementSettings) property to read the file's IRM settings.

>[!Note]
> The library holding files you want to protect need to be first setup for IRM by enabling IRM on it via the [IrmEnabled property](https://pnp.github.io/pnpcore/api/PnP.Core.Model.SharePoint.IList.html#collapsible-PnP_Core_Model_SharePoint_IList_IrmEnabled).

```csharp
string documentUrl = $"{context.Uri.PathAndQuery}/Shared Documents/document.docx";

// Get a reference to the file, also request the Versions property
IFile testDocument = await context.Web.GetFileByServerRelativeUrlAsync(documentUrl, f => f.InformationRightsManagementSettings);

var fileAccessExpirationTimeInDays = testDocument.InformationRightsManagementSettings.DocumentAccessExpireDays;
```
