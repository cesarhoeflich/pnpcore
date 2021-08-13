using PnP.Core.Model.Security;
using PnP.Core.Services;
using System;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PnP.Core.Model.SharePoint
{
    /// <summary>
    /// File class, write your custom code here
    /// </summary>
    [SharePointType("SP.File", Target = typeof(Folder), Uri = "_api/Web/getFileById('{Id}')", Get = "_api/Web/getFolderById('{Parent.Id}')/Files", LinqGet = "_api/Web/getFolderById('{Parent.Id}')/Files")]
    [SharePointType("SP.File", Target = typeof(Web), Uri = "_api/Web/getFileById('{Id}')")]
    [SharePointType("SP.File", Target = typeof(ListItem), Uri = "_api/Web/lists(guid'{List.Id}')/items({Parent.Id})/file")]
    internal partial class File : BaseDataModel<IFile>, IFile
    {
        internal const string AddFileContentAdditionalInformationKey = "Content";
        internal const string AddFileOverwriteAdditionalInformationKey = "Overwrite";

        #region Construction
        public File()
        {
        }
        #endregion

        #region Properties
        public string CheckInComment { get => GetValue<string>(); set => SetValue(value); }

        public CheckOutType CheckOutType { get => GetValue<CheckOutType>(); set => SetValue(value); }

        public string ContentTag { get => GetValue<string>(); set => SetValue(value); }

        public CustomizedPageStatus CustomizedPageStatus { get => GetValue<CustomizedPageStatus>(); set => SetValue(value); }

        public Guid ListId { get => GetValue<Guid>(); set => SetValue(value); }

        public string ETag { get => GetValue<string>(); set => SetValue(value); }

        public bool HasAlternateContentStreams { get => GetValue<bool>(); set => SetValue(value); }

        public bool IrmEnabled { get => GetValue<bool>(); set => SetValue(value); }

        public long Length { get => GetValue<long>(); set => SetValue(value); }

        public string LinkingUri { get => GetValue<string>(); set => SetValue(value); }

        public string LinkingUrl { get => GetValue<string>(); set => SetValue(value); }

        public int MajorVersion { get => GetValue<int>(); set => SetValue(value); }

        public int MinorVersion { get => GetValue<int>(); set => SetValue(value); }

        public string Name { get => GetValue<string>(); set => SetValue(value); }

        public ListPageRenderType PageRenderType { get => GetValue<ListPageRenderType>(); set => SetValue(value); }

        public PublishedStatus Level { get => GetValue<PublishedStatus>(); set => SetValue(value); }

        public string ServerRedirectedUrl { get => GetValue<string>(); set => SetValue(value); }

        public string ServerRelativeUrl { get => GetValue<string>(); set => SetValue(value); }

        public Guid SiteId { get => GetValue<Guid>(); set => SetValue(value); }

        public DateTime TimeCreated { get => GetValue<DateTime>(); set => SetValue(value); }

        public DateTime TimeLastModified { get => GetValue<DateTime>(); set => SetValue(value); }

        public string Title { get => GetValue<string>(); set => SetValue(value); }

        public int UIVersion { get => GetValue<int>(); set => SetValue(value); }

        public string UIVersionLabel { get => GetValue<string>(); set => SetValue(value); }

        public Guid UniqueId { get => GetValue<Guid>(); set => SetValue(value); }

        public string VroomDriveID { get => GetValue<string>(); set => SetValue(value); }

        public string VroomItemID { get => GetValue<string>(); set => SetValue(value); }

        public Guid WebId { get => GetValue<Guid>(); set => SetValue(value); }

        public IListItem ListItemAllFields { get => GetModelValue<IListItem>(); }

        public IEffectiveInformationRightsManagementSettings EffectiveInformationRightsManagementSettings { get => GetModelValue<IEffectiveInformationRightsManagementSettings>(); }

        public IInformationRightsManagementFileSettings InformationRightsManagementSettings { get => GetModelValue<IInformationRightsManagementFileSettings>(); }

        public IPropertyValues Properties { get => GetModelValue<IPropertyValues>(); }

        public IFileVersionEventCollection VersionEvents { get => GetModelCollectionValue<IFileVersionEventCollection>(); }

        public IFileVersionCollection Versions { get => GetModelCollectionValue<IFileVersionCollection>(); }

        public ISharePointUser Author { get => GetModelValue<ISharePointUser>(); }

        public ISharePointUser CheckedOutByUser { get => GetModelValue<ISharePointUser>(); }

        public ISharePointUser LockedByUser { get => GetModelValue<ISharePointUser>(); }

        public ISharePointUser ModifiedBy { get => GetModelValue<ISharePointUser>(); }

        [KeyProperty(nameof(UniqueId))]
        public override object Key { get => UniqueId; set => UniqueId = Guid.Parse(value.ToString()); }
        #endregion

        #region Extension methods

        #region GetContent
        public async Task<Stream> GetContentAsync(bool streamContent = false)
        {
            string downloadUrl = $"{PnPContext.Uri}/_layouts/15/download.aspx?UniqueId={UniqueId}";

            var apiCall = new ApiCall(downloadUrl, ApiType.SPORest)
            {
                Interactive = true,
                ExpectBinaryResponse = true,
                StreamResponse = streamContent
            };

            var response = await RawRequestAsync(apiCall, HttpMethod.Get).ConfigureAwait(false);
            return response.BinaryContent;
        }

        public Stream GetContent(bool streamContent = false)
        {
            return GetContentAsync(streamContent).GetAwaiter().GetResult();
        }


        public async Task<byte[]> GetContentBytesAsync()
        {
            using (var contentStream = await GetContentAsync().ConfigureAwait(false))
            {
                return ((MemoryStream)contentStream).ToArray();
            }
        }

        public byte[] GetContentBytes()
        {
            return GetContentBytesAsync().GetAwaiter().GetResult();
        }
        #endregion

        #region Publish
        public async Task PublishAsync(string comment = null)
        {
            var entity = EntityManager.GetClassInfo(GetType(), this);
            string publishEndpointUrl = $"{entity.SharePointUri}/publish(comment='{comment ?? string.Empty}')";

            var apiCall = new ApiCall(publishEndpointUrl, ApiType.SPORest);

            await RawRequestAsync(apiCall, HttpMethod.Post).ConfigureAwait(false);
        }

        public void Publish(string comment = null)
        {
            PublishAsync(comment).GetAwaiter().GetResult();
        }

        public async Task PublishBatchAsync(string comment = null)
        {
            await PublishBatchAsync(PnPContext.CurrentBatch, comment).ConfigureAwait(false);
        }

        public void PublishBatch(string comment = null)
        {
            PublishBatchAsync(comment).GetAwaiter().GetResult();
        }

        public async Task PublishBatchAsync(Batch batch, string comment = null)
        {
            var entity = EntityManager.GetClassInfo(GetType(), this);
            string publishEndpointUrl = $"{entity.SharePointUri}/publish(comment='{comment ?? string.Empty}')";

            var apiCall = new ApiCall(publishEndpointUrl, ApiType.SPORest);

            await RawRequestBatchAsync(batch, apiCall, HttpMethod.Post).ConfigureAwait(false);
        }

        public void PublishBatch(Batch batch, string comment = null)
        {
            PublishBatchAsync(batch, comment).GetAwaiter().GetResult();
        }
        #endregion

        #region Unpublish
        public void Unpublish(string comment = null)
        {
            UnpublishAsync(comment).GetAwaiter().GetResult();
        }

        public async Task UnpublishAsync(string comment = null)
        {
            var entity = EntityManager.GetClassInfo(GetType(), this);
            string publishEndpointUrl = $"{entity.SharePointUri}/unpublish(comment='{comment ?? string.Empty}')";

            var apiCall = new ApiCall(publishEndpointUrl, ApiType.SPORest);

            await RawRequestAsync(apiCall, HttpMethod.Post).ConfigureAwait(false);
        }

        public void UnpublishBatch(Batch batch, string comment = null)
        {
            UnpublishBatchAsync(batch, comment).GetAwaiter().GetResult();
        }

        public async Task UnpublishBatchAsync(string comment = null)
        {
            await UnpublishBatchAsync(PnPContext.CurrentBatch, comment).ConfigureAwait(false);
        }

        public async Task UnpublishBatchAsync(Batch batch, string comment = null)
        {
            var entity = EntityManager.GetClassInfo(GetType(), this);
            string publishEndpointUrl = $"{entity.SharePointUri}/unpublish(comment='{comment ?? string.Empty}')";

            var apiCall = new ApiCall(publishEndpointUrl, ApiType.SPORest);

            await RawRequestBatchAsync(batch, apiCall, HttpMethod.Post).ConfigureAwait(false);
        }

        public void UnpublishBatch(string comment = null)
        {
            UnpublishBatchAsync(comment).GetAwaiter().GetResult();
        }
        #endregion

        #region Checkout
        public async Task CheckoutAsync()
        {
            var entity = EntityManager.GetClassInfo(GetType(), this);
            string checkoutEndpointUrl = $"{entity.SharePointUri}/checkout";

            var apiCall = new ApiCall(checkoutEndpointUrl, ApiType.SPORest);

            await RawRequestAsync(apiCall, HttpMethod.Post).ConfigureAwait(false);
        }

        public void Checkout()
        {
            CheckoutAsync().GetAwaiter().GetResult();
        }

        public async Task CheckoutBatchAsync()
        {
            await CheckoutBatchAsync(PnPContext.CurrentBatch).ConfigureAwait(false);
        }

        public void CheckoutBatch()
        {
            CheckoutBatchAsync().GetAwaiter().GetResult();
        }

        public async Task CheckoutBatchAsync(Batch batch)
        {
            var entity = EntityManager.GetClassInfo(GetType(), this);
            string checkoutEndpointUrl = $"{entity.SharePointUri}/checkout";

            var apiCall = new ApiCall(checkoutEndpointUrl, ApiType.SPORest);

            await RawRequestBatchAsync(batch, apiCall, HttpMethod.Post).ConfigureAwait(false);
        }

        public void CheckoutBatch(Batch batch)
        {
            CheckoutBatchAsync(batch).GetAwaiter().GetResult();
        }
        #endregion

        #region UndoCheckout
        public void UndoCheckout()
        {
            UndoCheckoutAsync().GetAwaiter().GetResult();
        }

        public async Task UndoCheckoutAsync()
        {
            var entity = EntityManager.GetClassInfo(GetType(), this);
            string undoCheckoutEndpointUrl = $"{entity.SharePointUri}/undoCheckout";

            var apiCall = new ApiCall(undoCheckoutEndpointUrl, ApiType.SPORest);

            await RawRequestAsync(apiCall, HttpMethod.Post).ConfigureAwait(false);
        }

        public void UndoCheckoutBatch()
        {
            UndoCheckoutBatchAsync().GetAwaiter().GetResult();
        }

        public void UndoCheckoutBatch(Batch batch)
        {
            UndoCheckoutBatchAsync(batch).GetAwaiter().GetResult();
        }

        public async Task UndoCheckoutBatchAsync()
        {
            await UndoCheckoutBatchAsync(PnPContext.CurrentBatch).ConfigureAwait(false);
        }

        public async Task UndoCheckoutBatchAsync(Batch batch)
        {
            var entity = EntityManager.GetClassInfo(GetType(), this);
            string undoCheckoutEndpointUrl = $"{entity.SharePointUri}/undoCheckout";

            var apiCall = new ApiCall(undoCheckoutEndpointUrl, ApiType.SPORest);

            await RawRequestBatchAsync(batch, apiCall, HttpMethod.Post).ConfigureAwait(false);
        }
        #endregion

        #region Checkin

        public async Task CheckinAsync(string comment = null, CheckinType checkinType = CheckinType.MinorCheckIn)
        {
            var entity = EntityManager.GetClassInfo(GetType(), this);
            string checkinEndpointUrl = $"{entity.SharePointUri}/checkin(comment='{comment ?? string.Empty}',checkintype={(int)checkinType})";

            var apiCall = new ApiCall(checkinEndpointUrl, ApiType.SPORest);

            await RawRequestAsync(apiCall, HttpMethod.Post).ConfigureAwait(false);
        }

        public void Checkin(string comment = null, CheckinType checkinType = CheckinType.MinorCheckIn)
        {
            CheckinAsync(comment, checkinType).GetAwaiter().GetResult();
        }

        public void CheckinBatch(string comment = null, CheckinType checkinType = CheckinType.MinorCheckIn)
        {
            CheckinBatchAsync(comment, checkinType).GetAwaiter().GetResult();
        }

        public void CheckinBatch(Batch batch, string comment = null, CheckinType checkinType = CheckinType.MinorCheckIn)
        {
            CheckinBatchAsync(batch, comment, checkinType).GetAwaiter().GetResult();
        }

        public async Task CheckinBatchAsync(string comment = null, CheckinType checkinType = CheckinType.MinorCheckIn)
        {
            await CheckinBatchAsync(PnPContext.CurrentBatch, comment, checkinType).ConfigureAwait(false);
        }

        public async Task CheckinBatchAsync(Batch batch, string comment = null, CheckinType checkinType = CheckinType.MinorCheckIn)
        {
            var entity = EntityManager.GetClassInfo(GetType(), this);
            string checkinEndpointUrl = $"{entity.SharePointUri}/checkin(comment='{comment ?? string.Empty}',checkintype={(int)checkinType})";

            var apiCall = new ApiCall(checkinEndpointUrl, ApiType.SPORest);

            await RawRequestBatchAsync(batch, apiCall, HttpMethod.Post).ConfigureAwait(false);
        }
        #endregion

        #region Approve

        public async Task ApproveAsync(string comment = null)
        {
            var entity = EntityManager.GetClassInfo(GetType(), this);
            string checkinEndpointUrl = $"{entity.SharePointUri}/approve(comment='{comment ?? string.Empty}')";

            var apiCall = new ApiCall(checkinEndpointUrl, ApiType.SPORest);

            await RawRequestAsync(apiCall, HttpMethod.Post).ConfigureAwait(false);
        }

        public void Approve(string comment = null)
        {
            CheckinAsync(comment).GetAwaiter().GetResult();
        }

        public void ApproveBatch(string comment = null)
        {
            ApproveBatchAsync(comment).GetAwaiter().GetResult();
        }

        public void ApproveBatch(Batch batch, string comment = null)
        {
            ApproveBatchAsync(batch, comment).GetAwaiter().GetResult();
        }

        public async Task ApproveBatchAsync(string comment = null)
        {
            await ApproveBatchAsync(PnPContext.CurrentBatch, comment).ConfigureAwait(false);
        }

        public async Task ApproveBatchAsync(Batch batch, string comment = null)
        {
            var entity = EntityManager.GetClassInfo(GetType(), this);
            string approveEndpointUrl = $"{entity.SharePointUri}/approve(comment='{comment ?? string.Empty}')";

            var apiCall = new ApiCall(approveEndpointUrl, ApiType.SPORest);

            await RawRequestBatchAsync(batch, apiCall, HttpMethod.Post).ConfigureAwait(false);
        }

        #endregion

        #region Recycle

        public Guid Recycle()
        {
            return RecycleAsync().GetAwaiter().GetResult();
        }

        public async Task<Guid> RecycleAsync()
        {
            var entity = EntityManager.GetClassInfo(GetType(), this);
            string recycleEndpointUrl = $"{entity.SharePointUri}/recycle";

            var apiCall = new ApiCall(recycleEndpointUrl, ApiType.SPORest)
            {
                RemoveFromModel = true
            };

            var response = await RawRequestAsync(apiCall, HttpMethod.Post).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(response.Json))
            {
                return ProcessRecycleResponse(response.Json);
            }

            return Guid.Empty;
        }

        private static Guid ProcessRecycleResponse(string json)
        {
            var document = JsonSerializer.Deserialize<JsonElement>(json);
            if (document.TryGetProperty("d", out JsonElement root))
            {
                if (root.TryGetProperty("Recycle", out JsonElement recycleBinItemId))
                {
                    // return the recyclebin item id
                    return recycleBinItemId.GetGuid();
                }
            }

            return Guid.Empty;
        }

        public IBatchSingleResult<BatchResultValue<Guid>> RecycleBatch()
        {
            return RecycleBatchAsync().GetAwaiter().GetResult();
        }

        public IBatchSingleResult<BatchResultValue<Guid>> RecycleBatch(Batch batch)
        {
            return RecycleBatchAsync(batch).GetAwaiter().GetResult();
        }

        public async Task<IBatchSingleResult<BatchResultValue<Guid>>> RecycleBatchAsync()
        {
            return await RecycleBatchAsync(PnPContext.CurrentBatch).ConfigureAwait(false);
        }

        public async Task<IBatchSingleResult<BatchResultValue<Guid>>> RecycleBatchAsync(Batch batch)
        {
            var entity = EntityManager.GetClassInfo(GetType(), this);
            string recycleEndpointUrl = $"{entity.SharePointUri}/recycle";

            var apiCall = new ApiCall(recycleEndpointUrl, ApiType.SPORest)
            {
                RemoveFromModel = true,
                RawSingleResult = new BatchResultValue<Guid>(Guid.Empty),
                RawResultsHandler = (json, apiCall) =>
                {
                    (apiCall.RawSingleResult as BatchResultValue<Guid>).Value = ProcessRecycleResponse(json);
                }
            };

            var batchRequest = await RawRequestBatchAsync(batch, apiCall, HttpMethod.Post).ConfigureAwait(false);
            return new BatchSingleResult<BatchResultValue<Guid>>(batch, batchRequest.Id, apiCall.RawSingleResult as BatchResultValue<Guid>);
        }
        #endregion

        #region CopyTo
        private ApiCall GetCopyToSameSiteApiCall(string destinationUrl, bool overwrite)
        {
            var entityInfo = EntityManager.GetClassInfo(GetType(), this);
            // NOTE WebUtility encode spaces to "+" instead of %20
            string encodedDestinationUrl = WebUtility.UrlEncode(destinationUrl).Replace("+", "%20").Replace("/", "%2F");
            string copyToEndpointUrl = $"{entityInfo.SharePointUri}/copyTo(strnewurl='{encodedDestinationUrl}', boverwrite={overwrite.ToString().ToLower()})";

            return new ApiCall(copyToEndpointUrl, ApiType.SPORest);
        }

        private ApiCall GetCopyToCrossSiteApiCall(string destinationUrl, bool overwrite, MoveCopyOptions options)
        {
            string destUrl = UrlUtility.EnsureAbsoluteUrl(PnPContext.Uri, destinationUrl).ToString();
            string srcUrl = UrlUtility.EnsureAbsoluteUrl(PnPContext.Uri, ServerRelativeUrl).ToString();

            // Default is "overwrite=false" => "KeepBoth=true"
            options ??= new MoveCopyOptions() { KeepBoth = true };

            // If options are specified, use the CopyByPath API endpoint
            var parameters = new
            {
                destPath = new
                {
                    __metadata = new { type = "SP.ResourcePath" },
                    DecodedUrl = destUrl
                },
                srcPath = new
                {
                    __metadata = new { type = "SP.ResourcePath" },
                    DecodedUrl = srcUrl
                },
                options = new
                {
                    __metadata = new { type = "SP.MoveCopyOptions" },
                    options.KeepBoth,
                    options.ResetAuthorAndCreatedOnCopy,
                    options.ShouldBypassSharedLocks
                }
            }.AsExpando();
            string body = JsonSerializer.Serialize(parameters, typeof(ExpandoObject));
            string copyToEndpointUrl = $"_api/SP.MoveCopyUtil.CopyFileByPath(overwrite=@a1)?@a1={overwrite.ToString().ToLower()}";

            return new ApiCall(copyToEndpointUrl, ApiType.SPORest, body);
        }

        private ApiCall GetCopyToApiCall(string destinationUrl, bool overwrite, MoveCopyOptions options)
        {
            // If same site
            if (UrlUtility.IsSameSite(PnPContext.Uri, destinationUrl))
            {
                return GetCopyToSameSiteApiCall(destinationUrl, overwrite);
            }
            else
            {
                options ??= new MoveCopyOptions() { KeepBoth = !overwrite };
                return GetCopyToCrossSiteApiCall(destinationUrl, overwrite, options);
            }
        }

        public async Task CopyToAsync(string destinationUrl, bool overwrite = false, MoveCopyOptions options = null)
        {
            var apiCall = GetCopyToApiCall(destinationUrl, overwrite, options);
            await RawRequestAsync(apiCall, HttpMethod.Post).ConfigureAwait(false);
        }

        public void CopyTo(string destinationUrl, bool overwrite = false, MoveCopyOptions options = null)
        {
            CopyToAsync(destinationUrl, overwrite).GetAwaiter().GetResult();
        }

        public async Task CopyToBatchAsync(Batch batch, string destinationUrl, bool overwrite = false, MoveCopyOptions options = null)
        {
            var apiCall = GetCopyToApiCall(destinationUrl, overwrite, options);
            await RawRequestBatchAsync(batch, apiCall, HttpMethod.Post).ConfigureAwait(false);
        }

        public void CopyToBatch(Batch batch, string destinationUrl, bool overwrite = false, MoveCopyOptions options = null)
        {
            CopyToBatchAsync(batch, destinationUrl, overwrite, options).GetAwaiter().GetResult();
        }

        public async Task CopyToBatchAsync(string destinationUrl, bool overwrite = false, MoveCopyOptions options = null)
        {
            await CopyToBatchAsync(PnPContext.CurrentBatch, destinationUrl, overwrite, options).ConfigureAwait(false);
        }

        public void CopyToBatch(string destinationUrl, bool overwrite = false, MoveCopyOptions options = null)
        {
            CopyToBatchAsync(destinationUrl, overwrite, options).GetAwaiter().GetResult();
        }
        #endregion

        #region MoveTo
        private ApiCall GetMoveToSameSiteApiCall(string destinationUrl, MoveOperations moveOperations)
        {
            var entityInfo = EntityManager.GetClassInfo(GetType(), this);
            // NOTE WebUtility encode spaces to "+" instead of %20
            string encodedDestinationUrl = WebUtility.UrlEncode(destinationUrl).Replace("+", "%20").Replace("/", "%2F");
            string moveToEndpointUrl = $"{entityInfo.SharePointUri}/moveTo(newurl='{encodedDestinationUrl}', flags={(int)moveOperations})";

            return new ApiCall(moveToEndpointUrl, ApiType.SPORest);
        }

        private ApiCall GetMoveToCrossSiteApiCall(string destinationUrl, bool overwrite, MoveCopyOptions options)
        {
            string destUrl = UrlUtility.EnsureAbsoluteUrl(PnPContext.Uri, destinationUrl).ToString();
            string srcUrl = UrlUtility.EnsureAbsoluteUrl(PnPContext.Uri, ServerRelativeUrl).ToString();

            // Default is "overwrite=false" => "KeepBoth=true"
            options ??= new MoveCopyOptions() { KeepBoth = true };

            // If options are specified, use the CopyByPath API endpoint
            var parameters = new
            {
                destPath = new
                {
                    __metadata = new { type = "SP.ResourcePath" },
                    DecodedUrl = destUrl
                },
                srcPath = new
                {
                    __metadata = new { type = "SP.ResourcePath" },
                    DecodedUrl = srcUrl
                },
                options = new
                {
                    __metadata = new { type = "SP.MoveCopyOptions" },
                    options.KeepBoth,
                    options.ResetAuthorAndCreatedOnCopy,
                    options.ShouldBypassSharedLocks
                }
            }.AsExpando();
            string body = JsonSerializer.Serialize(parameters, typeof(ExpandoObject));
            string moveToEndpointUrl = $"_api/SP.MoveCopyUtil.MoveFileByPath(overwrite=@a1)?@a1={overwrite.ToString().ToLower()}";

            return new ApiCall(moveToEndpointUrl, ApiType.SPORest, body);
        }

        private ApiCall GetMoveToApiCall(string destinationUrl, MoveOperations moveOperations, MoveCopyOptions options)
        {
            // If same site
            if (UrlUtility.IsSameSite(PnPContext.Uri, destinationUrl))
            {
                return GetMoveToSameSiteApiCall(destinationUrl, moveOperations);
            }
            else
            {
                bool overwrite = moveOperations.HasFlag(MoveOperations.Overwrite);
                options ??= new MoveCopyOptions() { KeepBoth = !overwrite };
                return GetMoveToCrossSiteApiCall(destinationUrl, overwrite, options);
            }
        }

        public async Task MoveToAsync(string destinationUrl, MoveOperations moveOperations = MoveOperations.None, MoveCopyOptions options = null)
        {
            var apiCall = GetMoveToApiCall(destinationUrl, moveOperations, options);
            await RawRequestAsync(apiCall, HttpMethod.Post).ConfigureAwait(false);
        }

        public void MoveTo(string destinationUrl, MoveOperations moveOperations = MoveOperations.None, MoveCopyOptions options = null)
        {
            MoveToAsync(destinationUrl, moveOperations, options).GetAwaiter().GetResult();
        }

        public async Task MoveToBatchAsync(Batch batch, string destinationUrl, MoveOperations moveOperations = MoveOperations.None, MoveCopyOptions options = null)
        {
            var apiCall = GetMoveToApiCall(destinationUrl, moveOperations, options);
            await RawRequestBatchAsync(batch, apiCall, HttpMethod.Post).ConfigureAwait(false);
        }

        public void MoveToBatch(Batch batch, string destinationUrl, MoveOperations moveOperations = MoveOperations.None, MoveCopyOptions options = null)
        {
            MoveToBatchAsync(batch, destinationUrl, moveOperations, options).GetAwaiter().GetResult();
        }

        public async Task MoveToBatchAsync(string destinationUrl, MoveOperations moveOperations = MoveOperations.None, MoveCopyOptions options = null)
        {
            await MoveToBatchAsync(PnPContext.CurrentBatch, destinationUrl, moveOperations, options).ConfigureAwait(false);
        }

        public void MoveToBatch(string destinationUrl, MoveOperations moveOperations = MoveOperations.None, MoveCopyOptions options = null)
        {
            MoveToBatchAsync(destinationUrl, moveOperations, options).GetAwaiter().GetResult();
        }
        #endregion

        #region Syntex
        public async Task<ISyntexClassifyAndExtractResult> ClassifyAndExtractAsync()
        {
            ApiCall apiCall = CreateClassifyAndExtractApiCall();
            var apiResult = await RawRequestAsync(apiCall, HttpMethod.Post).ConfigureAwait(false);

            // Process response
            return ProcessClassifyAndExtractResponse(apiResult.Json);
        }

        public ISyntexClassifyAndExtractResult ClassifyAndExtract()
        {
            return ClassifyAndExtractAsync().GetAwaiter().GetResult();
        }

        public async Task<IBatchSingleResult<ISyntexClassifyAndExtractResult>> ClassifyAndExtractBatchAsync(Batch batch)
        {
            ApiCall apiCall = CreateClassifyAndExtractApiCall();
            apiCall.RawSingleResult = new SyntexClassifyAndExtractResult();
            apiCall.RawResultsHandler = (json, apiCall) =>
            {
                var result = ProcessClassifyAndExtractResponse(json);
                (apiCall.RawSingleResult as SyntexClassifyAndExtractResult).Created = result.Created;
                (apiCall.RawSingleResult as SyntexClassifyAndExtractResult).DeliverDate = result.DeliverDate;
                (apiCall.RawSingleResult as SyntexClassifyAndExtractResult).ErrorMessage = result.ErrorMessage;
                (apiCall.RawSingleResult as SyntexClassifyAndExtractResult).Id = result.Id;
                (apiCall.RawSingleResult as SyntexClassifyAndExtractResult).Status = result.Status;
                (apiCall.RawSingleResult as SyntexClassifyAndExtractResult).StatusCode = result.StatusCode;
                (apiCall.RawSingleResult as SyntexClassifyAndExtractResult).TargetServerRelativeUrl = result.TargetServerRelativeUrl;
                (apiCall.RawSingleResult as SyntexClassifyAndExtractResult).TargetSiteUrl = result.TargetSiteUrl;
                (apiCall.RawSingleResult as SyntexClassifyAndExtractResult).TargetWebServerRelativeUrl = result.TargetWebServerRelativeUrl;
                (apiCall.RawSingleResult as SyntexClassifyAndExtractResult).WorkItemType = result.WorkItemType;
            };

            var batchRequest = await RawRequestBatchAsync(batch, apiCall, HttpMethod.Post).ConfigureAwait(false);

            return new BatchSingleResult<ISyntexClassifyAndExtractResult>(batch, batchRequest.Id, apiCall.RawSingleResult as ISyntexClassifyAndExtractResult);
        }

        public IBatchSingleResult<ISyntexClassifyAndExtractResult> ClassifyAndExtractBatch(Batch batch)
        {
            return ClassifyAndExtractBatchAsync(batch).GetAwaiter().GetResult();
        }

        public async Task<IBatchSingleResult<ISyntexClassifyAndExtractResult>> ClassifyAndExtractBatchAsync()
        {
            return await ClassifyAndExtractBatchAsync(PnPContext.CurrentBatch).ConfigureAwait(false);
        }

        public IBatchSingleResult<ISyntexClassifyAndExtractResult> ClassifyAndExtractBatch()
        {
            return ClassifyAndExtractBatchAsync().GetAwaiter().GetResult();
        }

        private static ISyntexClassifyAndExtractResult ProcessClassifyAndExtractResponse(string json)
        {
            var root = JsonDocument.Parse(json).RootElement.GetProperty("d");
            return new SyntexClassifyAndExtractResult
            {
                Created = root.GetProperty("Created").GetDateTime(),
                DeliverDate = root.GetProperty("DeliverDate").GetDateTime(),
                ErrorMessage = root.GetProperty("ErrorMessage").GetString(),
                StatusCode = root.GetProperty("StatusCode").GetInt32(),
                Id = root.GetProperty("ID").GetGuid(),
                Status = root.GetProperty("Status").GetString(),
                WorkItemType = root.GetProperty("Type").GetGuid(),
                TargetServerRelativeUrl = root.GetProperty("TargetServerRelativeUrl").GetString(),
                TargetSiteUrl = root.GetProperty("TargetSiteUrl").GetString(),
                TargetWebServerRelativeUrl = root.GetProperty("TargetWebServerRelativeUrl").GetString()
            };
        }

        private ApiCall CreateClassifyAndExtractApiCall()
        {
            var classifyAndExtractFile = new
            {
                __metadata = new { type = "Microsoft.Office.Server.ContentCenter.SPMachineLearningWorkItemEntityData" },
                Type = "AE9D4F24-EE84-4C0C-A972-A74CFFE939A1",
                TargetSiteId = PnPContext.Site.Id,
                TargetWebId = PnPContext.Web.Id,
                TargetUniqueId = UniqueId
            }.AsExpando();

            string body = JsonSerializer.Serialize(classifyAndExtractFile, new JsonSerializerOptions()
            {
                IgnoreNullValues = true
            });

            var apiCall = new ApiCall("_api/machinelearning/workitems", ApiType.SPORest, body);
            return apiCall;
        }
        #endregion

        #endregion

        #region Helper methods
        internal static bool ErrorIndicatesFileDoesNotExists(SharePointRestError error)
        {
            // Indicates the file did not exist
            if (error.HttpResponseCode == 404 && error.ServerErrorCode == -2130575338)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}
