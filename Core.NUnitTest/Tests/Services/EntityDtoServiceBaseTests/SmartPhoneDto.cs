using Core.Dto;

namespace Core.NUnitTest.Tests.Services.EntityDtoServiceBaseTests
{
    public class SmartPhoneDto : EntityDto
    {
        public string Model { get; set; }

        public string Make { get; set; }

        public int Year { get; set; }
    }
}
