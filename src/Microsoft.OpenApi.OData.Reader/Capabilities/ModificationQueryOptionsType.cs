// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OpenApi.OData.Common;
using Microsoft.OpenApi.OData.Edm;

namespace Microsoft.OpenApi.OData.Capabilities
{
    /// <summary>
    /// Complex type: Org.OData.Capabilities.V1.ModificationQueryOptionsType
    /// </summary>
    internal class ModificationQueryOptionsType
    {
        /// <summary>
        /// Gets/sets the $expand with modification requests.
        /// </summary>
        public bool? ExpandSupported { get; private set; }

        /// <summary>
        /// Gets/sets the $select with modification requests.
        /// </summary>
        public bool? SelectSupported { get; private set; }

        /// <summary>
        /// Gets/sets the $compute with modification requests.
        /// </summary>
        public bool? ComputeSupported { get; private set; }

        /// <summary>
        /// Gets/sets the $filter with modification requests.
        /// </summary>
        public bool? FilterSupported { get; private set; }

        /// <summary>
        /// Gets/sets the $search with modification requests.
        /// </summary>
        public bool? SearchSupported { get; private set; }

        /// <summary>
        /// Gets/sets the $sort with modification requests.
        /// </summary>
        public bool? SortSupported { get; private set; }

        /// <summary>
        /// Init the <see cref="PermissionType"/>.
        /// </summary>
        /// <param name="record">The input record.</param>
        public void Init(IEdmRecordExpression record)
        {
            Utils.CheckArgumentNull(record, nameof(record));

            // ExpandSupported
            ExpandSupported = record.GetBoolean("ExpandSupported");

            // SelectSupported
            SelectSupported = record.GetBoolean("SelectSupported");

            // ComputeSupported
            ComputeSupported = record.GetBoolean("ComputeSupported");

            // FilterSupported
            FilterSupported = record.GetBoolean("FilterSupported");

            // SearchSupported
            SearchSupported = record.GetBoolean("SearchSupported");

            // SortSupported
            SortSupported = record.GetBoolean("SortSupported");
        }
    }
}
