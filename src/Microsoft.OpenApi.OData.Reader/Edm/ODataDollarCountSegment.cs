// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OpenApi.OData.Edm
{
    /// <summary>
    /// The $count segment.
    /// </summary>
    public class ODataDollarCountSegment : ODataSegment
    {
        /// <summary>
        /// Get the static instance of $count segment.
        /// </summary>
        internal static ODataDollarCountSegment Instance = new();

        /// <inheritdoc />
        public override ODataSegmentKind Kind => ODataSegmentKind.DollarCount;

        /// <inheritdoc />
        public override string Identifier => "$count";

        /// <inheritdoc />
        public override string GetPathItemName(OpenApiConvertSettings settings, HashSet<string> parameters) => "$count";
    }
}