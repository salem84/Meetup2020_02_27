using System;
using System.Linq;
using AutoMapper;
using Food.API.Dtos;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Food.API.Repositories;
using System.Collections.Generic;
using Food.API.Entities;
using Food.API.Models;
using Food.API.Helpers;
using Microsoft.Extensions.Configuration;

namespace Food.API.v1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class FoodsController : ControllerBase
    {
        private readonly IFoodRepository _foodRepository;
        private readonly IUrlHelper _urlHelper;
        private readonly IMapper _mapper;
        private readonly IConfiguration Configuration;

        public FoodsController(
            IUrlHelper urlHelper,
            IFoodRepository foodRepository,
            IMapper mapper,
            IConfiguration configuration)
        {
            _foodRepository = foodRepository;
            _mapper = mapper;
            Configuration = configuration;
            _urlHelper = urlHelper;
        }

        [HttpGet(Name = nameof(GetAllFoods))]
        public ActionResult<IEnumerable<FoodDto>> GetAllFoods(ApiVersion version, [FromQuery] QueryParameters queryParameters)
        {
            List<FoodEntity> foodItems = _foodRepository.GetAll(queryParameters).ToList();

            var toReturn = foodItems.Select(x => ExpandSingleFoodItem(x, version)).ToList();

            return Ok(toReturn);
        }

        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSingleFood))]
        public ActionResult<FoodDto> GetSingleFood(ApiVersion version, int id)
        {
            FoodEntity foodItem = _foodRepository.GetSingle(id);

            if (foodItem == null)
            {
                return NotFound();
            }

            return Ok(ExpandSingleFoodItem(foodItem, version));
        }

        [HttpPost(Name = nameof(AddFood))]
        public ActionResult AddFood(ApiVersion version, [FromBody] FoodCreateDto foodCreateDto)
        {
            if (foodCreateDto == null)
            {
                return BadRequest();
            }

            FoodEntity toAdd = _mapper.Map<FoodEntity>(foodCreateDto);

            _foodRepository.Add(toAdd);

            if (!_foodRepository.Save())
            {
                throw new Exception("Creating a fooditem failed on save.");
            }

            FoodEntity newFoodItem = _foodRepository.GetSingle(toAdd.Id);

            return CreatedAtRoute(nameof(GetSingleFood), new { version = version.ToString(), id = newFoodItem.Id },
                _mapper.Map<FoodDto>(newFoodItem));
        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateFood))]
        public ActionResult<FoodDto> PartiallyUpdateFood(int id, [FromBody] JsonPatchDocument<FoodUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            FoodEntity existingEntity = _foodRepository.GetSingle(id);

            if (existingEntity == null)
            {
                return NotFound();
            }

            FoodUpdateDto foodUpdateDto = _mapper.Map<FoodUpdateDto>(existingEntity);
            patchDoc.ApplyTo(foodUpdateDto);

            TryValidateModel(foodUpdateDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(foodUpdateDto, existingEntity);
            FoodEntity updated = _foodRepository.Update(id, existingEntity);

            if (!_foodRepository.Save())
            {
                throw new Exception("Updating a fooditem failed on save.");
            }

            return _mapper.Map<FoodDto>(updated);
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(RemoveFood))]
        public ActionResult RemoveFood(int id)
        {
            if (id < 1)
            {
                return BadRequest();
            }

            FoodEntity foodItem = _foodRepository.GetSingle(id);

            if (foodItem == null)
            {
                return NotFound();
            }

            _foodRepository.Delete(id);

            if (!_foodRepository.Save())
            {
                throw new Exception("Deleting a fooditem failed on save.");
            }

            return NoContent();
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateFood))]
        public ActionResult<FoodDto> UpdateFood(int id, [FromBody]FoodUpdateDto foodUpdateDto)
        {
            if (foodUpdateDto == null)
            {
                return BadRequest();
            }

            var existingFoodItem = _foodRepository.GetSingle(id);

            if (existingFoodItem == null)
            {
                return NotFound();
            }

            _mapper.Map(foodUpdateDto, existingFoodItem);

            _foodRepository.Update(id, existingFoodItem);

            if (!_foodRepository.Save())
            {
                throw new Exception("Updating a fooditem failed on save.");
            }

            return _mapper.Map<FoodDto>(existingFoodItem);
        }

        [HttpGet("GetRandomMeal", Name = nameof(GetRandomMeal))]
        public ActionResult<IEnumerable<FoodDto>> GetRandomMeal()
        {
            var configRandomRate = Configuration.GetValue<double>("ErrorGenerator:ErrorRate");
            var randomValue = new Random().NextDouble();

            if(randomValue > configRandomRate)
            {
                ICollection<FoodEntity> foodItems = _foodRepository.GetRandomMeal();

                var dtos = foodItems.Select(x => _mapper.Map<FoodDto>(x)).ToList();

                return dtos;
            }
            else
            {
                return StatusCode(503);
            }
            
        }
        private FoodDto ExpandSingleFoodItem(FoodEntity foodItem, ApiVersion version)
        {
            FoodDto item = _mapper.Map<FoodDto>(foodItem);

            return item;
        }
    }
}
