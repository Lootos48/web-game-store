using GameStore.DAL.SearchPipelines.Enums;
using GameStore.DAL.SearchPipelines.GoodsSearchPipeline.Interfaces;
using GameStore.DomainModels.Enums;
using GameStore.DomainModels.Models;
using System.Collections.Generic;
using System.Linq;

namespace GameStore.DAL.SearchPipelines.GoodsSearchPipeline.SorterStep
{
    public class GoodsCommentCountSorter : IGoodsSearchPipelineStep
    {
        public ExecutionPriority Priority => ExecutionPriority.BelowAverage;

        public List<Goods> Execute(List<Goods> goodsList, GoodsSearchRequest request)
        {
            if (request.SortBy != GamesSortType.MostCommented)
            {
                return goodsList;
            }

            return goodsList.OrderByDescending(g => g.Comments.Count).ToList();
        }
    }
}
