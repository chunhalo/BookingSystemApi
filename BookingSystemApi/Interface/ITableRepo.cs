using BookingSystemApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingSystemApi.Interface
{
    public interface ITableRepo
    {
        Task<List<Table>> GetAllTablesByRestaurantId(int resId);
        
        Task<List<Table>> GetTableWithDateTime(BookDateWithResId bookDateWithResId);
        Task UpdateTable(List<Table> tableList);
        Task DeleteTable(int tableId);
        Task<List<Table>> GetTableByResId(int resId);
        Task<List<Table>> GetQtyTableByResId(int resId);

    }
}
