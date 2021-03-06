﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschränkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading.Tasks;
using Squidex.Domain.Apps.Entities.Assets.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Validation;

namespace Squidex.Domain.Apps.Entities.Assets.Guards
{
    public static class GuardAsset
    {
        public static void CanAnnotate(AnnotateAsset command)
        {
            Guard.NotNull(command, nameof(command));

            Validate.It(() => "Cannot annotate asset.", e =>
            {
                if (string.IsNullOrWhiteSpace(command.FileName) &&
                    string.IsNullOrWhiteSpace(command.Slug) &&
                    command.IsProtected == null &&
                    command.Metadata == null &&
                    command.Tags == null)
                {
                   e("At least one property must be defined.",
                       nameof(command.FileName),
                       nameof(command.IsProtected),
                       nameof(command.Metadata),
                       nameof(command.Slug),
                       nameof(command.Tags));
                }
            });
        }

        public static Task CanCreate(CreateAsset command, IAssetQueryService assetQuery)
        {
            Guard.NotNull(command, nameof(command));

            return Validate.It(() => "Cannot upload asset.", async e =>
            {
                await CheckPathAsync(command.AppId.Id, command.ParentId, assetQuery, e);
            });
        }

        public static Task CanMove(MoveAsset command, IAssetQueryService assetQuery, DomainId oldParentId)
        {
            Guard.NotNull(command, nameof(command));

            return Validate.It(() => "Cannot move asset.", async e =>
            {
                if (command.ParentId != oldParentId)
                {
                    await CheckPathAsync(command.AppId.Id, command.ParentId, assetQuery, e);
                }
            });
        }

        public static void CanUpdate(UpdateAsset command)
        {
            Guard.NotNull(command, nameof(command));
        }

        public static void CanDelete(DeleteAsset command)
        {
            Guard.NotNull(command, nameof(command));
        }

        private static async Task CheckPathAsync(DomainId appId, DomainId parentId, IAssetQueryService assetQuery, AddValidation e)
        {
            if (parentId != DomainId.Empty)
            {
                var path = await assetQuery.FindAssetFolderAsync(appId, parentId);

                if (path.Count == 0)
                {
                    e("Asset folder does not exist.", nameof(MoveAsset.ParentId));
                }
            }
        }
    }
}
