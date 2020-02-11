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
using SampleWebApiAspNetCore.Dtos;
using SampleWebApiAspNetCore.Entities;
using SampleWebApiAspNetCore.Models;
using SampleWebApiAspNetCore.Repositories;
using SampleWebApiAspNetCore.v1.Controllers;
using Xunit;

namespace Menu.API.UnitTests.Controllers
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
            GetMock<IMapper>().Setup(x => x.Map<IEnumerable<FoodDto>>(It.IsAny<List<FoodEntity>>())).Returns(foodDtos);

            var controller = ClassUnderTest;

            // Ensure the controller can add response headers
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // When
            var result = controller.GetAllFoods(ApiVersion.Default, queryParameters);

            // Then
            result.Should().BeOfType<OkObjectResult>();
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

            var controller = ClassUnderTest;

            // Ensure the controller can add response headers
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // When
            var result = controller.GetSingleFood(ApiVersion.Default, id);

            // Then
            //result.Should().Be(foodDto);
            result.Should().BeOfType<OkObjectResult>();
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
            //GetMock<IMapper>().Setup(x => x.Map<FoodEntity>(foodDto)).Throws<Exception>();

            var result = ClassUnderTest.RemoveFood(id);
            result.Should().BeOfType<BadRequestResult>();
        }
    }
}
