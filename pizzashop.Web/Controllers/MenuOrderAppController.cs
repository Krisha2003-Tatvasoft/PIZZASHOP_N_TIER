using System.Threading.Tasks;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using pizzashop.Entity.ViewModels;
using pizzashop.Service.Interfaces;
using pizzashop.Service.Utils;
using pizzashop.web.Attributes;
using pizzashop.Web.Hubs;
using QRCoder;

namespace pizzashop.Web.Controllers;

public class MenuOrderAppController : Controller
{
    private readonly ICategoryService _categoryService;

    private readonly IItemService _itemService;

    private readonly IOrderAppMenuService _orderAppMenuService;

    private readonly IOrderService _orderService;

    private readonly IJwtService _jwtService;

    private readonly IHubContext<ChatHub> _hubContext;


    public MenuOrderAppController(ICategoryService categoryService, IItemService itemService,
     IOrderAppMenuService orderAppMenuService, IOrderService orderService, IJwtService jwtService, IHubContext<ChatHub> hubContext)
    {
        _categoryService = categoryService;
        _itemService = itemService;
        _orderAppMenuService = orderAppMenuService;
        _orderService = orderService;
        _jwtService = jwtService;
        _hubContext = hubContext;
    }

    [HttpGet]
    [CustomAuthorize("", "", new string[] { "Account Manager", "Customer" })]
    public IActionResult MenuOrders()
    {
        
        string? token = Request.Cookies["CustomerToken"];
        if (string.IsNullOrEmpty(token))
        {
            return View();
        }
        else
        {
            var customerToken = _jwtService.ValidateToken(token);
            var encodeId = customerToken.Claims.FirstOrDefault(c => c.Type == "encodeId")?.Value;
            var orderIdparam = HttpContext.Request.Query["orderId"];
            if (encodeId == null || encodeId != orderIdparam)
            {
                return RedirectToAction("Error", "Error", new { statusCode = 404 });
            }
            return View();
        }

    }

    [HttpGet]
    [CustomAuthorize("", "", new string[] { "Account Manager", "Customer" })]
    public async Task<IActionResult> CategoryList()
    {
        return PartialView("_CategoryList", await _categoryService.GetMenuCategoryList());
    }


    [HttpGet]
    [CustomAuthorize("", "", new string[] { "Account Manager", "Customer" })]
    public async Task<IActionResult> ItemList(int id, string search = "")
    {
        var itemlist = await _itemService.GetMenuItem(id, search);

        return PartialView("_ItemList", itemlist);
    }

    [HttpPost]
    [CustomAuthorize("", "", new string[] { "Account Manager" })]
    public async Task<IActionResult> ToggleFavourite(int id)
    {
        // CookieData user = SessionUtils.GetUser(HttpContext);
        bool isFavourite = await _itemService.ToggleFavourite(id);
        // Call the SignalR hub to send a message
        await _hubContext.Clients.All.SendAsync("FavouriteUpdated");
        return Json(new { isFavourite = isFavourite });

    }

    [HttpGet]
    [CustomAuthorize("", "", new string[] { "Account Manager", "Customer" })]
    public async Task<IActionResult> ModifierList(int id)
    {
        List<IMGMviewmodel> modifierslist = await _orderAppMenuService.ModifiersById(id);
        if (!modifierslist.Any())
        {
            return Json(new { success = false, message = "No modifiers For this Item." });
        }
        else
        {
            return PartialView("_ModifierList", modifierslist);
        }
    }

    [HttpGet]
    [CustomAuthorize("", "", new string[] { "Account Manager", "Customer" })]
    public async Task<IActionResult> OrderDetails(int id)
    {
        var orderDetails = await _orderAppMenuService.OrderDetails(id); // Fetch order details by ID

        return PartialView("_OrderDetails", orderDetails);
    }

    [HttpPost]
    [CustomAuthorize("", "", new string[] { "Account Manager", "Customer" })]
    public async Task<IActionResult> SaveOrder([FromBody] Bill model)
    {
        if (model == null || model.Items == null)
            return BadRequest("Invalid data");

        var (success, message) = await _orderAppMenuService.SaveOrder(model);
        if (success == true)
        {
            // Call the SignalR hub to send a message
            await _hubContext.Clients.All.SendAsync("OrderUpdated");
        }

        return Ok(new { success = success, message = message });
    }

    [HttpGet]
    [CustomAuthorize("", "", new string[] { "Account Manager", "Customer" })]
    public async Task<IActionResult> OrderComment(int id)
    {
        var orderComment = await _orderAppMenuService.GetOrderComment(id);
        return Json(new { success = true, message = orderComment });
    }

