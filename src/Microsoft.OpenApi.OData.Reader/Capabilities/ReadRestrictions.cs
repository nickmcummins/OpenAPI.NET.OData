// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OpenApi.OData.Common;
using Microsoft.OpenApi.OData.Edm;

namespace Microsoft.OpenApi.OData.Capabilities
{
    internal abstract class ReadRestrictionsBase : CapabilitiesRestrictions
    {
        /// <summary>
        /// The Term type name.
        /// </summary>
        public override CapabilitesTermKind Kind => CapabilitesTermKind.ReadRestrictions;

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

        public virtual void Init(IEdmRecordExpression record)
        {
            Utils.CheckArgumentNull(record, nameof(record));

            // Readable
            Readable = record.GetBoolean("Readable");

            // Permission
            Permission = record.GetRecord<PermissionType>("Permission", (r, t) => r.Init(t));

            // CustomHeaders
            CustomHeaders = record.GetCollection<CustomParameter>("CustomHeaders", (r, t) => r.Init(t));

            // CustomQueryOptions
            CustomQueryOptions = record.GetCollection<CustomParameter>("CustomQueryOptions", (r, t) => r.Init(t));
        }
    }

    /// <summary>
    /// Restrictions for retrieving an entity by key
    /// </summary>
    internal class ReadByKeyRestrictions : ReadRestrictionsBase
    {
        protected override bool Initialize(IEdmVocabularyAnnotation annotation)
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// Org.OData.Capabilities.V1.ReadRestrictions
    /// </summary>
    internal class ReadRestrictions : ReadRestrictionsBase
    {
        public ReadByKeyRestrictions ReadByKeyRestrictions { get; set; }

        protected override bool Initialize(IEdmVocabularyAnnotation annotation)
        {
            if (annotation == null ||
                annotation.Value == null ||
                annotation.Value.ExpressionKind != EdmExpressionKind.Record)
            {
                return false;
            }

            IEdmRecordExpression record = (IEdmRecordExpression)annotation.Value;

            // Load base
            base.Init(record);

            // ReadByKeyRestrictions
            ReadByKeyRestrictions = record.GetRecord<ReadByKeyRestrictions>("ReadByKeyRestrictions", (r, t) => r.Init(t));

            return true;
        }
    }
}
