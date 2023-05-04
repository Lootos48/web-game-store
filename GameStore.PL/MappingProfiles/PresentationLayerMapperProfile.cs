using AutoMapper;
using GameStore.DomainModels.Models;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.DomainModels.Models.EditModels;
using GameStore.DomainModels.Models.Localizations;
using GameStore.PL.DTOs;
using GameStore.PL.DTOs.CreateDTOs;
using GameStore.PL.DTOs.EditDTOs;
using GameStore.PL.DTOs.LocalizationsDTO;
using System;

namespace GameStore.PL.MappingProfiles
{
    public class PresentationLayerMapperProfile : Profile
    {
        public PresentationLayerMapperProfile()
        {
            #region Game Maps

            CreateMap<Guid, Goods>()
                .ForMember(bo => bo.Id,
                    cfg => cfg.MapFrom(guid => guid))
                .ForMember(bo => bo.DateOfPublishing,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.DateOfAdding,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.ViewCount,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Key,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Name,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Description,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Price,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.UnitsInStock,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Discontinued,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Distributor,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Comments,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.PlatformTypes,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Genres,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.QuantityPerUnit,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.UnitsInOrder,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.ReorderLevel,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.DistributorId,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Localizations,
                    cfg => cfg.Ignore());

            CreateMap<Goods, string>()
                .ConvertUsing(bo => bo.Name);

            CreateMap<Goods, GoodsDTO>()
                .ForMember(dto => dto.Distributor,
                    cfg => cfg.MapFrom(bo => bo.Distributor.CompanyName))
                .ReverseMap();

            CreateMap<CreateGoodsRequestDTO, CreateGoodsRequest>()
                .ForMember(bo => bo.DateOfAdding, 
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Distributor,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Id, cfg => 
                    cfg.Ignore());

            CreateMap<EditGoodsRequestDTO, EditGoodsRequest>()
                .ForMember(bo => bo.Key,
                    cfg => cfg.MapFrom(dto => dto.Key));

            CreateMap<Goods, EditGoodsRequestDTO>()
                .ForMember(dto => dto.DistributorId, 
                    cfg => cfg.MapFrom(bo => bo.Distributor.Id))
                .ForMember(bo => bo.ChosedLocalization,
                    cfg => cfg.Ignore());

            #endregion

            #region Comment Maps

            CreateMap<Comment, CommentDTO>()
                .ReverseMap();

            CreateMap<CreateCommentRequestDTO, CreateCommentRequest>();

            CreateMap<Comment, EditCommentRequestDTO>();

            CreateMap<EditCommentRequestDTO, EditCommentRequest>();

            #endregion

            #region Genre Maps

            CreateMap<Genre, GenreDTO>()
                .ReverseMap();

            CreateMap<CreateGenreRequestDTO, CreateGenreRequest>()
                .ForMember(bo => bo.Description,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Picture,
                    cfg => cfg.Ignore())
                .ReverseMap();

            CreateMap<EditGenreRequestDTO, EditGenreRequest>()
                .ReverseMap();

            CreateMap<Genre, EditGenreRequestDTO>()
                .ForMember(dto => dto.ParentId,
                    cfg => cfg.MapFrom(bo => bo.Parent.Id))
                .ForMember(bo => bo.Description,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Picture,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.ChosedLocalization, 
                    cfg => cfg.Ignore());

            CreateMap<Genre, Guid>()
                .ConvertUsing(bo => bo.Id);

            CreateMap<Guid, Genre>()
                .ForMember(bo => bo.Id,
                    cfg => cfg.MapFrom(i => i))
                .ForMember(bo => bo.Name, 
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Parent, 
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.ParentId, 
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.SubGenres, 
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Localizations,
                    cfg => cfg.Ignore());

            #endregion

            #region Order Maps

            CreateMap<Order, OrderDTO>()
                .ReverseMap();

            CreateMap<Guid, Order>()
                .ForMember(bo => bo.Id,
                    cfg => cfg.MapFrom(guid => guid))
                .ForMember(bo => bo.CustomerId,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.OrderDate,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.OrderDetails,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Status,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.ExpirationDateUtc,
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
                .ForMember(bo => bo.ShipAddress,
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
                    cfg => cfg.Ignore());

            CreateMap<Order, EditOrderRequestDTO>()
                .ForMember(dto => dto.OldStatus,
                    cfg => cfg.MapFrom(bo => bo.Status));

            CreateMap<CreateOrderRequestDTO, CreateOrderRequest>();

            CreateMap<EditOrderRequestDTO, EditOrderRequest>();

            CreateMap<Order, EditOrderRequest>()
                .ForMember(dto => dto.OldStatus,
                    cfg => cfg.MapFrom(bo => bo.Status));

            #endregion

            #region OrderDetails Maps

            CreateMap<OrderDetails, OrderDetailsDTO>();

            CreateMap<OrderDetailsDTO, OrderDetails>()
                .ForMember(x => x.ProductId,
                    cfg => cfg.MapFrom(y => y.Product.Id))
                .ForMember(bo => bo.Order,
                    cfg => cfg.Ignore());

            CreateMap<OrderDetails, EditOrderDetailsRequestDTO>();

            CreateMap<EditOrderDetailsRequestDTO, EditOrderDetailsRequest>();

            CreateMap<CreateOrderDetailsRequestDTO, CreateOrderDetailsRequest>();

            CreateMap<OrderDetails, EditOrderDetailsRequest>();

            #endregion

            #region Publisher Maps

            CreateMap<Guid, Distributor>()
                .ForMember(bo => bo.Id,
                    cfg => cfg.MapFrom(guid => guid))
                .ForMember(bo => bo.CompanyName,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Description,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.HomePage,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.DistributedGoods,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.ContactName,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.ContactTitle,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Address,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.City,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Region,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.PostalCode,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Country,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Phone,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Fax,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.UserId,
                    cfg => cfg.Ignore());

            CreateMap<Distributor, string>()
                .ConvertUsing(p => p.CompanyName);

            CreateMap<Distributor, DistributorDTO>();

            CreateMap<Distributor, EditDistributorRequestDTO>();

            CreateMap<CreateDistributorRequestDTO, CreateDistributorRequest>();

            CreateMap<EditDistributorRequestDTO, EditDistributorRequest>();

            #endregion

            #region Platform Type Maps

            CreateMap<Guid, PlatformType>()
                .ForMember(bo => bo.Id,
                    cfg => cfg.MapFrom(i => i))
                .ForMember(bo => bo.Type,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Localizations,
                    cfg => cfg.Ignore());

            CreateMap<PlatformType, Guid>()
                .ConvertUsing(bo => bo.Id);

            CreateMap<string, CreatePlatformTypeRequest>()
                .ForMember(bo => bo.Type,
                    cfg => cfg.MapFrom(str => str));

            CreateMap<PlatformTypeDTO, CreatePlatformTypeRequest>();

            CreateMap<PlatformType, EditPlatformTypeRequestDTO>()
                .ForMember(bo => bo.ChosedLocalization,
                    cfg => cfg.Ignore());

            CreateMap<EditPlatformTypeRequestDTO, EditPlatformRequest>()
                .ReverseMap();

            CreateMap<PlatformType, PlatformTypeDTO>()
                .ReverseMap();

            #endregion

            #region Games Filters

            CreateMap<GoodsSearchRequestDTO, GoodsSearchRequest>()
                .ReverseMap();

            CreateMap<GoodsSearchRequestDTO, GoodsSearchRequestDTO>()
                .ReverseMap();

            #endregion

            #region User Maps

            CreateMap<User, UserDTO>()
                .ForMember(dto => dto.Password,
                    cfg => cfg.Ignore());

            CreateMap<UserDTO, User>();

            CreateMap<CreateUserRequestDTO, User>()
                .ForMember(bo => bo.Id,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Comments,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Orders,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Role,
                    cfg => cfg.Ignore());

            CreateMap<EditUserRequestDTO, User>()
                .ForMember(bo => bo.Password,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Comments,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Orders,
                    cfg => cfg.Ignore())
                .ForMember(bo => bo.Role,
                    cfg => cfg.Ignore())
                .ReverseMap();

            #endregion

            #region Shipper Maps

            CreateMap<Shipper, ShipperDTO>()
                .ReverseMap();

            #endregion

            #region Localization

            CreateMap<Localization, LocalizationDTO>()
                .ReverseMap();

            #endregion

            #region EntitiesLocalizations

            CreateMap<GoodsLocalization, GoodsLocalizationDTO>()
                .ReverseMap();

            CreateMap<GenreLocalization, GenreLocalizationDTO>()
                .ReverseMap();

            CreateMap<PlatformTypeLocalization, PlatformTypeLocalizationDTO>()
                .ReverseMap();

            #endregion
        }
    }

}
