using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;

namespace Wingineers.Controllers;

public class OrdersController : Controller
{
    private readonly HttpClient _httpClient;

    public OrdersController(){
        _httpClient = new HttpClient();
    }
    
    public async Task<IEnumerable<Order>> GetOrders(){
        string baseUrl = "https://azurecandidatetestapi.azurewebsites.net/api/v1";
        string endpointPath = "/Orderlines"; 
        string apiUrl = $"{baseUrl}{endpointPath}";

        string apiKey = "test1234";
        _httpClient.DefaultRequestHeaders.Add("ApiKey", apiKey);

        HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

         if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            // Process the response content and pass data to a list
            IEnumerable<Order> lst = new List<Order>();
            lst =  JsonConvert.DeserializeObject<IEnumerable<Order>>(responseContent);
            return lst;
        }
        else
        {
            // Handle error cases
            return null;
        }
    }

    public async Task<ActionResult> Orders()
    {
        var lst = await GetOrders();

        string[] formats = { "dd-MM-yyyy HH:mm", "yyyy-MM-dd HH:mm" };

        IDictionary<string, int> orders = new Dictionary<string, int>();

        for (int i = 43; i >= 0; i--) 
        {
            DateTime dateTime = DateTime.Now.AddMonths(-i);
            string shortDate = dateTime.Month.ToString("d2") + "/" + dateTime.Year.ToString();
            orders.Add(shortDate,0);
        }

        foreach (Order order in lst){
            DateTime dateTime;
            if (DateTime.TryParseExact(order.OrderDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime)){
                string shortDate = dateTime.Month.ToString("d2") + "/" + dateTime.Year.ToString();
                    orders[shortDate] = orders[shortDate] + 1;
            }else{
                Console.WriteLine("Unable to parse DateTime");
            }
        }
        return View("OrdersView",orders);
    }  
}
