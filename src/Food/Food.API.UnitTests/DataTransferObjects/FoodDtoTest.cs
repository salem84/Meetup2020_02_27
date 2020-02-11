using Albedo;
using AutoFixture.Idioms;
using BaseUnitTests;
using SampleWebApiAspNetCore.Dtos;
using Xunit;

namespace Menu.API.UnitTests.DataTransferObjects
{
    public class FoodDtoTest
    {
        [Theory, AutoDomainData]
        public void Test_Getter_And_Setters_FoodDto(WritablePropertyAssertion assertion)
        {
             assertion.Verify(new Properties<FoodDto>().Select(c => c.Id));
             assertion.Verify(new Properties<FoodDto>().Select(c => c.Name));
             assertion.Verify(new Properties<FoodDto>().Select(c => c.Calories));
             assertion.Verify(new Properties<FoodDto>().Select(c => c.Type));
             assertion.Verify(new Properties<FoodDto>().Select(c => c.Created));
        }
    }
}