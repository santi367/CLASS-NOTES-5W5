using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCEtWebAPI.Data;
using Models.Models;
using WebAPI.DTOs;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TestDataController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TestDataController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateData(CreateTestDataDTO createTestDataDTO)
        {
            TestData testData = new TestData()
            {
                Name = createTestDataDTO.Name
            };

            _context.Add(testData);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
