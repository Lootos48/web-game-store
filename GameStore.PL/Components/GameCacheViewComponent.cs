using GameStore.BLL.Interfaces;

namespace GameStore.PL.Components
{
    public class GamesCacheViewComponent
    {
        private readonly IGoodsService _gameService;

        public GamesCacheViewComponent(IGoodsService gameService)
        {
            _gameService = gameService;
        }

        public string Invoke()
        {
            return _gameService.GetCountOfGoodsAsync().Result.ToString();
        }
    }
}
