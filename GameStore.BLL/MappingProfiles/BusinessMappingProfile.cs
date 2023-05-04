using AutoMapper;
using GameStore.DomainModels.Models;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.DomainModels.Models.EditModels;

namespace GameStore.BLL.MappingProfiles
{
    public class BusinessMappingProfile : Profile
    {
        public BusinessMappingProfile()
        {
            #region Comment

            CreateMap<CreateCommentRequest, Comment>()
                .ForMember(bo => bo.Author,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Parent,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Replies,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Id,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.IsDeleted,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.GameId,
                    cfg => cfg.Ignore());

            CreateMap<EditCommentRequest, Comment>()
                .ForMember(bo => bo.Author,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Parent,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Replies,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.IsDeleted,
                    cfg => cfg.Ignore());

            #endregion

            #region Game

            CreateMap<CreateGoodsRequest, Goods>()
                .ForMember(bo => bo.QuantityPerUnit,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.UnitsInOrder,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.ReorderLevel,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Localizations,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Discontinued,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.ViewCount,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Comments,
                    cfg => cfg.Ignore());

            CreateMap<EditGoodsRequest, Goods>()
                .ForMember(bo => bo.Distributor,
                    cfg => cfg.MapFrom(request => new Distributor { Id = request.DistributorId }))
                .ForMember(bo => bo.Comments,
                    cfg => cfg.Ignore());

            #endregion

            #region Genre

            CreateMap<CreateGenreRequest, Genre>()
                .ForMember(bo => bo.Localizations,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Id,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Parent,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.SubGenres,
                    cfg => cfg.Ignore());

            CreateMap<EditGenreRequest, Genre>()
                .ForMember(bo => bo.Parent,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.SubGenres,
                    cfg => cfg.Ignore());

            #endregion

            #region OrderDetails

            CreateMap<CreateOrderDetailsRequest, OrderDetails>()
                .ForMember(bo => bo.Id,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Product,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Order,
                    cfg => cfg.Ignore());

            CreateMap<EditOrderDetailsRequest, OrderDetails>()
                .ForMember(bo => bo.Product,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.ProductId,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Order,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.OrderId,
                    cfg => cfg.Ignore());

            #endregion

            #region PlatformType

            CreateMap<CreatePlatformTypeRequest, PlatformType>()
                .ForMember(bo => bo.Localizations,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Id,
                    cfg => cfg.Ignore());

            CreateMap<EditPlatformRequest, PlatformType>();

            #endregion

            #region Publisher

            CreateMap<CreateDistributorRequest, Distributor>()
                .ForMember(bo => bo.UserId,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Id,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.DistributedGoods,
                    cfg => cfg.Ignore());

            CreateMap<EditDistributorRequest, Distributor>()
                .ForMember(bo => bo.DistributedGoods,
                    cfg => cfg.Ignore());

            #endregion

            #region Order

            CreateMap<CreateOrderRequest, Order>()
                .ForMember(bo => bo.Id,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.RequiredDate,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.ShippedDate,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.ShipVia,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Freight,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.ShipName,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.ShipCity,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.ShipRegion,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.ShipPostalCode,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.ShipCountry,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Shipper,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.ShipAddress,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Status,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.ExpirationDateUtc,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.OrderDetails,
                    cfg => cfg.Ignore());

            CreateMap<EditOrderRequest, Order>()
                .ForMember(bo => bo.Shipper,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.RequiredDate,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.OrderDetails,
                    cfg => cfg.MapFrom(request => request.OrderDetails))
                .ReverseMap();

            #endregion
        }
    }
}
