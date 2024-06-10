using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.Administration;
namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected IEnumerable<T> Data { get; set; }

        public InMemoryRepository(IEnumerable<T> data)
        {
            Data = data;
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(Data);
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public Task Create(T item)
        {
            object locker = new();
            lock (locker)
            {
                var finded = GetByIdAsync(item.Id);
                if (finded.Result != null)
                {
                    throw new Exception("Такой элемент уже существует");
                }
                var newData = Data.ToList();
                newData.Add(item);
                Data = newData;
            }
            return Task.FromResult(Data);
        }

        public Task Update(T item)
        {

            object locker = new();
            lock (locker)
            {
                var tupleIndex = Data.Select((employee, index) => (index, employee)).FirstOrDefault(t => t.employee.Id == item.Id);
                if (tupleIndex.employee == null)
                {
                    throw new Exception("Такого элемента не существует");
                }
                var newData = Data.ToList();
                newData[tupleIndex.index] = item;

                Data = newData;

            }
            return Task.FromResult(Data);
        }

        public Task Delete(Guid id)
        {

            object locker = new();
            lock (locker)
            {
                var newData = Data.Where(t => t.Id != id);
                if (newData.Count() == Data.Count())
                {
                    throw new Exception("Такого элемента не существует");
                }


                Data = newData;

            }
            return Task.FromResult(Data);
        }
    }
}