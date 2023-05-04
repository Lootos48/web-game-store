using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.DAL.Abstractions.Interfaces;
using GameStore.DomainModels.Exceptions;
using GameStore.DomainModels.Models;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.DomainModels.Models.EditModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.BLL.Services
{
    public class CommentService : ICommentService
    {
        private static readonly string DeletedCommentBody = "Comment was deleted";
        private readonly ICommentRepository _commentRepository;
        private readonly IGoodsRepository _gameRepository;
        private readonly IMapper _mapper;

        public CommentService(
            ICommentRepository commentRepository, 
            IGoodsRepository gameRepository,
            IMapper mapper)
        {
            _commentRepository = commentRepository;
            _gameRepository = gameRepository;
            _mapper = mapper;
        }

        public async Task CreateCommentAsync(string key, CreateCommentRequest commentToCreate)
        {
            Comment comment = _mapper.Map<Comment>(commentToCreate);

            Goods game = await _gameRepository.FindByKeyAsync(key);
            if (game is null)
            {
                throw new NotFoundException($"Game with that Key wasn`t found: {key}");
            }

            comment.GameId = Guid.Parse(game.Id);
            await _commentRepository.CreateAsync(comment);
        }

        public async Task<List<Comment>> GetByGameKeyAsync(string key, bool isIncludeDeleted = false)
        {
            Goods game = await _gameRepository.FindByKeyAsync(key, isIncludeDeleted: isIncludeDeleted);
            if (game is null)
            {
                throw new NotFoundException($"Game with that Key wasn`t found: {key}");
            }

            List<Comment> gameComments = await _commentRepository.GetByGameIdAsync(Guid.Parse(game.Id), isIncludeDeleted);
            if (isIncludeDeleted)
            {
                ModerateDeletedComments(gameComments);
            }

            return gameComments;
        }

        public async Task<Comment> GetCommentByIdAsync(Guid id)
        {
            Comment foundedComment = await _commentRepository.FindByIdAsync(id);
            if (foundedComment is null)
            {
                throw new NotFoundException($"Comment with id wasn`t found: {id}");
            }

            return foundedComment;
        }

        public Task<List<Comment>> GetAllCommentsAsync()
        {
            return _commentRepository.GetAllAsync();
        }

        public Task EditCommentAsync(EditCommentRequest commentToEdit)
        {
            Comment createComment = _mapper.Map<Comment>(commentToEdit);

            return _commentRepository.UpdateAsync(createComment);
        }

        public async Task SoftDeleteCommentAsync(Guid id)
        {
            try
            {
                await _commentRepository.SoftDeleteAsync(id);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException($"Comment with id {id} wasn't found");
            }
        }

        public async Task HardDeleteCommentAsync(Guid id)
        {
            try
            {
                await _commentRepository.DeleteByIdAsync(id);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException($"Comment with id {id} wasn't found");
            }
        }

        private void ModerateDeletedComments(List<Comment> comments)
        {
            comments.RemoveAll(c => c.IsDeleted && !c.Replies.Any(x => !x.IsDeleted));

            var commentsToUpdate = comments.Where(c => c.IsDeleted && c.Replies.Any(x => !x.IsDeleted));

            foreach (Comment commentToUpdate in commentsToUpdate)
            {
                commentToUpdate.Body = DeletedCommentBody;
            }
        }
    }
}
