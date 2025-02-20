﻿using System;
using System.Threading.Tasks;

namespace PnP.Core.Model.SharePoint
{
    /// <summary>
    /// Represents the set used in a term store. The set represents a unit which contains a collection of hierarchical terms. A group can contain multiple sets.
    /// </summary>
    [ConcreteType(typeof(TermSet))]
    public interface ITermSet : IDataModel<ITermSet>, IDataModelGet<ITermSet>, IDataModelLoad<ITermSet>, IDataModelUpdate, IDataModelDelete, IQueryableDataModel
    {
        /// <summary>
        /// The Unique ID of the term set.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Name of the term set for each language.
        /// </summary>
        public ITermSetLocalizedNameCollection LocalizedNames { get; }

        /// <summary>
        /// Description giving details on the term set.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Date and time of term set creation. Read-only.
        /// </summary>
        public DateTimeOffset CreatedDateTime { get; }

        /// <summary>
        /// Collection of term in this term set
        /// </summary>
        public ITermCollection Terms { get; }

        /// <summary>
        /// The parent group for this termset
        /// </summary>
        public ITermGroup Group { get; }

        /// <summary>
        /// Properties on this term set
        /// </summary>
        public ITermSetPropertyCollection Properties { get; }

        /// <summary>
        /// Collection of terms relations
        /// </summary>
        public ITermRelationCollection Relations { get; }

        /// <summary>
        /// Adds a property to the term set's property collection. Call update to persist this change.
        /// </summary>
        /// <param name="key">Property key</param>
        /// <param name="value">Property value</param>
        public Task AddPropertyAsync(string key, string value);

        /// <summary>
        /// Adds a property to the term set's property collection. Call update to persist this change.
        /// </summary>
        /// <param name="key">Property key</param>
        /// <param name="value">Property value</param>
        public void AddProperty(string key, string value);

    }
}