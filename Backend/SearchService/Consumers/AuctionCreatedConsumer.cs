using AutoMapper;
using Contracts.Actions;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers
{
    public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
    {
        private readonly IMapper _mapper;

        public AuctionCreatedConsumer(IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<AuctionCreated> context)
        {
            Console.WriteLine("--> Consuming auction created " + context.Message.Id);

            var item = _mapper.Map<Item>(context.Message);

            if(item.Model == "foo")
            {
                throw new ArgumentException();
            }

            await item.SaveAsync();
        }
    }
}
