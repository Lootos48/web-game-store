using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.DomainModels.Enums;
using GameStore.DomainModels.Models;
using GameStore.PL.DTOs;
using GameStore.PL.DTOs.CreateDTOs;
using GameStore.PL.DTOs.EditDTOs;
using GameStore.PL.Factories.Interfaces;
using GameStore.PL.ViewContexts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.PL.Factories
{
    public class CommentContextFactory : ICommentContextFactory
    {
        private readonly ICommentService _commentService;
        private readonly IMapper _mapper;

        public CommentContextFactory(
            ICommentService commentService,
            IMapper mapper)
        {
            _commentService = commentService;
            _mapper = mapper;
        }

        public async Task<CommentDeletingViewContext> BuildCommentDeletingContextViewAsync(string key, Guid commentId)
        {
            Comment foundedComment = await _commentService.GetCommentByIdAsync(commentId);

            CommentDTO commentToDelete = _mapper.Map<CommentDTO>(foundedComment);

            CommentDeletingViewContext context = new CommentDeletingViewContext
            {
                CommentToDelete = commentToDelete,
                GameKey = key
            };

            return context;
        }

        public async Task<CommentEditingViewContext> BuildCommentEditingContextViewAsync(string key, Guid commentId)
        {
            Comment foundedComment = await _commentService.GetCommentByIdAsync(commentId);

            EditCommentRequestDTO commentToEdit = _mapper.Map<EditCommentRequestDTO>(foundedComment);

            CommentEditingViewContext context = new CommentEditingViewContext
            {
                CommentToEdit = commentToEdit,
                GameKey = key
            };

            return context;
        }

        public async Task<GameCommentsViewContext> BuildGameCommentsContextViewAsync(string key, Guid? parentId = null, CommentType type = CommentType.Standalone)
        {
            List<Comment> comments = await _commentService.GetByGameKeyAsync(key, true);

            CreateCommentRequestDTO comment = new CreateCommentRequestDTO()
            {
                ParentId = parentId,
                Type = type
            };

            GameCommentsViewContext context = new GameCommentsViewContext()
            {
                GameKey = key,
                CommentToCreate = comment,
                Comments = _mapper.Map<List<CommentDTO>>(comments)
            };

            return context;
        }
    }
}
