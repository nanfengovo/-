﻿using BCVP.NET8.Model;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace BCVP.NET8.Repository.Base
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, new()
    {
        public async Task<List<TEntity>> Query()
        {

            await Task.CompletedTask;
            var data = "[{\"Id\":18,\"Name\":\"namenamename\"}]";
            return JsonConvert.DeserializeObject<List<TEntity>>(data) ?? new List<TEntity>();
        }
    }
}
