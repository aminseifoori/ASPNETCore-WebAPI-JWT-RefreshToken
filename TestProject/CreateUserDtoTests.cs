using Shared.Dtos.Users;
using System.ComponentModel.DataAnnotations;

namespace TestProject
{
    public class CreateUserDtoTests
    {
        [Fact]
        public void CreateUserDto_OnlyRequiredPropertiesAreMandatory()
        {
            // Arrange
            var createUserDto = new CreateUserDto();

            // Act
            var validationContext = new ValidationContext(createUserDto, null, null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(createUserDto, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);

            // Assert that only properties with [Required] attribute are mandatory
            Assert.Single(validationResults, vr => vr.MemberNames.Contains(nameof(CreateUserDto.UserName)));
            Assert.Single(validationResults, vr => vr.MemberNames.Contains(nameof(CreateUserDto.Password)));
            Assert.Single(validationResults, vr => vr.MemberNames.Contains(nameof(CreateUserDto.Email)));

            // Assert that FirstName and LastName are not mandatory
            Assert.Empty(validationResults.Where(vr => vr.MemberNames.Contains(nameof(CreateUserDto.FirstName))));
            Assert.Empty(validationResults.Where(vr => vr.MemberNames.Contains(nameof(CreateUserDto.LastName))));
        }

        [Fact]
        public void CreateUserDto_ValidProperties_ValidationSucceeds()
        {
            // Arrange
            var createUserDto = new CreateUserDto
            {
                FirstName = "John",
                LastName = "Doe",
                UserName = "johndoe",
                Password = "Password123",
                Email = "john.doe@example.com"
            };

            // Act
            var validationContext = new ValidationContext(createUserDto, null, null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(createUserDto, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults); // No validation errors
        }

        [Fact]
        public void CreateUserDto_RequiredPropertiesAreNull_ValidationFails()
        {
            // Arrange
            var createUserDto = new CreateUserDto
            {
                FirstName = "John",
                LastName = "Doe",
                UserName = null,
                Password = null,
                Email = null
            };

            // Act
            var validationContext = new ValidationContext(createUserDto, null, null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(createUserDto, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Equal(3, validationResults.Count); // UserName, Password, and Email should fail validation
        }
    }
}