using System.Diagnostics;
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
        private readonly ILogger<RestfulAPIServiceController> _logger;

        public RestfulAPIServiceController(
            IBrowserCheckService browserCheckService,
            ILogger<RestfulAPIServiceController> logger
        )
        {
            _browserCheckService = browserCheckService;
            _logger = logger;
        }

        //? Unified API Response Wrapper
        private static JsonResult BuildResponse<T>(
            bool success,
            string message,
            T? data,
            object? error = null,
            long? elapsedMilliseconds = null
        )
        {
            return new JsonResult(
                new ApiResponse<T>
                {
                    Status = success ? "success" : "error",
                    Message = message,
                    Data = data,
                    Error = error,
                    Metadata = new
                    {
                        Timestamp = DateTime.UtcNow,
                        ApiVersion = "1.0",
                        RequestId = Guid.NewGuid().ToString(),
                        ResponseTimeMs = elapsedMilliseconds,
                    },
                }
            );
        }

        private IActionResult WithTiming(string actionName, Func<long, IActionResult> action)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("{Action} started", actionName);
            var result = action(stopwatch.ElapsedMilliseconds);
            stopwatch.Stop();
            _logger.LogInformation(
                "{Action} completed in {Time} ms",
                actionName,
                stopwatch.ElapsedMilliseconds
            );
            return result;
        }

        //========================
        //* Basic Data Endpoints
        //========================

        //! GET: api/rest/data/basic-1
        [HttpGet("data")]
        public IActionResult FetchBasicData1() =>
            WithTiming(
                "FetchBasicData1",
                elapsed =>
                    BuildResponse(
                        true,
                        "Request successful",
                        new { content = "Sample Basic Content from getData1" },
                        null,
                        elapsed
                    )
            );

        //! GET: api/rest/data/basic-2
        [HttpGet("data/basic-2")]
        public IActionResult FetchBasicData2() =>
            WithTiming(
                "FetchBasicData2",
                elapsed =>
                    BuildResponse(
                        true,
                        "Request successful",
                        new { content = "Sample Basic Content from getData2" },
                        null,
                        elapsed
                    )
            );

        //! GET: api/rest/data/static/1
        [HttpGet("data/static/1")]
        public IActionResult FetchStaticData3() =>
            WithTiming(
                "FetchStaticData3",
                elapsed =>
                    BuildResponse(
                        true,
                        "Request successful",
                        new { content = "Sample Basic Content from getData3" },
                        null,
                        elapsed
                    )
            );

        //! GET: api/rest/data/id/{id}
        [HttpGet("data/id/{id}")]
        public IActionResult FetchDataById([FromRoute] int id) =>
            WithTiming(
                "FetchDataById",
                elapsed => BuildResponse(true, "Request successful", new { id }, null, elapsed)
            );

        //! GET: api/rest/data/query?str1=x&str2=y
        [HttpGet("data/query")]
        public IActionResult FetchCombinedQuery([FromQuery] string str1, [FromQuery] string str2) =>
            WithTiming(
                "FetchCombinedQuery",
                elapsed =>
                    BuildResponse(
                        true,
                        "Request successful",
                        new { combined = $"{str1} + {str2}" },
                        null,
                        elapsed
                    )
            );

        //! GET: api/rest/data/route/{str1}/{str2}
        [HttpGet("data/route/{str1}/{str2}")]
        public IActionResult FetchCombinedRoute([FromRoute] string str1, [FromRoute] string str2) =>
            WithTiming(
                "FetchCombinedRoute",
                elapsed =>
                    BuildResponse(
                        true,
                        "Request successful",
                        new { combined = $"{str1} + {str2}" },
                        null,
                        elapsed
                    )
            );

        //! GET: api/rest/meta/status-code
        [HttpGet("meta/status-code")]
        public IActionResult FetchCustomStatus()
        {
            _logger.LogInformation("FetchCustomStatus called");
            return StatusCode(250, new { status = "info", message = "Sample Title" });
        }

        //=============================
        //* Product Endpoints
        //=============================

        //! GET: api/rest/products
        [HttpGet("products")]
        public IActionResult FetchAuthorizedProducts() =>
            WithTiming(
                "FetchAuthorizedProducts",
                elapsed =>
                {
                    var products = new List<Product>
                    {
                        new Product { productId = 1, productName = "p1" },
                        new Product { productId = 2, productName = "p2" },
                        new Product { productId = 3, productName = "p3" },
                    };

                    return BuildResponse(
                        true,
                        "Products retrieved successfully",
                        products,
                        null,
                        elapsed
                    );
                }
            );

        //! GET: api/rest/products/unauthorized
        [HttpGet("products/unauthorized")]
        public ActionResult<IEnumerable<Product>> FetchUnauthorizedProducts(
            [FromHeader] string token
        )
        {
            _logger.LogWarning("Unauthorized access attempt with token: {Token}", token);
            //TODO: Implement real token validation & authorization
            return Unauthorized(new { error = "User unauthorized to access the products list" });
        }

        //! POST: api/rest/products/details
        [HttpPost("products/details")]
        public IActionResult ReceiveProductDetails([FromBody] Product product) =>
            WithTiming(
                "ReceiveProductDetails",
                elapsed =>
                {
                    if (product == null)
                    {
                        _logger.LogWarning("Invalid product data received.");
                        return BadRequest(
                            BuildResponse<string>(
                                false,
                                "Invalid product data",
                                null,
                                null,
                                elapsed
                            )
                        );
                    }

                    return BuildResponse(
                        true,
                        "Product details received successfully",
                        new { productInfo = $"{product.productId}, {product.productName}" },
                        null,
                        elapsed
                    );
                }
            );

        //=============================
        //* Browser Validation Service
        //=============================

        //! GET: api/rest/browser/validate
        [HttpGet("browser/validate")]
        public async Task<IActionResult> ValidateBrowserFromServiceAsync(
            [FromServices] IBrowserCheckService browserCheckService
        )
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("ValidateBrowserFromServiceAsync called");

            var userAgent = Request.Headers["User-Agent"].ToString();
            var isValidBrowser = await browserCheckService.ValidateBrowser(userAgent);

            stopwatch.Stop();
            _logger.LogInformation("Browser validated in {Time} ms", stopwatch.ElapsedMilliseconds);

            return BuildResponse(
                true,
                "Browser validation successful",
                new { result = $"Valid Browser: {isValidBrowser}" },
                null,
                stopwatch.ElapsedMilliseconds
            );
        }

        //! GET: api/rest/browser/validate/constructor
        [HttpGet("browser/validate/constructor")]
        public async Task<IActionResult> ValidateBrowserFromConstructorAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("ValidateBrowserFromConstructorAsync called");

            var userAgent = Request.Headers["User-Agent"].ToString();
            var isValidBrowser = await _browserCheckService.ValidateBrowser(userAgent);

            stopwatch.Stop();
            _logger.LogInformation("Browser validated in {Time} ms", stopwatch.ElapsedMilliseconds);

            return BuildResponse(
                true,
                "Browser validation successful",
                new { result = $"Valid Browser: {isValidBrowser}" },
                null,
                stopwatch.ElapsedMilliseconds
            );
        }

        //=============================
        //* User & Token Endpoints
        //=============================

        //! POST: api/rest/users/profile
        [HttpPost("users/profile")]
        public IActionResult CreateUserProfile(
            [FromForm] string firstName,
            [FromForm] string lastName
        ) =>
            WithTiming(
                "CreateUserProfile",
                elapsed =>
                    BuildResponse(
                        true,
                        "User profile created successfully",
                        new { fullName = $"{firstName} {lastName}" },
                        null,
                        elapsed
                    )
            );

        //! POST: api/rest/users/token
        [HttpPost("users/token")]
        public IActionResult ReceiveCustomerToken([FromHeader] string token) =>
            WithTiming(
                "ReceiveCustomerToken",
                elapsed =>
                {
                    if (string.IsNullOrEmpty(token))
                    {
                        _logger.LogWarning("Token missing from header.");
                        return BadRequest(
                            BuildResponse<string>(false, "Token is required", null, null, elapsed)
                        );
                    }

                    return BuildResponse(
                        true,
                        "Token received successfully",
                        new { token },
                        null,
                        elapsed
                    );
                }
            );
    }
}
