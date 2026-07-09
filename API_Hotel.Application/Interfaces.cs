using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Hotel.Application
{    
    public interface IEventPublisher
    {
        void Publish<T>(T @event, string queueName);
    }
}
