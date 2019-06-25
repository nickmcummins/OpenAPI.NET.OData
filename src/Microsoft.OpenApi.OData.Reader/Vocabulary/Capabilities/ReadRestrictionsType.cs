// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OpenApi.OData.Common;
using Microsoft.OpenApi.OData.Edm;

namespace Microsoft.OpenApi.OData.Vocabulary.Capabilities
{
    internal abstract class ReadRestrictionsBase : IRecord
    {
        /// <summary>
        /// Get the Entities can be retrieved.
        /// </summary>
        public bool? Readable { get; private set; }

        /// <summary>
        /// Gets the List of required scopes to invoke an action or function
        /// </summary>
        public PermissionType Permission { get; private set; }

        /// <summary>
        /// Gets the Supported or required custom headers.
        /// </summary>
        public IList<CustomParameter> CustomHeaders { get; private set; }

        /// <summary>
        /// Gets the Supported or required custom query options.
        /// </summary>
        public IList<CustomParameter> CustomQueryOptions { get; private set; }

        /// <summary>
        /// Test the target supports update.
        /// </summary>
        /// <returns>True/false.</returns>
        public bool IsReadable => Readable == null || Readable.Value;

        public virtual void Initialize(IEdmRecordExpression record)
        {
            Utils.CheckArgumentNull(record, nameof(record));

            // Readable
            Readable = record.GetBoolean("Readable");

            // Permission
            Permission = record.GetRecord<PermissionType>("Permission");

            // CustomHeaders
            CustomHeaders = record.GetCollection<CustomParameter>("CustomHeaders");

            // CustomQueryOptions
            CustomQueryOptions = record.GetCollection<CustomParameter>("CustomQueryOptions");
        }
    }

    /// <summary>
    /// Restrictions for retrieving an entity by key
    /// </summary>
    internal class ReadByKeyRestrictions : ReadRestrictionsBase
    {
    }

    /// <summary>
    /// Complex Type: Org.OData.Capabilities.V1.ReadRestrictionsType
    /// </summary>
    [Term("Org.OData.Capabilities.V1.ReadRestrictions")]
    internal class ReadRestrictionsType : ReadRestrictionsBase
    {
        public ReadByKeyRestrictions ReadByKeyRestrictions { get; set; }

        public override void Initialize(IEdmRecordExpression record)
        {
            // Load base
            base.Initialize(record);

            // ReadByKeyRestrictions
            ReadByKeyRestrictions = record.GetRecord<ReadByKeyRestrictions>("ReadByKeyRestrictions");
        }
    }
}
