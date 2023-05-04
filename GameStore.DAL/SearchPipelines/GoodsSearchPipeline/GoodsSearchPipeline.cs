using System.Collections.Generic;
using System.Linq;
using GameStore.DAL.SearchPipelines.GoodsSearchPipeline.Interfaces;
using GameStore.DomainModels.Models;

namespace GameStore.DAL.SearchPipelines.GoodsSearchPipeline
{
    public class GoodsSearchPipeline : IGoodsSearchPipeline
    {
        private readonly IEnumerable<IGoodsSearchPipelineStep> _pipelineSteps;

        public GoodsSearchPipeline(IEnumerable<IGoodsSearchPipelineStep> pipelineSteps)
        {
            _pipelineSteps = SortPipelineSteps(pipelineSteps);
        }

        public List<Goods> Execute(List<Goods> goodsList, GoodsSearchRequest request)
        {
            foreach (var pipelineStep in _pipelineSteps)
            {
                goodsList = pipelineStep.Execute(goodsList, request);
            }

            return goodsList;
        }

        private static IEnumerable<IGoodsSearchPipelineStep> SortPipelineSteps(IEnumerable<IGoodsSearchPipelineStep> pipelineSteps)
        {
            return pipelineSteps.OrderByDescending(step => step.Priority);
        }
    }
}
