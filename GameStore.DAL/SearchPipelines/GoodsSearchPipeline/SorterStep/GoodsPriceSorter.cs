﻿using GameStore.DAL.SearchPipelines.Enums;
using GameStore.DAL.SearchPipelines.GoodsSearchPipeline.Interfaces;
using GameStore.DomainModels.Enums;
using GameStore.DomainModels.Models;
using System.Collections.Generic;
using System.Linq;

namespace GameStore.DAL.SearchPipelines.GoodsSearchPipeline.SorterStep
{
    public class GoodsPriceSorter : IGoodsSearchPipelineStep
    {
        public ExecutionPriority Priority => ExecutionPriority.BelowAverage;

        public List<Goods> Execute(List<Goods> goodsList, GoodsSearchRequest request)
        {
            if (request.SortBy == GamesSortType.PriceAscending)
            {
                return goodsList.OrderBy(g => g.Price).ToList();
            }
            else if (request.SortBy == GamesSortType.PriceDescending)
            {
                return goodsList.OrderByDescending(g => g.Price).ToList();
            }

            return goodsList;
        }
    }
}
