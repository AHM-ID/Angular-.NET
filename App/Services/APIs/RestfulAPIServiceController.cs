using Microsoft.AspNetCore.Mvc;

namespace Angular.App.Services.APIs
{
    [ApiController]
    [Route("api/rest")]
    public class RestfulAPIServiceController : ControllerBase
    {
        [HttpGet()]
        public IActionResult GetData1()
        {
            return new JsonResult(
                new
                {
                    success = true,
                    message = "Request successful",
                    data = "Sample Basic Content from getData1",
                }
            );
        }

        [HttpGet("data2")]
        public IActionResult GetData2()
        {
            return new JsonResult(
                new
                {
                    success = true,
                    message = "Request successful",
                    data = "Sample Basic Content from getData2",
                }
            );
        }

        [HttpGet("data3/1")]
        public IActionResult GetData3()
        {
            return new JsonResult(
                new
                {
                    success = true,
                    message = "Request successful",
                    data = "Sample Basic Content from getData3",
                }
            );
        }

        [HttpGet("data4/{id}")]
        public IActionResult GetData4([FromRoute] int id)
        {
            return new JsonResult(
                new
                {
                    success = true,
                    message = "Request successful",
                    data = new { id },
                }
            );
        }

        [HttpGet("data5")]
        public IActionResult GetData5([FromQuery] string str1, [FromQuery] string str2)
        {
            return new JsonResult(
                new
                {
                    success = true,
                    message = "Request successful",
                    data = new { combined = $"{str1} + {str2}" },
                }
            );
        }

        [HttpGet("data6/{str1}/{str2}")]
        public IActionResult GetData6([FromRoute] string str1, [FromRoute] string str2)
        {
            return new JsonResult(
                new
                {
                    success = true,
                    message = "Request successful",
                    data = new { combined = $"{str1} + {str2}" },
                }
            );
        }
    }
}
