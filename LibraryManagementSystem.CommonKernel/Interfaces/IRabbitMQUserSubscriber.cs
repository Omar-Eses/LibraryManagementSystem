using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.CommonKernel.Interfaces;

public interface IRabbitMQUserSubscriber<T>
{
    Task ConsumeMessageFromQueueAsync();
    Task ProcessMessageAsync(string message);
}
