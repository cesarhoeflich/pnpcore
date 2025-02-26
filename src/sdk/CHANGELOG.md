# PnP Core SDK Changelog

*Please do not commit changes to this file, it is maintained by the repo owner.*

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/).

## [Unreleased]

### Added

- Added IWeb.GetFileByServerRelativeUrlOrDefault methods that can return null for non existing files + dropped the IFile.Exists property as it never really worked [jansenbe - Bert Jansen]

### Changed

## [1.3.0]

### Added

- Support for working with IListItem attachments [jansenbe - Bert Jansen]
- Page scheduling support: enable a pages library for page publish scheduling and schedule the publishing of a page [jansenbe - Bert Jansen]
- IsSubSite methods on IWeb [jansenbe - Bert Jansen]
- Support for converting the site's datetime to UTC and vice versa [jansenbe - Bert Jansen]
- Using @mentioning in page/listitem comments and replies [jansenbe - Bert Jansen]
- Support for downloading versions of a file #483 [DaleyKD - Kyle Daley]
- Support for ListItem versions #481 [DaleyKD - Kyle Daley]
- Support for liking/unliking pages + adding/deleting comments and comment replies + like/unlike these comments on pages [jansenbe - Bert Jansen]
- Support for adding/deleting comments and comment replies + like/unlike these comments on list items [jansenbe - Bert Jansen]

### Changed

- Reliabilty update when parsing taxonomy field json [jansenbe - Bert Jansen]
- Throw exception when using a REST based list item add/update while only the user id is specified [jansenbe - Bert Jansen]
- "Special" field value types (User, Lookup, Url, Taxonomy) were wrongly marked as changed on data load causing blanking out data with item updates [jansenbe - Bert Jansen]
- Fixed the SharePoint Syntex ClassifyAndExtract functionality to skip folders and work well with large lists [jansenbe - Bert Jansen]
- Fixed issue when using AddValidateUpdateItemUsingPath (used for IlistItem adds) is returning a field level add error - now an exception is thrown [jansenbe - Bert Jansen]
- Fixed memory leaks for use cases that resulted in repeated creation of the Azure AppInsights TelemetryClient [jansenbe - Bert Jansen]
- Fixed bug that resulted in an unneeded query due to paging #486 [PaoloPia - Paolo Pialorsi]

## [1.2.0]

### Added

- Support for getting changes on IWeb, ISite, IList, IListItem and IFolder [DaleyKD - Kyle Daley]
- Added support batching custom SPO REST / Microsoft Graph calls [jansenbe - Bert Jansen]
- Added support to request additional Site/Web properties to be loaded as part of the PnPContext initialization [DaleyKD - Kyle Daley] and [jansenbe - Bert Jansen]
- Added support for adding custom HTTP headers when executing a custom apirequest via ExecuteRequest #459 [czullu - Christian Zuellig]
- Added support to load an IFile directly off of an IListItem #463 [DaleyKD - Kyle Daley]
- Added missing properties to IListItem and GetDisplayName methods #452 [DaleyKD - Kyle Daley]
- Added missing properties to IFile #445 [DaleyKD - Kyle Daley]
- Support for making custom SPO REST / Microsoft Graph calls [jansenbe - Bert Jansen]
- IsFile() method on IListItem [jansenbe - Bert Jansen]

### Changed

