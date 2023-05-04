using AutoMapper;
using GameStore.DomainModels.Models;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.DomainModels.Models.EditModels;
using GameStore.DAL.Entities;
using System.Linq;
using GameStore.DAL.Entities.MongoEntities;
using System;
using GameStore.DAL.Entities.Localizations;
using GameStore.DomainModels.Models.Localizations;

namespace GameStore.DAL.MappingProfiles
{
    public class EntityDomainMapperProfile : Profile
    {
        public EntityDomainMapperProfile()
        {
            #region Game Maps

            CreateMap<GameEntity, Goods>()
                .ForMember(bo => bo.UnitsInOrder,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.DistributorId,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Distributor,
                    cfg => cfg.MapFrom(entity => entity.Publisher))
                .ForMember(bo => bo.Genres,
                    cfg => cfg.MapFrom(
                        entity => entity.GameGenres.Select(gg => gg.Genre).ToList()))
                .ForMember(bo => bo.PlatformTypes,
                    cfg => cfg.MapFrom(
                        entity => entity.GamesPlatformTypes.Select(gp => gp.PlatformType).ToList()));

            CreateMap<Goods, GameEntity>()
                .AfterMap((bo, entity) =>
                 {
                     foreach (var item in bo.Genres)
                     {
                         entity.GameGenres.Add(new GameGenreEntity()
                         {
                             GameId = entity.Id,
                             GenreId = item.Id
                         });
                     }
                 })
                .AfterMap((bo, entity) =>
                {
                    foreach (var item in bo.PlatformTypes)
                    {
                        entity.GamesPlatformTypes.Add(new GamePlatformTypeEntity()
                        {
                            GameId = entity.Id,
                            PlatformId = item.Id
                        });
                    }
                })
                .ForMember(entity => entity.Publisher, cfg => cfg.Ignore())
                .ForMember(entity => entity.UnitsOnOrder, cfg => cfg.Ignore())
                .ForMember(entity => entity.PublisherId, cfg => cfg.Ignore())
                .ForMember(entity => entity.GameGenres, cfg => cfg.Ignore())
                .ForMember(entity => entity.GamesPlatformTypes, cfg => cfg.Ignore())
                .ForMember(entity => entity.IsDeleted, cfg => cfg.Ignore())
                .ForMember(entity => entity.DateOfDelete, cfg => cfg.Ignore());

            CreateMap<CreateGoodsRequest, GameEntity>()
                .AfterMap((bo, entity) =>
                {
                    foreach (var item in bo.Genres)
                    {
                        entity.GameGenres.Add(new GameGenreEntity()
                        {
                            GameId = entity.Id,
                            GenreId = item.Id
                        });
                    }
                })
                .AfterMap((bo, entity) =>
                {
                    foreach (var item in bo.PlatformTypes)
                    {
                        entity.GamesPlatformTypes.Add(new GamePlatformTypeEntity()
                        {
                            GameId = entity.Id,
                            PlatformId = item.Id
                        });
                    }
                })
                .ForMember(entity => entity.QuantityPerUnit, cfg => cfg.Ignore())
                .ForMember(entity => entity.UnitsOnOrder, cfg => cfg.Ignore())
                .ForMember(entity => entity.ReorderLevel, cfg => cfg.Ignore())
                .ForMember(entity => entity.Localizations, cfg => cfg.Ignore())
                .ForMember(entity => entity.PublisherId, cfg => cfg.Ignore())
                .ForMember(entity => entity.ViewCount, cfg => cfg.Ignore())
                .ForMember(entity => entity.Comments, cfg => cfg.Ignore())
                .ForMember(entity => entity.GameGenres, cfg => cfg.Ignore())
                .ForMember(entity => entity.GamesPlatformTypes, cfg => cfg.Ignore())
                .ForMember(entity => entity.IsDeleted, cfg => cfg.Ignore())
                .ForMember(entity => entity.DateOfDelete, cfg => cfg.Ignore())
                .ForMember(entity => entity.Publisher, cfg => cfg.Ignore())
                .ForMember(entity => entity.Discontinued, cfg => cfg.Ignore());

            CreateMap<EditGoodsRequest, GameEntity>()
                .ForMember(entity => entity.UnitsOnOrder, cfg => cfg.Ignore())
                .ForMember(entity => entity.Publisher, cfg => cfg.Ignore())
                .ForMember(entity => entity.PublisherId, cfg => cfg.Ignore())
                .ForMember(entity => entity.DateOfAdding, cfg => cfg.Ignore())
                .ForMember(entity => entity.GamesPlatformTypes, cfg => cfg.Ignore())
                .ForMember(entity => entity.Comments, cfg => cfg.Ignore())
                .ForMember(entity => entity.GameGenres, cfg => cfg.Ignore())
                .ForMember(entity => entity.IsDeleted, cfg => cfg.Ignore())
                .ForMember(entity => entity.DateOfDelete, cfg => cfg.Ignore());

            #endregion

            #region Comment Maps

            CreateMap<CreateCommentRequest, CommentEntity>()
                .ForMember(entity => entity.Author, cfg => cfg.Ignore())
                .ForMember(entity => entity.GameId, cfg => cfg.Ignore())
                .ForMember(entity => entity.Replies, cfg => cfg.Ignore())
                .ForMember(entity => entity.Id, cfg => cfg.Ignore())
                .ForMember(entity => entity.Parent, cfg => cfg.Ignore())
                .ForMember(entity => entity.IsDeleted, cfg => cfg.Ignore())
                .ForMember(entity => entity.Game, cfg => cfg.Ignore())
                .ForMember(entity => entity.DateOfDelete, cfg => cfg.Ignore());

            CreateMap<EditCommentRequest, CommentEntity>()
                .ForMember(entity => entity.Author, cfg => cfg.Ignore())
                .ForMember(entity => entity.Game, cfg => cfg.Ignore())
                .ForMember(entity => entity.Replies, cfg => cfg.Ignore())
                .ForMember(entity => entity.Parent, cfg => cfg.Ignore())
                .ForMember(entity => entity.DateOfDelete, cfg => cfg.Ignore())
                .ForMember(entity => entity.IsDeleted, cfg => cfg.Ignore());

            CreateMap<CommentEntity, Comment>()
                .ReverseMap();

            #endregion

            #region Genre Maps

            CreateMap<GenreEntity, Genre>()
                .ReverseMap();

            CreateMap<EditGenreRequest, GenreEntity>()
                .ForMember(entity => entity.SubGenres,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.IsDeleted,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.DateOfDelete,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.Parent,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.GameGenres,
                    cfg => cfg.Ignore());

            CreateMap<CreateGenreRequest, GenreEntity>()
                .ForMember(entity => entity.Id,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.Localizations,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.SubGenres,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.Parent,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.DateOfDelete,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.GameGenres,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.IsDeleted,
                    cfg => cfg.Ignore());

            #endregion

            #region PlatformType Maps

            CreateMap<EditPlatformRequest, PlatformTypeEntity>()
                .ForMember(entity => entity.IsDeleted,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.GamesPlatformTypes,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.DateOfDelete,
                    cfg => cfg.Ignore());

            CreateMap<CreatePlatformTypeRequest, PlatformTypeEntity>()
                .ForMember(entity => entity.Id,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.Localizations,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.IsDeleted,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.DateOfDelete,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.GamesPlatformTypes,
                    cfg => cfg.Ignore());

            CreateMap<PlatformTypeEntity, PlatformType>()
                .ReverseMap();

            #endregion

            #region Publisher

            CreateMap<Distributor, PublisherEntity>()
                .ForMember(entity => entity.PublishedGames,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.User,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.IsDeleted,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.DateOfDelete,
                    cfg => cfg.Ignore())
                .ReverseMap();

            CreateMap<CreateDistributorRequest, PublisherEntity>()
                .ForMember(entity => entity.User,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.UserId,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.IsDeleted,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.Id,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.DateOfDelete,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.PublishedGames,
                    cfg => cfg.Ignore());

            CreateMap<EditDistributorRequest, PublisherEntity>()
                .ForMember(entity => entity.User,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.IsDeleted,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.DateOfDelete,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.PublishedGames,
                    cfg => cfg.Ignore());

            #endregion

            #region Order Maps

            CreateMap<Order, OrderEntity>()
                .ForMember(entity => entity.Customer,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.IsDeleted,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.DateOfDelete,
                    cfg => cfg.Ignore())
                .ReverseMap();

            CreateMap<EditOrderRequest, OrderEntity>()
                .ForMember(entity => entity.RequiredDate,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.Customer,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.OrderDetails,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.IsDeleted,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.DateOfDelete,
                    cfg => cfg.Ignore());

            CreateMap<CreateOrderRequest, OrderEntity>()
                .ForMember(entity => entity.Customer,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.RequiredDate,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.ShippedDate,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.ShipVia,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.Freight,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.ShipName,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.ShipAddress,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.ShipCity,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.ShipRegion,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.ShipPostalCode,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.ShipCountry,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.DateOfDelete,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.OrderDetails,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.Id,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.Status,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.ExpirationDateUtc,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.IsDeleted,
                    cfg => cfg.Ignore());

            #endregion

            #region OrderDetails

            CreateMap<OrderDetailsEntity, OrderDetails>();

            CreateMap<OrderDetails, OrderDetailsEntity>()
                .ForMember(entity => entity.ProductId,
                    cfg =>
                    {
                        cfg.PreCondition(model => Guid.TryParse(model.ProductId, out Guid gameId));
                        cfg.MapFrom(model => Guid.Parse(model.ProductId));
                    })
                .ForMember(entity => entity.DateOfDelete,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.Product,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.IsDeleted,
                    cfg => cfg.Ignore());

            CreateMap<CreateOrderDetailsRequest, OrderDetailsEntity>()
                .ForMember(entity => entity.Product,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.Id,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.IsDeleted,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.DateOfDelete,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.Order,
                    cfg => cfg.Ignore());

            CreateMap<EditOrderDetailsRequest, OrderDetailsEntity>()
                .ForMember(entity => entity.ProductId,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.Product,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.Order,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.OrderId,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.IsDeleted,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.DateOfDelete,
                    cfg => cfg.Ignore());

            #endregion

            #region GameGenre

            CreateMap<GameGenre, GameGenreEntity>()
                .ForMember(gge => gge.Game,
                    cfg => cfg.Ignore())
                .ForMember(gge => gge.Genre,
                    cfg => cfg.Ignore());

            CreateMap<GameGenreEntity, GameGenre>()
                .ForMember(gg => gg.Game,
                    cfg => cfg.Ignore())
                .ForMember(gg => gg.Genre,
                    cfg => cfg.Ignore());

            #endregion

            #region User Maps

            CreateMap<User, UserEntity>()
                .ForMember(entity => entity.IsDeleted,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.DateOfDelete,
                    cfg => cfg.Ignore())
                .ReverseMap();

            #endregion

            #region GamePlatformType

            CreateMap<GamePlatformType, GamePlatformTypeEntity>()
                .ForMember(gpte => gpte.Game,
                    cfg => cfg.Ignore())
                .ForMember(gpte => gpte.PlatformType,
                    cfg => cfg.Ignore());

            CreateMap<GamePlatformTypeEntity, GamePlatformType>();

            #endregion

            #region OrderDetailsMongo

            CreateMap<OrderDetailsMongoEntity, OrderDetails>()
                .ForMember(domain => domain.Discount,
                    cfg => cfg.MapFrom(entity => entity.Discount))
                .ForMember(domain => domain.Quantity,
                    cfg => cfg.MapFrom(entity => entity.Quantity))
                .ForMember(domain => domain.Price,
                    cfg => cfg.MapFrom(entity => entity.UnitPrice))
                .ForMember(domain => domain.Id,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.Order,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.Product,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.ProductId,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.OrderId,
                    cfg => cfg.Ignore());

            #endregion

            #region OrderMongo

            CreateMap<OrderMongoEntity, Order>()
                .ForMember(domain => domain.Shipper,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.ExpirationDateUtc,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.Id,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.Status,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.OrderDetails,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.CustomerId,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.OrderDate,
                    cfg => cfg.MapFrom(entity => entity.OrderDate.Value));

            #endregion

            #region SupplierMongoEntity

            CreateMap<SupplierMongoEntity, Distributor>()
                .ForMember(domain => domain.Id,
                    cfg => cfg.MapFrom(mongo => mongo.SupplierId))
                .ForMember(domain => domain.UserId,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.DistributedGoods,
                    cfg => cfg.Ignore());

            #endregion

            #region ProductMongoEntity

            CreateMap<ProductMongoEntity, Goods>()
                .ForMember(domain => domain.Name,
                    cfg => cfg.MapFrom(entity => entity.ProductName))
                .ForMember(domain => domain.Key,
                    cfg => cfg.MapFrom(entity => entity.ProductKey))
                .ForMember(domain => domain.Discontinued,
                    cfg => cfg.MapFrom(entity => entity.Discontinued))
                .ForMember(domain => domain.Price,
                    cfg => cfg.MapFrom(entity => entity.UnitPrice))
                .ForMember(domain => domain.ViewCount,
                    cfg => cfg.MapFrom(entity => entity.ViewCount))
                .ForMember(domain => domain.Id,
                    cfg => cfg.MapFrom(entity => entity.ProductId))
                .ForMember(domain => domain.Distributor,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.Localizations,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.DateOfPublishing,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.DistributorId,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.Genres,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.PlatformTypes,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.Comments,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.Description,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.DateOfAdding,
                    cfg => cfg.Ignore());

            CreateMap<ProductMongoEntity, GameEntity>()
                .ForMember(domain => domain.Name,
                    cfg => cfg.MapFrom(entity => entity.ProductName))
                .ForMember(domain => domain.Key,
                    cfg => cfg.MapFrom(entity => entity.ProductKey))
                .ForMember(domain => domain.Discontinued,
                    cfg => cfg.MapFrom(entity => entity.Discontinued))
                .ForMember(domain => domain.Price,
                    cfg => cfg.MapFrom(entity => entity.UnitPrice))
                .ForMember(domain => domain.ViewCount,
                    cfg => cfg.MapFrom(entity => entity.ViewCount))
                .ForMember(domain => domain.QuantityPerUnit,
                    cfg => cfg.MapFrom(entity => entity.QuantityPerUnit))
                .ForMember(domain => domain.ReorderLevel,
                    cfg => cfg.MapFrom(entity => entity.ReorderLevel))
                .ForMember(domain => domain.UnitsOnOrder,
                    cfg => cfg.MapFrom(entity => entity.UnitsInOrder))
                .ForMember(domain => domain.Id,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.Publisher,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.GameGenres,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.Localizations,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.PublisherId,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.DateOfDelete,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.IsDeleted,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.GamesPlatformTypes,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.DateOfPublishing,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.Comments,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.Description,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.DateOfAdding,
                    cfg => cfg.Ignore());

            CreateMap<ProductMongoEntity, GoodsProductMapping>()
                .ForMember(domain => domain.ProductId,
                    cfg => cfg.MapFrom(entity => entity.ProductId))
                .ForMember(domain => domain.GameKey,
                    cfg => cfg.MapFrom(entity => entity.ProductKey))
                .ForMember(domain => domain.Id,
                    cfg => cfg.Ignore())
                .ForMember(domain => domain.GameId,
                    cfg => cfg.Ignore());

            CreateMap<Goods, ProductMongoEntity>()
                .ForMember(entity => entity.SupplierId,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.CategoryId,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.ProductId,
                    cfg => cfg.Ignore())
                .ForMember(entity => entity.ProductName,
                    cfg => cfg.MapFrom(domain => domain.Name))
                .ForMember(entity => entity.UnitsInStock,
                    cfg => cfg.MapFrom(domain => domain.UnitsInStock))
                .ForMember(entity => entity.UnitPrice,
                    cfg => cfg.MapFrom(domain => domain.Price))
                .ForMember(entity => entity.ProductKey,
                    cfg => cfg.MapFrom(domain => domain.Key))
                .ForMember(entity => entity.Discontinued,
                    cfg => cfg.MapFrom(domain => domain.Discontinued));

            #endregion

            #region ShipperMongoEntity

            CreateMap<ShipperMongoEntity, Shipper>()
                .ReverseMap();

            #endregion

            #region GoodsProductMapping

            CreateMap<GoodsProductMappingEntity, GoodsProductMapping>()
                .ReverseMap();

            #endregion

            #region GenreCategoryMapping

            CreateMap<GenreCategoryMappingEntity, GenreCategoryMapping>()
                .ReverseMap();

            #endregion

            #region PublisherSupplierMapping

            CreateMap<PublisherSupplierMappingEntity, PublisherSupplierMapping>()
                .ReverseMap();

            #endregion

            #region Localization

            CreateMap<LocalizationEntity, Localization>()
                .ReverseMap();

            #endregion

            #region EntitiesLocalizations

            CreateMap<GameLocalizationEntity, GoodsLocalization>()
                .ForMember(x => x.GoodsId, 
                    cfg => cfg.MapFrom(y => y.GameId))
                .ForMember(x => x.Goods,
                    cfg => cfg.MapFrom(y => y.Game))
                .ReverseMap();

            CreateMap<GenreLocalizationEntity, GenreLocalization>()
                .ReverseMap();

            CreateMap<PlatformTypeLocalizaitionEntity, PlatformTypeLocalization>()
                .ReverseMap();

            #endregion

            #region SearchRequest

            CreateMap<GoodsSearchRequest, ProductSearchRequest>()
                .ForMember(p => p.Categories,
                    cfg => cfg.MapFrom(x => x.Genres))
                .ForMember(p => p.Suppliers,
                    cfg => cfg.MapFrom(x => x.Distributors))
                .ForMember(p => p.ExcludedProductKeys,
                    cfg => cfg.Ignore());

            CreateMap<GoodsSearchRequest, GamesSearchRequest>()
                .ForMember(p => p.Publishers,
                    cfg => cfg.MapFrom(x => x.Distributors));

            #endregion
        }
    }

}
