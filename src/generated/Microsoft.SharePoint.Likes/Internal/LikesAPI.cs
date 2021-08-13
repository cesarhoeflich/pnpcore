using System;
using PnP.Core.Services;

namespace PnP.Core.Model.SharePoint
{
    /// <summary>
    /// LikesAPI class, write your custom code here
    /// </summary>
    [SharePointType("Microsoft.SharePoint.Likes.LikesAPI", Uri = "_api/xxx", LinqGet = "_api/xxx")]
    internal partial class LikesAPI : BaseDataModel<ILikesAPI>, ILikesAPI
    {
        #region Construction
        public LikesAPI()
        {
        }
        #endregion

        #region Properties
        #region New properties

        public string Id4a81de82eeb94d6080ea5bf63e27023a { get => GetValue<string>(); set => SetValue(value); }

        #endregion

        #endregion

        #region Extension methods
        #endregion
    }
}
