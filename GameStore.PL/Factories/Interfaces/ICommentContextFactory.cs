using GameStore.DomainModels.Enums;
using GameStore.PL.ViewContexts;
using System;
using System.Threading.Tasks;

namespace GameStore.PL.Factories.Interfaces
{
    public interface ICommentContextFactory
    {
        Task<CommentDeletingViewContext> BuildCommentDeletingContextViewAsync(string key, Guid commentId);

        Task<GameCommentsViewContext> BuildGameCommentsContextViewAsync(string key, Guid? parentId = null, CommentType type = CommentType.Standalone);

        Task<CommentEditingViewContext> BuildCommentEditingContextViewAsync(string key, Guid commentId);
    }
}
