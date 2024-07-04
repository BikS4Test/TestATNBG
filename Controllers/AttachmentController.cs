
using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttachmentController : ControllerBase
    {
        private readonly IAttachmentService _attachmentService;

        public AttachmentController(IAttachmentService attachmentService)
        {
            _attachmentService = attachmentService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAttachment([FromBody] CreateAttachmentDto createAttachmentDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _attachmentService.CreateAttachment(createAttachmentDto);
                return Ok(new Response<string>(result));
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetAttachment([FromBody] AttachmentRequestDto attachmentRequestDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _attachmentService.GetAttachment(attachmentRequestDto);
                return Ok(new Response<Attachment>(result));
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateAttachment([FromBody] UpdateAttachmentDto updateAttachmentDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _attachmentService.UpdateAttachment(updateAttachmentDto);
                return Ok(new Response<string>(result));
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteAttachment([FromBody] DeleteAttachmentDto deleteAttachmentDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _attachmentService.DeleteAttachment(deleteAttachmentDto);
                return Ok(new Response<bool>(result));
            });
        }

        [HttpPost("getList")]
        public async Task<IActionResult> GetListAttachment([FromBody] ListAttachmentRequestDto listAttachmentRequestDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _attachmentService.GetListAttachment(listAttachmentRequestDto);
                return Ok(new Response<List<Attachment>>(result));
            });
        }
    }
}
