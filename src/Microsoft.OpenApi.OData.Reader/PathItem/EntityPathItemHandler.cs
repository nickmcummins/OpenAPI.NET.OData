// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.OData.Edm;
using Microsoft.OpenApi.OData.Vocabulary.Capabilities;

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
            ReadRestrictionsType read = Context.Model.GetRecord<ReadRestrictionsType>(EntitySet, CapabilitiesConstants.ReadRestrictions);
            if (read == null ||
               (read.ReadByKeyRestrictions == null && read.IsReadable) ||
               (read.ReadByKeyRestrictions != null && read.ReadByKeyRestrictions.IsReadable))
            {
                // If we don't have Read by key read restriction, we should check the set read restrction.
                AddOperation(item, OperationType.Get);
            }

            UpdateRestrictionsType update = Context.Model.GetRecord<UpdateRestrictionsType>(EntitySet, CapabilitiesConstants.UpdateRestrictions);
            if (update == null || update.IsUpdatable)
            {
                AddOperation(item, OperationType.Patch);
            }

            DeleteRestrictionsType delete = Context.Model.GetRecord<DeleteRestrictionsType>(EntitySet, CapabilitiesConstants.DeleteRestrictions);
            if (delete == null || delete.IsDeletable)
            {
                AddOperation(item, OperationType.Delete);
            }
        }
    }
}
