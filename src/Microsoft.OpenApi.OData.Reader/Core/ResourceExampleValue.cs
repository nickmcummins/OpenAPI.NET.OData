// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OpenApi.OData.Common;

namespace Microsoft.OpenApi.OData.Core
{
    /// <summary>
    /// Base class for <see cref="EntityExampleValue"/> and <see cref="ComplexExampleValue"/>.
    /// </summary>
    internal abstract class ResourceExampleValue : ExampleValue
    {
        /// <summary>
        /// Gets the Example value for the custom parameter
        /// </summary>
        public ODataResourceValue Value { get; private set; }

        /// <summary>
        /// Init the <see cref="EntityExampleValue"/>
        /// </summary>
        /// <param name="record">The input record.</param>
        public override void Init(IEdmRecordExpression record)
        {
            Utils.CheckArgumentNull(record, nameof(record));

            // Load ExampleValue
            base.Init(record);

            // Value of PrimitiveExampleValue
            IEdmPropertyConstructor property = record.FindProperty("Value");
            if (property != null)
            {
                Value = property.Value.Convert() as ODataResourceValue;
            }
        }
    }
}
