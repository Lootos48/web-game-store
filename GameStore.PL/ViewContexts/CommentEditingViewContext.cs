using GameStore.PL.DTOs.EditDTOs;

namespace GameStore.PL.ViewContexts
{
    public class CommentEditingViewContext
    {
        public EditCommentRequestDTO CommentToEdit { get; set; }

        public string GameKey { get; set; }
    }
}
