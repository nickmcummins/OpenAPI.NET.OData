// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OpenApi.OData.Authorizations;
using Microsoft.OpenApi.OData.Common;
using Microsoft.OpenApi.OData.Edm;

namespace Microsoft.OpenApi.OData.Capabilities
{
    /// <summary>
    /// Complex type: Org.OData.Capabilities.V1.PermissionType
    /// </summary>
    internal class PermissionType
    {
        /// <summary>
        /// Gets the auth flow scheme name.
        /// </summary>
        public SecurityScheme Scheme { get; private set; }

        /// <summary>
        /// Gets the list of scopes that can provide access to the resource.
        /// </summary>
        public IEnumerable<ScopeType> Scopes { get; private set; }

        /// <summary>
        /// Init the <see cref="PermissionType"/>.
        /// </summary>
        /// <param name="record">The input record.</param>
        public void Init(IEdmRecordExpression record)
        {
            Utils.CheckArgumentNull(record, nameof(record));

            // Scheme
            Scheme = record.GetRecord<SecurityScheme>("Scheme", (r, s) => r.Init(s));

            // Scopes
            Scopes = record.GetCollection<ScopeType>("Scopes", (r, s) => r.Init(s));
        }
    }
}
