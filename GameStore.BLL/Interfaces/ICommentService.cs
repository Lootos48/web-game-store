using GameStore.DomainModels.Models;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.DomainModels.Models.EditModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.BLL.Interfaces
{
    public interface ICommentService
    {
        Task CreateCommentAsync(string key, CreateCommentRequest commentToCreate);
        Task<List<Comment>> GetByGameKeyAsync(string key, bool isIncludeDeleted = false);
        Task<Comment> GetCommentByIdAsync(Guid id);
        Task<List<Comment>> GetAllCommentsAsync();
        Task EditCommentAsync(EditCommentRequest commentToEdit);
        Task SoftDeleteCommentAsync(Guid id);
        Task HardDeleteCommentAsync(Guid id);
    }
}
