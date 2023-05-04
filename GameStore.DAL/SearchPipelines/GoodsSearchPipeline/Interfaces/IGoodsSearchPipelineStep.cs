using GameStore.DAL.SearchPipelines.Enums;
using GameStore.DomainModels.Models;
using System.Collections.Generic;

namespace GameStore.DAL.SearchPipelines.GoodsSearchPipeline.Interfaces
{
    public interface IGoodsSearchPipelineStep
    {
        public ExecutionPriority Priority { get; }

        public List<Goods> Execute(List<Goods> goodsList, GoodsSearchRequest request);
    }
}