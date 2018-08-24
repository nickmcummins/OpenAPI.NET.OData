﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OpenApi.OData.Capabilities
{
    /// <summary>
    /// Org.OData.Capabilities.V1.IndexableByKey
    /// </summary>
    internal class IndexableByKey : SupportedRestrictions
    {
        /// <summary>
        /// The Term type kind.
        /// </summary>
        public override CapabilitesTermKind Kind => CapabilitesTermKind.IndexableByKey;
    }
}
