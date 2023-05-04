using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.DomainModels.Models;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.DomainModels.Models.EditModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStore.DomainModels.Exceptions;
using GameStore.DAL.Abstractions.Interfaces;

namespace GameStore.BLL.Services
{
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;
        private readonly IGoodsRepository _gameRepository;
        private readonly IMapper _mapper;

        public GenreService(
            IGenreRepository genreRepository,
            IGoodsRepository gameRepository,
            IMapper mapper)
        {
            _genreRepository = genreRepository;
            _gameRepository = gameRepository;
            _mapper = mapper;
        }

        public async Task CreateGenreAsync(CreateGenreRequest genreToCreate)
        {
            Genre createGenre = _mapper.Map<Genre>(genreToCreate);

            bool isUnique = await _genreRepository.IsGenreUnique(createGenre);
            if (!isUnique)
            {
                throw new NotUniqueException("Genre with that name is already exist");
            }

            await _genreRepository.CreateAsync(createGenre);
        }

        public async Task<Genre> GetGenreByIdAsync(Guid id)
        {
            Genre foundedGenre = await _genreRepository.FindByIdAsync(id);
            if (foundedGenre is null)
            {
                throw new NotFoundException($"Genre with that id wasn`t found: {id}");
            }

            return foundedGenre;
        }

        public async Task<List<Goods>> GetGamesByGenreAsync(string genre, string localizationCode)
        {
            Genre foundedGenre = await _genreRepository.FindByNameAsync(genre);
            if (foundedGenre is null)
            {
                throw new NotFoundException($"Genre with that name wasn`t found: {genre}");
            }

            var foundedGames = await _gameRepository.FindByGenreAsync(foundedGenre.Id, localizationCode);

            return foundedGames;
        }

        public Task<List<Genre>> GetAllGenresAsync(string localizationCultureCode = null)
        {
            return _genreRepository.GetAllAsync(localizationCultureCode);
        }

        public async Task<Genre> GetGenreByNameWithAllLocalizationsAsync(string genre)
        {
            Genre foundedGenre = await _genreRepository.FindByNameIncludeAllLocalizationsAsync(genre);
            if (foundedGenre == null)
            {
                throw new NotFoundException($"Genre with that name wasn`t found: {genre}");
            }

            return foundedGenre;
        }

        public async Task<Genre> GetGenreByNameAsync(string genre, string localizationCode = null)
        {
            Genre foundedGenre = await _genreRepository.FindByNameAsync(genre, localizationCode);
            if (foundedGenre == null)
            {
                throw new NotFoundException($"Genre with that name wasn`t found: {genre}");
            }

            return foundedGenre;
        }

        public async Task EditGenreAsync(EditGenreRequest gameToEdit)
        {
            Genre changedGenre = _mapper.Map<Genre>(gameToEdit);

            bool isUnique = await _genreRepository.IsGenreUnique(changedGenre);
            if (!isUnique)
            {
                throw new NotUniqueException("Genre with that name is already exist");
            }

            if (gameToEdit.ChosedLocalization.HasValue)
            {
                await _genreRepository.UpdateLocalizationAsync(changedGenre, gameToEdit.ChosedLocalization.Value);
            }
            else
            {
                await _genreRepository.UpdateAsync(changedGenre);
            }
        }

        public async Task SoftDeleteGenreAsync(Guid id)
        {
            Genre genreToDelete = await _genreRepository.FindByIdAsync(id);
            if (genreToDelete is null)
            {
                throw new NotFoundException($"Genre with id {id} wasn't found");
            }

            await RaiseChildGenres(genreToDelete);

            await _genreRepository.SoftDeleteAsync(id);
        }

        public async Task HardDeleteGenreAsync(Guid id)
        {
            Genre genreToDelete = await _genreRepository.FindByIdAsync(id);
            if (genreToDelete is null)
            {
                throw new NotFoundException($"Genre with id {id} wasn't found");
            }

            await RaiseChildGenres(genreToDelete);
            await _genreRepository.DeleteByIdAsync(id);
        }

        private async Task RaiseChildGenres(Genre parentGenre)
        {
            List<Genre> childGenres = await _genreRepository.GetChildGenresAsync(parentGenre.Id);
            if (!childGenres.Any())
            {
                return;
            }

            childGenres.ForEach(g => g.ParentId = parentGenre.ParentId);
            await _genreRepository.BatchUpdateParentsAsync(childGenres);
        }
    }
}
