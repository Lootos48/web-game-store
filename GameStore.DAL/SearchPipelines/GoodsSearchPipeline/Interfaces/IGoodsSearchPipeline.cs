using GameStore.DomainModels.Models;
using System.Collections.Generic;

namespace GameStore.DAL.SearchPipelines.GoodsSearchPipeline.Interfaces
{
    public interface IGoodsSearchPipeline
    {
        public List<Goods> Execute(List<Goods> goodsList, GoodsSearchRequest request);
    }
}
