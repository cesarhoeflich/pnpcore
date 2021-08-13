using System;

namespace PnP.Core.Model.SharePoint
{
    /// <summary>
    /// Public interface to define a LikesAPI object
    /// </summary>
    [ConcreteType(typeof(LikesAPI))]
    public interface ILikesAPI : IDataModel<ILikesAPI>, IDataModelGet<ILikesAPI>, IDataModelUpdate, IDataModelDelete
    {

        #region New properties

        /// <summary>
        /// To update...
        /// </summary>
        public string Id4a81de82eeb94d6080ea5bf63e27023a { get; set; }

        #endregion

    }
}
