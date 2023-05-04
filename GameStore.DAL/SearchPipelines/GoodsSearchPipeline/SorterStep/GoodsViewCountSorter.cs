using GameStore.DAL.SearchPipelines.Enums;
using GameStore.DAL.SearchPipelines.GoodsSearchPipeline.Interfaces;
using GameStore.DomainModels.Enums;
using GameStore.DomainModels.Models;
using System.Collections.Generic;
using System.Linq;

namespace GameStore.DAL.SearchPipelines.GoodsSearchPipeline.SorterStep
{
    public class GoodsViewCountSorter : IGoodsSearchPipelineStep
    {
        public ExecutionPriority Priority => ExecutionPriority.BelowAverage;

        public List<Goods> Execute(List<Goods> goodsList, GoodsSearchRequest request)
        {
            if (request.SortBy != GamesSortType.MostViewed)
            {
                return goodsList;
            }

            return goodsList.OrderByDescending(g => g.ViewCount).ToList();
        }
    }
}
