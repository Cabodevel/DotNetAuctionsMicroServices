using Contracts.Actions;
using MassTransit;

namespace AuctionService.Consumers
{
    public class AuctionCreatedFaultConsumer : IConsumer<Fault<AuctionCreated>>
    {
        public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
        {
            Console.WriteLine("--> Consuming faulty creation");
            var exception = context.Message.Exceptions[0];

            if(exception.ExceptionType == $"{nameof(System)}.{nameof(ArgumentException)}")
            {
                context.Message.Message.Model = "FooBar";
                await context.Publish(context.Message.Message);
            }
            else
            {
                Console.WriteLine("Not an ArgumentException - update error dashboard somewhere");
            }
        }
    }
}
