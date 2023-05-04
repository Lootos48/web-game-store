using GameStore.DAL.SearchPipelines.Enums;
using GameStore.DAL.SearchPipelines.GoodsSearchPipeline.Interfaces;
using GameStore.DomainModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameStore.DAL.SearchPipelines.GoodsSearchPipeline.Pagination
{
    class GoodsPaginationStep : IGoodsSearchPipelineStep
    {
        public ExecutionPriority Priority => ExecutionPriority.Low;

        public List<Goods> Execute(List<Goods> goodsList, GoodsSearchRequest request)
        {
            if (IsRangeValuesNotAssigned(request.CurrentPage, request.ItemsPerPage))
            {
                return goodsList;
            }

            ValidatePageValues(request.CurrentPage, request.ItemsPerPage);

            return goodsList.Skip((request.CurrentPage - 1) * request.ItemsPerPage).Take(request.ItemsPerPage).ToList();
        }

        private static bool IsRangeValuesNotAssigned(int CurrentPage, int ItemsPerPage)
        {
            return CurrentPage == 0 && ItemsPerPage == 0;
        }

        private static void ValidatePageValues(int CurrentPage, int ItemsPerPage)
        {
            if (ItemsPerPage <= 0)
            {
                throw new ArgumentException("Items per page must be greater than zero");
            }

            if (CurrentPage < 0)
            {
                throw new ArgumentException("Current page cannot be less than zero");
            }
        }
    }
}