    [HttpPost]
    [CustomAuthorize("", "", new string[] { "Account Manager", "Customer" })]
    public async Task<IActionResult> AddOrderComment(string comment, int orderid)
    {
        bool success = await _orderAppMenuService.AddOrderComment(comment, orderid);
        if (success == true)
        {
            return Json(new { success = true, message = "Order Comment Added Sucessfully." });
        }
        else
        {
            return Json(new { success = false, message = "Error in Add Comment." });
        }
    }

    [HttpPost]
    [CustomAuthorize("", "", new string[] { "Account Manager" })]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var (success, message) = await _orderAppMenuService.CancelOrder(id);
        if (success == true)
        {
            // Call the SignalR hub to send a message
            await _hubContext.Clients.All.SendAsync("OrderUpdated");
        }


        return Ok(new { success = success, message = message });
    }

    [HttpPost]
    [CustomAuthorize("", "", new string[] { "Account Manager" })]
    public async Task<IActionResult> CompleteOrder(int id)
    {
        var (success, message) = await _orderAppMenuService.CompleteOrder(id);
        if (success == true)
        {
            // Call the SignalR hub to send a message
            await _hubContext.Clients.All.SendAsync("OrderUpdated");
        }


        return Ok(new { success = success, message = message });
    }

    [HttpGet]
    [CustomAuthorize("", "", new string[] { "Account Manager" })]
    public async Task<IActionResult> CustomerDetail(int id)
    {
        return PartialView("_CustomerDetails", await _orderAppMenuService.CustomerDetail(id));
    }

    [HttpPost]
    [CustomAuthorize("", "", new string[] { "Account Manager" })]
    public async Task<IActionResult> CustomerDetailPost(CustomerDetail model)
    {
        if (ModelState.IsValid)
        {
            var (success, message) = await _orderAppMenuService.EditCustomerdetail(model);
            if (success == true)
            {
                return Json(new { success = true, message = message });
            }
            else
            {
                return Json(new { success = false, message = message });
            }
        }
        else
        {
            // If model is invalid, return the same view with validation messages
            return Json(new { message = "Validation Error." });
            // return PartialView("_AddTable", model);
        }
    }

    [HttpPost]
    [CustomAuthorize("", "", new string[] { "Account Manager" })]
    public async Task<IActionResult> Review([FromBody] Review model)
    {
        if (model == null)
            return BadRequest("Invalid data");

        var (success, message) = await _orderAppMenuService.ReviewPost(model);

        return Json(new { success = success, message = message });
    }

    [HttpGet]
    [CustomAuthorize("", "", new string[] { "Account Manager" })]
    public async Task<IActionResult> GenerateQRCode(int orderId, string encodeId)
    {
        Console.WriteLine("Order ID: " + encodeId);
        var token = _jwtService.GenerateCustomerToken(orderId, encodeId);
        var qrUrl = Url.Action("GetMenu", "MenuOrderApp", new { orderId = encodeId, token = token }, Request.Scheme);

        using (var qrGenerator = new QRCodeGenerator())
        {
            var qrCodeData = qrGenerator.CreateQrCode(qrUrl, QRCodeGenerator.ECCLevel.Q);

            var pngQrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeImage = pngQrCode.GetGraphic(20);

            return File(qrCodeImage, "image/png");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetMenu(string orderId, string token)
    {
        var tokens = _jwtService.ValidateToken(token);
        if (tokens != null)
        {
            var orderIdToken = tokens.Claims.FirstOrDefault(c => c.Type == "orderId")?.Value;
            CookieUtils.ClearCookies(HttpContext);
            HttpContext.Response.Cookies.Delete("PermissionData");
            SessionUtils.ClearSession(HttpContext);
            Response.Cookies.Append("CustomerToken", token);
            return RedirectToAction("MenuOrders", "MenuOrderApp", new { orderId = orderId });
        }
        else
        {
            return RedirectToAction("Error", "Error", 404);
        }
    }

    [HttpGet]
    [CustomAuthorize("", "", new string[] { "Account Manager", "Customer" })]
    public async Task<IActionResult> GenerateUPIQRCode(int orderId)
    {
        var orderDetails = await _orderService.OrderDetails(orderId);
        var qrUrl = $"upi://pay?pa=krishavaghasiya1211@oksbi&pn=PIZZASHOP&am={orderDetails.Totalamount}&cu=INR&tn={orderDetails.Invoicenumber}";

        using (var qrGenerator = new QRCodeGenerator())
        {
            var qrCodeData = qrGenerator.CreateQrCode(qrUrl, QRCodeGenerator.ECCLevel.Q);

            var pngQrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeImage = pngQrCode.GetGraphic(20);

            return File(qrCodeImage, "image/png");
        }
    }

    [HttpGet]
    public async Task<IActionResult> checkOrderStatus(int id)
    {
        bool isvalidOrder = await _orderAppMenuService.checkOrderStatus(id); // Fetch order details by ID   
        return Json(new { success = isvalidOrder });
    }



}
