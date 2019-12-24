using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using code.Models;
using Microsoft.AspNetCore.Mvc;

namespace code.Controllers 
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private ConcurrentDictionary<int, Customer> _store = new ConcurrentDictionary<int, Customer>();
        
        public CustomersController(){
            var customerOne = new Customer()
            {
                Id = 1,
                Name = "Phil",
                Age = 60
            };
            var customerTwo = new Customer()
            {
                Id = 2,
                Name = "Mike",
                Age = 61
            };

            _store.TryAdd(customerOne.Id, customerOne);
            _store.TryAdd(customerTwo.Id, customerTwo);
        }

        // GET api/values
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_store.Values);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult GetSingle(int id)
        {
            _store.TryGetValue(id, out Customer existingCustomer);

            if(existingCustomer == null)
            {
                return NotFound();
            }

            return Ok(existingCustomer);
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]Customer customerToAdd)
        {
            if(customerToAdd == null)
            {
                return BadRequest();
            }

            customerToAdd.Id = _store.Values.Max(i => i.Id) + 1;

            var result = _store.TryAdd(customerToAdd.Id, customerToAdd);

            if(!result)
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(GetSingle), new { id = customerToAdd.Id}, customerToAdd);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Customer customerToUpdate)
        {
            _store.TryGetValue(id, out Customer existingCustomer);

            if(existingCustomer == null)
            {
                return NotFound();
            }

            if(customerToUpdate == null)
            {
                return BadRequest();
            }

            customerToUpdate.Id = id;

            var result = _store.TryUpdate(id, customerToUpdate, existingCustomer);

            if(!result)
            {
                return BadRequest();
            }

            return Ok(customerToUpdate);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
             _store.TryGetValue(id, out Customer existingCustomer);

            if(existingCustomer == null)
            {
                return NotFound();
            }

            var result = _store.TryRemove(id, out Customer deletedCustomer);

            if(!result)
            {
                return BadRequest();
            }

            return NoContent();
        }
    }
}