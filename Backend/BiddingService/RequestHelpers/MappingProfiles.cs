using AutoMapper;
using BiddingService.DTOs;
using BiddingService.Models;
using Contracts.Actions;

namespace BiddingService.RequestHelpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Bid, BidDto>();
            CreateMap<Bid, BidPlaced>();
            CreateMap<AuctionService.GrpcAuctionModel, Auction>();
        }
    }
}
