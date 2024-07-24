
using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiTagController : ControllerBase
    {
        private readonly IApiTagService _apiTagService;
        private readonly ISafeExecutor _safeExecutor;

        public ApiTagController(IApiTagService apiTagService, ISafeExecutor safeExecutor)
        {
            _apiTagService = apiTagService;
            _safeExecutor = safeExecutor;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateApiTag([FromBody] Request<CreateApiTagDto> request)
        {
            return await _safeExecutor.ExecuteAsync(async () => 
                Ok(await _apiTagService.CreateApiTag(request.Payload)));
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetApiTag([FromBody] Request<ApiTagRequestDto> request)
        {
            return await _safeExecutor.ExecuteAsync(async () => 
                Ok(await _apiTagService.GetApiTag(request.Payload)));
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateApiTag([FromBody] Request<UpdateApiTagDto> request)
        {
            return await _safeExecutor.ExecuteAsync(async () => 
                Ok(await _apiTagService.UpdateApiTag(request.Payload)));
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteApiTag([FromBody] Request<DeleteApiTagDto> request)
        {
            return await _safeExecutor.ExecuteAsync(async () => 
                Ok(await _apiTagService.DeleteApiTag(request.Payload)));
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListApiTag([FromBody] Request<ListApiTagRequestDto> request)
        {
            return await _safeExecutor.ExecuteAsync(async () => 
                Ok(await _apiTagService.GetListApiTag(request.Payload)));
        }
    }
}
