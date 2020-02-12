using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BaseUnitTests;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Food.API.Dtos;
using Food.API.Entities;
using Food.API.Models;
using Food.API.Repositories;
using Food.API.v1.Controllers;
using Xunit;

namespace Food.API.UnitTests.Controllers.v1
{
    public class FoodsControllerTests : BaseAutoMockedTest<FoodsController>
    {
        [Fact]
        public void Get_should_return_foods()
        {
            // Given
            var foods = Enumerable.Repeat(new FoodEntity(), 5);
            var foodDtos = Enumerable.Repeat(new FoodDto(), 5);

            var queryParameters = new QueryParameters();
            GetMock<IFoodRepository>().Setup(x => x.GetAll(queryParameters)).Returns(foods.AsQueryable());
            GetMock<IMapper>().Setup(x => x.Map<FoodDto>(It.IsAny<FoodEntity>())).Returns(foodDtos.First());

            // When
            var actionResult = ClassUnderTest.GetAllFoods(ApiVersion.Default, queryParameters);

            // Then
            var result = actionResult.Result as OkObjectResult;
            result.Should().NotBeNull();
            result.Value.Should().BeAssignableTo<IEnumerable<FoodDto>>();
        }

        [Fact]
        public void Given_food_id_Get_should_return_food()
        {
            // Given
            var food = new FoodEntity();
            var foodDto = new FoodDto();
            var id = 1;

            GetMock<IFoodRepository>().Setup(x => x.GetSingle(id)).Returns(food);
            GetMock<IMapper>().Setup(x => x.Map<FoodDto>(It.IsAny<FoodEntity>())).Returns(foodDto);

            // When
            var actionResult = ClassUnderTest.GetSingleFood(ApiVersion.Default, id);

            // Then
            var result = actionResult.Result as OkObjectResult;
            result.Should().NotBeNull();
            result.Value.Should().BeAssignableTo<FoodDto>();
        }

        [Fact]
        public void Given_not_valid_FoodCreateDto_should_return_bad_request()
        {
            FoodCreateDto foodDto = null;

            var controller = ClassUnderTest;

            // Ensure the controller can add response headers
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var result = controller.AddFood(ApiVersion.Default, foodDto);

            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public void Given_invalid_id_Delete_should_throw()
        {
            var id = -1;
            var foodDto = new FoodDto { Id = id };

            var result = ClassUnderTest.RemoveFood(id);
            result.Should().BeOfType<BadRequestResult>();
        }
    }
}
