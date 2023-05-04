using GameStore.PL.DTOs;
using GameStore.PL.DTOs.CreateDTOs;
using System.Collections.Generic;

namespace GameStore.PL.ViewContexts
{
    public class GameCommentsViewContext
    {
        public string GameKey { get; set; }

        public CreateCommentRequestDTO CommentToCreate { get; set; }

        public List<CommentDTO> Comments { get; set; }
    }
}
