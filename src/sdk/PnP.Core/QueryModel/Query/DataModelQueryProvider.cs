﻿using PnP.Core.Model;
using PnP.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace PnP.Core.QueryModel
{
    /// <summary>
    /// Concrete implementation of an IQueryProvider for the Domain Model
    /// </summary>
    /// <typeparam name="TModel">The Type of the Domain Model object that the IQueryProvider supports</typeparam>
    internal class DataModelQueryProvider<TModel> : BaseQueryProvider
    {
        /// <summary>
        /// The internal Query Service used to execute the actual queries
        /// </summary>
        private readonly DataModelQueryService<TModel> queryService;

        private EntityInfo entityInfo;

        internal EntityInfo EntityInfo
        {
            get
            {
                return entityInfo;
            }
            set
            {
                entityInfo = value;
                queryService.EntityInfo = entityInfo;
            }
        }

        #region Constructors

        // Keep this constructor protected to avoid creation from outside
        protected DataModelQueryProvider() { }

        /// <summary>
        /// Creates a new instance of the IQueryProvider based on an external Query Service
        /// </summary>
        /// <param name="queryService"></param>
        public DataModelQueryProvider(DataModelQueryService<TModel> queryService)
        {
            this.queryService = queryService ?? throw new ArgumentNullException(nameof(queryService));
        }

        #endregion

        #region BaseQueryProvider abstract methods implementation

        public override Task<IEnumerableBatchResult<TResult>> AddToCurrentBatchAsync<TResult>(Expression expression)
        {
            return AddToBatchAsync<TResult>(expression, queryService.PnPContext.CurrentBatch);
        }

        public override async Task<IEnumerableBatchResult<TResult>> AddToBatchAsync<TResult>(Expression expression, Batch batch)
        {
            // Translate the query expression into an actual query text for the target Query Service
            var query = Translate(expression);

            // Execute the query via the target Query Service
            BatchRequest batchRequest= await queryService.AddToBatchAsync(query, batch).ConfigureAwait(false);

            // Get the resulting property from the parent object
            var collection = batchRequest.Model.GetPublicInstancePropertyValue(queryService.MemberName) as IRequestableCollection;
            var resultValue = (IReadOnlyList<TResult>)collection.RequestedItems;

            return new BatchEnumerableBatchResult<TResult>(batch, batchRequest.Id, resultValue);
        }

        public override Task<object> ExecuteObjectAsync(Expression expression, CancellationToken token)
        {
            // Translate the query expression into an actual query text for the target Query Service
            var query = Translate(expression);

            // Execute the query via the target Query Service
            return queryService.ExecuteQueryAsync(expression.Type, query, token);
        }

        public override IQueryable CreateQuery(Expression expression)
        {
            var result = new QueryableDataModelCollection<TModel>(this, expression);
            return result;
        }

        #endregion

        /// <summary>
        /// Internal method to execute the translation of the query expression 
        /// into an actual query text for the target Query Service
        /// </summary>
        /// <param name="expression">The expression to translate</param>
        /// <returns>The expression translated into the actual query text for the target Query Service</returns>
        internal static ODataQuery<TModel> Translate(Expression expression)
        {
            return new DataModelQueryTranslator<TModel>().Translate(expression);
        }
    }

}
