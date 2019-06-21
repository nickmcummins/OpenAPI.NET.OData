// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.OData.Capabilities;
using Microsoft.OpenApi.OData.Edm;

namespace Microsoft.OpenApi.OData.PathItem
{
    /// <summary>
    /// Path item handler for single Entity.
    /// </summary>
    internal class EntityPathItemHandler : EntitySetPathItemHandler
    {
        /// <inheritdoc/>
        protected override ODataPathKind HandleKind => ODataPathKind.Entity;

        /// <inheritdoc/>
        protected override void SetOperations(OpenApiPathItem item)
        {
            ReadRestrictions read = Context.Model.GetReadRestrictions(EntitySet);
            if (read == null ||
                (read.ReadByKeyRestrictions == null && read.IsReadable) ||
                (read.ReadByKeyRestrictions.IsReadable))
            {
                // If we don't have Read by key read restriction, we should check the set read restrction.
                AddOperation(item, OperationType.Get);
            }

            UpdateRestrictions update = Context.Model.GetUpdateRestrictions(EntitySet);
            if (update == null || update.IsUpdatable)
            {
                AddOperation(item, OperationType.Patch);
            }

            DeleteRestrictions delete = Context.Model.GetDeleteRestrictions(EntitySet);
            if (delete == null || delete.IsDeletable)
            {
                AddOperation(item, OperationType.Delete);
            }
        }
    }
}
