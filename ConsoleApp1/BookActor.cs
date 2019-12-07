using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Proto;
using Proto.Persistence;

namespace ConsoleApp1
{
    public sealed class BookActor : IActor
    {
        private readonly Persistence _persistence;
        
        private readonly HashSet<string> _borrowedBooks = new HashSet<string>();

        public BookActor(IEventStore eventStore, string actorId)
        {
            _persistence = Persistence.WithEventSourcing(eventStore, actorId, ApplyEvent);
        }
        
        public async Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                    await _persistence.RecoverStateAsync();
                    break;
                case BookBorrowed bookBorrowed:
                    if (!_borrowedBooks.Contains(bookBorrowed.BookName))
                    {
                        await _persistence.PersistEventAsync(context.Message);
                    }
                    break;
                default:
                    Console.WriteLine($"Unknown message received of type: {context.Message.GetType().Name}");
                    break;
            }
        }
        
        private void ApplyEvent(Event @event)
        {
            switch (@event.Data)
            {
                case BookBorrowed bookBorrowed:
                    _borrowedBooks.Add(bookBorrowed.BookName);
                    Console.WriteLine($"Book is borrowed: {bookBorrowed}");
                    break;
            }
        }
    }
}