﻿using System.Threading.Tasks;

namespace PnP.Core.Model.Security
{
    /// <summary>
    /// Public interface to define a SharePoint User 
    /// </summary>
    [ConcreteType(typeof(SharePointUser))]
    public interface ISharePointUser : IDataModel<ISharePointUser>, IDataModelGet<ISharePointUser>, IDataModelLoad<ISharePointUser>, ISharePointPrincipal, IQueryableDataModel
    {
        /// <summary>
        /// Id of the underlying graph object (if any)
        /// </summary>
        public string AadObjectId { get; set; }

        /// <summary>
        /// User principle name (UPN) of the user
        /// </summary>
        public string UserPrincipalName { get; set; }

        /// <summary>
        /// E-mail address of the user.
        /// SP REST property name: Email
        /// </summary>
        public string Mail { get; set; }

        /// <summary>
        /// Department of the user
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// A date/time string for which the format conforms to the ISO8601 time format YYYY-MM-DDTHH:MM:SSZ and
        /// which represents the time and date of expiry for the user.
        /// A null value indicates no expiry.
        /// </summary>
        public string Expiration { get; }

        /// <summary>
        /// Indicates whether the user is a guest user authenticated via an e-mail address
        /// </summary>
        public bool IsEmailAuthenticationGuestUser { get; }

        /// <summary>
        /// Indicates whether the user is a guest user shared by e-mail
        /// </summary>
        public bool IsShareByEmailGuestUser { get; }

        /// <summary>
        /// Indicates whether the user is a site collection administrator
        /// </summary>
        public bool IsSiteAdmin { get; }

        /// <summary>
        /// Returns this SharePoint user as a Graph user
        /// </summary>
        /// <returns></returns>
        public Task<IGraphUser> AsGraphUserAsync();

        /// <summary>
        /// Returns this SharePoint user as a Graph user
        /// </summary>
        /// <returns></returns>
        public IGraphUser AsGraphUser();
    }
}
