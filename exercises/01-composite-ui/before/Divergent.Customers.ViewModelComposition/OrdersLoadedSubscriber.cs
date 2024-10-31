﻿using Divergent.Sales.ViewModelComposition.Events;
using ITOps.Json;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;

namespace Divergent.Customers.ViewModelComposition;

public class OrdersLoadedSubscriber : ICompositionEventsSubscriber
{
    [HttpGet("/orders/{id}")]
    public void Subscribe(ICompositionEventsPublisher publisher)
    {
        publisher.Subscribe<OrdersLoaded>(async (ordersLoaded, httpRequest) =>
        {
            var orderIds = string.Join(",", ordersLoaded.OrderViewModelDictionary.Keys);

            // Hardcoded to simplify the exercise. In a production app, a config object could be injected.
            var url = $"http://localhost:20186/api/customers/byorders?orderIds={orderIds}";
            var response = await new HttpClient().GetAsync(url);

            dynamic[] customers = await response.Content.AsExpandoArrayAsync();

            foreach (dynamic customer in customers)
            {
                ordersLoaded.OrderViewModelDictionary[customer.OrderId].OrderCustomerName = customer.CustomerName;
            }
        });
    }
}