- Upgrade to version 4.30.1 of Microsoft.Identity.Web [jansenbe - Bert Jansen]
- Upgrade to version 6.11.1 of System.IdentityModel.Tokens.Jwt [jansenbe - Bert Jansen]
- Optimized initial load to speed up detection of "Templates" folder [jansenbe - Bert Jansen]
- Detect existing templates folder if it's not named "Templates" [jansenbe - Bert Jansen]
- Handle SPO REST's replacement of _ with _x005f_ and space with _x0020_ when deserializing data [jansenbe - Bert Jansen]
- Added ChangeTokenOptions class to allow creation of an IChangeToken #471 [DaleyKD - Kyle Daley]
- Fixed an issue where adding items to the root site collection resulted in a 404 #470 [DaleyKD - Kyle Daley]
- Split batches containing Graph Beta and Graph V1 requests into individual batches [jansenbe - Bert Jansen]
- Prevent unneeded extra requests in a batch when the a previous batch was reused [jansenbe - Bert Jansen]
- Fixed loading of extra properties on retrieved content types [jansenbe - Bert Jansen]
- Renamed GetFolder methods on IListItem to GetParentFolder [jansenbe - Bert Jansen]
- Improved reliability for ChunkedFileUpload #465 [thechriskent - Chris Kent]
- Fixed nested QueryProperties() use on non-collection expandable properties (e.g. List.RootFolder) [jansenbe - Bert Jansen]
- List item documentation update #461 [cesarhoeflich - Cesar Hoeflich]
- Fix for supporting teams apps as web parts on pages [jansenbe - Bert Jansen]
- RenderListDataAsStream populates the IListItem's IContentType [jansenbe - Bert Jansen]
- Fixed NullReferenceException in Values_CollectionChange (issue #447) #445 [DaleyKD - Kyle Daley]
- Ensure the Context.Group model has it's groupid set if that was available [jansenbe - Bert Jansen]
- Use ConcurrentDictionary to track batches in the BatchClient class [jansenbe - Bert Jansen]
- Fix BannerImageUrl to use sphost only since siteid, webid are given as parameter #439 [czullu - Christian Zuellig]
- Alignment of PageHeaderTitleAlignment enum with what's used in PnP Framework [jansenbe - Bert Jansen]
- Hubsite joining/unjoining documention update #416 [plamber - Patrick Lamber]
- Fixed url encoding issue when the .NET Standard 2.0 build was used in a .NET Framework application [jansenbe - Bert Jansen]
- Correctly handle list item data for fields starting with an _ (replace OData__ with _ when parsing the response) [jansenbe - Bert Jansen]
- Improved handling of errors when message is not in the expected format [jansenbe - Bert Jansen]
- Correctly handle site url's containing special characters when building a SP REST batch request [jansenbe - Bert Jansen]
- Removed List.Url property as it does not exist on the SP.List object [wonderplayer - Rolands Strakis]
- Get hub site information by hub site id [pkbullock - Paul Bullock]
- Refreshed list of OOB web parts, now includes Spaces (mixed reality) web parts [jansenbe - Bert Jansen]
- Update sites documentation #397 [wonderplayer - Rolands Strakis]
- Added support for reading/saving Viva Topic pages [jansenbe - Bert Jansen]
- Implement paging in loadPagesAsync [jansenbe - Bert Jansen]
- Make loadPagesAsync work when querying for a specific page in a big list #383 [YannickRe - Yannick Reekmans]
- Implement the EnsurePageListItemAsync method via GetFileByServerRelativeUrlAsync instead of using a CAML query to load the item to prevent 5000 item limit issues [jansenbe - Bert Jansen]

## [1.1.0]

### Added

- Parameterless constructor for InteractiveAuthenticationProvider [jansenbe - Bert Jansen]
- Taxonomy support [jansenbe - Bert Jansen]
- Added HubSite methods [pkbullock - Paul Bullock]
- Added AddRoleDefinition and RemoveRoleDefinition methods on ListItem [jansenbe - Bert Jansen]
- Additional batch method for BreakRoleInheritance and ResetRoleInheritance [jansenbe - Bert Jansen]
- Added ClassifyAndExtract methods for a IList [jansenbe - Bert Jansen]
- Added AsBatchAsync overloads to allow specifying the batch to use [PaoloPia - Paolo Pialorsi]
- Added output for ClassifyAndExtractBatch methods [jansenbe - Bert Jansen]
- Added Batch methods for SyntexModel.GetModelPublications, SyntexModel.SyntexModel.PublishModel and SyntexModel.UnPublishModel [jansenbe - Bert Jansen]
- Support for processing result(s) when using one of the RawRequestBatch methods (see SyntexModel.GetModelPublicationsBatchAsync and SyntexModel.PublishModelBatchAsync as examples) [jansenbe - Bert Jansen]
- Support to request for classifying and extracting of a file via the connected Syntex models [jansenbe - Bert Jansen]
- Exposed additional Get/Load synchronous and asynchronous extension methods #354 [PaoloPia - Paolo Pialorsi]

### Changed

- List documentation updates #375 [dgtheninja - David Gent]
- Make Page Publish respect Version Settings and Checkout Status on publish #361 [czullu - Christian Zuellig]
- Improved handling of ListItem operations when list item was fetch via Folder or File (ListItemAllFields property) [jansenbe - Bert Jansen]
- CSOM batches are now split on size (when needed) [jansenbe - Bert Jansen]
- Added CSOM batching support to ensure a multiple CSOM requests are grouped into a single server call + integrated CSOM response handling [jansenbe - Bert Jansen]
- Added output for RecycleBatch methods on List, ListItem and File [jansenbe - Bert Jansen]
- Fix bug in EnsureFolder: running this method when the folder structure existed tried to create the folder again [jansenbe - Bert Jansen]
- Improved page name normalization (only # are not allowed in modern page names) #353 [Sarah4x - Sarah Wilson]
- Uplifted samples to GA v1.0.0 #350 [pkbullock - Paul Bullock]
- Bug fix for issue #351 - Teams Chat Messages not working [pkbullock - Paul Bullock]
- Improved handling of special characters ('#& ) for files and folders [jansenbe - Bert Jansen]
- Fix for single quote in client side page name #348 [Sarah4x - Sarah Wilson]
- Improved CSOM support for content type creation and taxonomy field creation #349 [mgwojciech - Marcin Wojciechowski]
- Fix for older SharePoint UI created image web parts where links is set to null #347 [Sarah4x - Sarah Wilson]

## [1.0.0]

### Added

- Added Rights support for custom actions [erwinvanhunen - Erwin van Hunen]
- Added support to recycle a list item to the recycle bin [erwinvanhunen - Erwin van Hunen]
- Added support to add a folder to a SharePoint list [erwinvanhunen - Erwin van Hunen]
- Added support to modify roledefinitions for a SharePoint list [erwinvanhunen - Erwin van Hunen]
- Added support to add and remove roledefinitions to a SharePoint group [erwinvanhunen - Erwin van Hunen]
- Added support for breaking role inheritance and resetting role inheritance for a list [erwinvanhunen - Erwin van Hunen]
- Added support for creating and deleting roledefinitions [erwinvanhunen - Erwin van Hunen]
- Added support for deleting SharePoint groups [erwinvanhunen - Erwin van Hunen]
- Added support for adding and removing users from ISharePointGroup [erwinvanhunen - Erwin van Hunen]
- Added support for setting the Compliance Tag on a list [erwinvanhunen - Erwin van Hunen]
- Added support for setting the Compliance Tag on a list item [erwinvanhunen - Erwin van Hunen]
- Added support for retrieving available site Compliance Tags / retention labels [erwinvanhunen - Erwin van Hunen]
- Added support for retrieving the Compliance Tag set for a list [erwinvanhunen - Erwin van Hunen]
- Implemented PnP.Core.Auth [PaoloPia - Paolo Pialorsi]
- Issue templates (#34) [ypcode - Yannick Plennevaux] / [jansenbe - Bert Jansen]
- Enable PnPContext creation from a Microsoft 365 Group id (#39) [jansenbe - Bert Jansen]
- Added the Group model, represents a Microsoft 365 group (#48) [jansenbe - Bert Jansen]
- Added custom exception handling for authentication errors (#45) [jansenbe - Bert Jansen]
- Support to use an externally obtained access token (#44) [jansenbe - Bert Jansen]
- Add step-by-step guide on how to extend the model (#46) [ypcode - Yannick Plennevaux] / [jansenbe - Bert Jansen]
- First iteration of content type support (within the limitations of SharePoint REST) (#47) [ypcode - Yannick Plennevaux]
- Update test code so that custom settings file do not have to be marked as "copy always" #53 [jansenbe - Bert Jansen]
- Use RegisterWaitForSingleObject for the token invalidation thread in combination with IDisposable to prevent threads to leak #56 [jansenbe - Bert Jansen]
- Mark read-only properties as Get only in the interfaces to ensure SDK consumers are not trying to update them #59 [jansenbe - Bert Jansen]
- Request retry mechanism, will handle core http and Graph batch requests #21 [jansenbe - Bert Jansen]
- Tweaking and updating of the writing tests documentation #65 [pkbullock - Paul Bullock]
- Paging support for Graph/Rest #3 [jansenbe - Bert Jansen]
- Added Clone method on PnPContext #49 [jansenbe - Bert Jansen]
- Extend the domain model for Web/Site Feature support #62 [pkbullock - Paul Bullock]
- Extend SP Field model (add operation for each field type) for both Web and List fields #71 [ypcode - Yannick Plennevaux]
- Added support for AddFieldAsXml #71 [ypcode - Yannick Plennevaux]
- Add basic support for FieldLinks #75 [ypcode - Yannick Plennevaux]
- Add support for external access token provider (IOAuthAccessTokenProvider) #76 [ypcode - Yannick Plennevaux]
- Added recursive support for .Include() to define which fields of a collection are loaded [jansenbe - Bert Jansen]
- Add support for getting list items via a CAML query #80 [jansenbe - Bert Jansen]
- Support RenderListDataAsStream method on lists - basic implementation #81 [jansenbe - Bert Jansen]
- Add support for calling client.svc (CSOM) endpoint #86 [jansenbe - Bert Jansen]
- Added SystemUpdate() and UpdateOverwriteVersion() methods on the ListItem model (uses CSOM) [jansenbe - Bert Jansen]
- Async LINQ support [PaoloPia - Paolo Pialorsi]
- Added sync equivalents for all async methods [jansenbe - Bert Jansen]
- Support for list and web folders #78 [ypcode - Yannick Plennevaux]
- Test cases for the Teams model #95 [JarbasHorst - Jarbas Horst]
- Taxonomy support using the Graph API #104 [jansenbe - Bert Jansen]
- Support working with files #78 [ypcode - Yannick Plennevaux]
- Use CSOM to create content types with a specified id #108 [ypcode - Yannick Plennevaux]
- Support for making interactive SharePoint REST requests + upload of files #112 [jansenbe - Bert Jansen]
- Additional test cases - working getting >80% test coverage [pkbullock - Paul Bullock]
- Expansion of the Web object with web properties, root folder, site user info list and more [ypcode - Yannick Plennevaux]
- Rewrite of the auth model, fixes #43 [PaoloPia - Paolo Pialorsi]
- LoadProperties now also supports Graph #145 [jansenbe - Bert Jansen]
- Authentication enhancements #302 [sebastianmattar - Sebastian Mattar]
- CSOM Light client - first step implementation for PropertyBag updates #334 [mgwojciech - Marcin Wojciechowski]
- Support for ListItem permission management #343 [jimmywim - Jim Love]
- CSOM Light client - List item update logic #345 [mgwojciech - Marcin Wojciechowski]

### Changed

- Documentation updates [JarbasHorst - Jarbas Horst]
- EnsurePropertiesAsync takes in account .Include (recursive) usage [jansenbe - Bert Jansen]
- Documentation updates [Ashikpaul - Ashik Paul]
- Moved to complete async internal implementation [jansenbe - Bert Jansen]
- Code cleanup (Teams implementation / ODataQuery) [JarbasHorst - Jarbas Horst]
- When a non loaded model properties is assigned it now is considered as a change and send back to the server on update #106 [jansenbe - Bert Jansen]
- Throw client exception when an API call still contains unresolved tokens before being executed #107 [jansenbe - Bert Jansen]