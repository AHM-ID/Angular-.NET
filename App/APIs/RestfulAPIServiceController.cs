using Angular.App.Models;
using Angular.App.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Angular.App.APIs
{
    [ApiController]
    [Route("api/rest")]
    public class RestfulAPIServiceController : ControllerBase
    {
        private readonly IBrowserCheckService _browserCheckService;

        public RestfulAPIServiceController(IBrowserCheckService browserCheckService)
        {
            _browserCheckService = browserCheckService;
        }

        [HttpGet]
        public async Task<IActionResult> GetData1Async()
        {
            return await Task.FromResult(
                new JsonResult(
                    new
                    {
                        success = true,
                        message = "Request successful",
                        data = "Sample Basic Content from getData1",
                    }
                )
            );
        }

        [HttpGet("data2")]
        public async Task<IActionResult> GetData2Async()
        {
            return await Task.FromResult(
                new JsonResult(
                    new
                    {
                        success = true,
                        message = "Request successful",
                        data = "Sample Basic Content from getData2",
                    }
                )
            );
        }

        [HttpGet("data3/1")]
        public async Task<IActionResult> GetData3Async()
        {
            return await Task.FromResult(
                new JsonResult(
                    new
                    {
                        success = true,
                        message = "Request successful",
                        data = "Sample Basic Content from getData3",
                    }
                )
            );
        }

        [HttpGet("data4/{id}")]
        public async Task<IActionResult> GetData4Async([FromRoute] int id)
        {
            return await Task.FromResult(
                new JsonResult(
                    new
                    {
                        success = true,
                        message = "Request successful",
                        data = new { id },
                    }
                )
            );
        }

        [HttpGet("data5")]
        public async Task<IActionResult> GetData5Async(
            [FromQuery] string str1,
            [FromQuery] string str2
        )
        {
            return await Task.FromResult(
                new JsonResult(
                    new
                    {
                        success = true,
                        message = "Request successful",
                        data = new { combined = $"{str1} + {str2}" },
                    }
                )
            );
        }

        [HttpGet("data6/{str1}/{str2}")]
        public async Task<IActionResult> GetData6Async(
            [FromRoute] string str1,
            [FromRoute] string str2
        )
        {
            return await Task.FromResult(
                new JsonResult(
                    new
                    {
                        success = true,
                        message = "Request successful",
                        data = new { combined = $"{str1} + {str2}" },
                    }
                )
            );
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetStatusAsync()
        {
            return await Task.FromResult(StatusCode(250, "Sample Title"));
        }

        [HttpGet("products/authorized")]
        public async Task<IActionResult> GetProductsAsync()
        {
            var products = new List<Product>()
            {
                new Product() { productId = 1, productName = "p1" },
                new Product() { productId = 2, productName = "p2" },
                new Product() { productId = 3, productName = "p3" },
            };

            return await Task.FromResult(Ok(products));
        }

        [HttpGet("products/unauthorized")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsAsync(
            [FromHeader] string token
        )
        {
            //? Token -> Claims -> UserName -> Authorize -> Unauthorized
            //? DataBase Check
            var products = new List<Product>()
            {
                new Product() { productId = 1, productName = "p1" },
                new Product() { productId = 2, productName = "p2" },
                new Product() { productId = 3, productName = "p3" },
            };

            return await Task.FromResult(Unauthorized("User unauthorized to get products list"));
        }

        [HttpGet("service")]
        public async Task<IActionResult> CheckBrowserServiceAsync(
            [FromServices] IBrowserCheckService browserCheckService
        )
        {
            var userAgent = Request.Headers["User-Agent"].ToString();
            var isValidBrowser = await browserCheckService.ValidateBrowser(userAgent);

            return new JsonResult(
                new
                {
                    success = true,
                    message = "Request successful",
                    data = new { result = $"Valid Browser: {isValidBrowser}" },
                }
            );
        }

        [HttpGet("service/constructor")]
        public async Task<IActionResult> CheckBrowserServiceAsync()
        {
            var userAgent = Request.Headers["User-Agent"].ToString();
            var isValidBrowser = await _browserCheckService.ValidateBrowser(userAgent);

            return new JsonResult(
                new
                {
                    success = true,
                    message = "Request successful",
                    data = new { result = $"Valid Browser: {isValidBrowser}" },
                }
            );
        }

        [HttpPost("profile")]
        public async Task<IActionResult> CreateProfileAsync(
            [FromForm] string firstName,
            [FromForm] string lastName
        )
        {
            return await Task.FromResult(
                new JsonResult(
                    new
                    {
                        success = true,
                        message = "Request successful",
                        data = new { combined = $"Name: {firstName} {lastName}" },
                    }
                )
            );
        }

        [HttpPost("product")]
        public async Task<IActionResult> GetProductsAsync([FromBody] Product product)
        {
            if (product == null)
            {
                return await Task.FromResult(
                    BadRequest(new { success = false, message = "Invalid product data" })
                );
            }

            return await Task.FromResult(
                new JsonResult(
                    new
                    {
                        success = true,
                        message = "Request successful",
                        data = new { product = $"{product.productId}, {product.productName}" },
                    }
                )
            );
        }

        [HttpPost("customer")]
        public async Task<IActionResult> GetCustomerAsync([FromHeader] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return await Task.FromResult(
                    BadRequest(new { success = false, message = "Token is required" })
                );
            }

            return await Task.FromResult(
                new JsonResult(
                    new
                    {
                        success = true,
                        message = "Request successful",
                        data = new { token },
                    }
                )
            );
        }
    }
}
