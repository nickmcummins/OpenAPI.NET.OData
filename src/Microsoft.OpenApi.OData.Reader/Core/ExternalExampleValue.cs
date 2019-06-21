// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OpenApi.OData.Common;
using Microsoft.OpenApi.OData.Edm;

namespace Microsoft.OpenApi.OData.Core
{
    /// <summary>
    /// Complex type: Org.OData.Core.V1.ExternalExampleValue.
    /// </summary>
    internal class ExternalExampleValue : ExampleValue
    {
        /// <summary>
        /// Gets the Url reference to the value in its literal format
        /// </summary>
        public string ExternalValue { get; set; }

        /// <summary>
        /// Init the <see cref="ComplexExampleValue"/>
        /// </summary>
        /// <param name="record">The input record.</param>
        public override void Init(IEdmRecordExpression record)
        {
            Utils.CheckArgumentNull(record, nameof(record));

            // Load ExampleValue
            base.Init(record);

            // ExternalValue
            ExternalValue = record.GetString("ExternalValue");
        }
    }
}
