using GameStore.PL.DTOs;

namespace GameStore.PL.ViewContexts
{
    public class CommentDeletingViewContext
    {
        public CommentDTO CommentToDelete { get; set; }

        public string GameKey { get; set; }
    }
}
