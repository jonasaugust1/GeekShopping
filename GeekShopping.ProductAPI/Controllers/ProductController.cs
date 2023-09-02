﻿using GeekShopping.ProductAPI.Data.ValueObjects;
using GeekShopping.ProductAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.ProductAPI.Controllers
{
    [Route("api/vi/[controller]")]
    public class ProductController : Controller
    {
        private IProductRepository _repository;

        public ProductController(IProductRepository repository)
        {
              _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductVO>> FindById(long id)
        {
            ProductVO product = await _repository.FindById(id);

            if(product == null) return NotFound();

            return Ok(product);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductVO>>> FindAll()
        {
            IEnumerable<ProductVO> products = await _repository.FindAll();

            return Ok(products);
        }

        [HttpPost]
        public async Task<ActionResult<ProductVO>> Create(ProductVO productVO)
        {
            if (productVO == null) return BadRequest();

            await _repository.Create(productVO);

            return CreatedAtAction(nameof(Create), productVO);
        }

        [HttpPut]
        public async Task<ActionResult<ProductVO>> Update(ProductVO productVO)
        {
            if (productVO == null) return BadRequest();

            await _repository.Update(productVO);

            return Ok(productVO);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            bool deleted = await _repository.Delete(id);

            if(!deleted) return BadRequest();

            return NoContent();
        }
    }
}
