using BookingSystemApi.Authentication;
using BookingSystemApi.Interface;
using BookingSystemApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingSystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly ITableRepo _tableRepo;
        public TableController(ITableRepo tableRepo)
        {
            _tableRepo = tableRepo;
        }

        [EnableCors("AnotherPolicy")]
        [HttpGet("{id}")]
        public async Task<ActionResult<List<Table>>> GetAllTableByRestaurantId(int id)
        {
            List<Table> tableList = await _tableRepo.GetAllTablesByRestaurantId(id);
            if(tableList == null)
            {
                return NotFound();
            }
            return Ok(tableList);
        }

        [HttpGet]
        [Route("GetTableByResId")]
        public async Task<ActionResult<List<Table>>> GetTableById([FromQuery]int id)
        {
            List<Table> tableList= await _tableRepo.GetTableByResId(id);
            
            return Ok(tableList);

        }

        [HttpGet]
        [Route("GetQtyTableByResId")]
        public async Task<ActionResult<List<Table>>> GetQtyTableById([FromQuery] int id)
        {
            List<Table> tableList = await _tableRepo.GetQtyTableByResId(id);

            return Ok(tableList);

        }

        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public async Task<ActionResult<List<Table>>> GetTableWithDateTime([FromForm] BookDateWithResId bookDateWithResId)
        {
            List<Table> tableList = await _tableRepo.GetTableWithDateTime(bookDateWithResId);
            if (tableList == null)
            {
                return NotFound();
            }
            return Ok(tableList);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut]
        public async Task<ActionResult> UpdateTable([FromBody] List<Table> tableList)
        {
            await _tableRepo.UpdateTable(tableList);
            return Ok();
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [Route("DeleteTable")]
        public async Task<ActionResult> DeleteTable([FromForm] int tableId)
        {
            await _tableRepo.DeleteTable(tableId);
            return Ok();
        }
    }
}
