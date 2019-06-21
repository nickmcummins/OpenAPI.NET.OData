// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OpenApi.OData.Capabilities
{
    internal abstract class ReadRestrictionsBase : CapabilitiesRestrictions
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
    }

    /// <summary>
    /// Restrictions for retrieving an entity by key
    /// </summary>
    internal class ReadByKeyRestrictions : ReadRestrictionsBase
    {
        /// <summary>
        /// The Term type name.
        /// </summary>
        public override CapabilitesTermKind Kind => CapabilitesTermKind.ReadRestrictions;

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
        /// <summary>
        /// The Term type name.
        /// </summary>
        public override CapabilitesTermKind Kind => CapabilitesTermKind.ReadRestrictions;

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


            return true;
        }
    }
}
