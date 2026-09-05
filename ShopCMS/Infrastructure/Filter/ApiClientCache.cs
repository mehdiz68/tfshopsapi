using DataLayer;
using Domain;
using System;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Caching;

public static class ApiClientCache
{
    private static ObjectCache cache = MemoryCache.Default;

    public static ApiClients Get(string clientId)
    {
        string key = "api_client_" + clientId;

        var client = cache[key] as ApiClients;
        if (client != null)
            return client;

        using (var db = new TfShopDbContext())
        {
            client = db.ApiClients.FirstOrDefault(x => x.ClientId == clientId && x.IsActive);

            if (client != null)
                cache.Set(key, client, DateTimeOffset.Now.AddHours(12));
        }

        return client;
    }
}
