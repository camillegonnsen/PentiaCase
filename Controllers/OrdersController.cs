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
        if (response.IsSuccessStatusCode){
            //HttpResponseMessage -> string
            string responseContent = await response.Content.ReadAsStringAsync();
            
            // Process the response content and pass data to a list
            IEnumerable<Order> orders = new List<Order>();
            orders =  JsonConvert.DeserializeObject<IEnumerable<Order>>(responseContent);
            return orders;
        }else{
            Console.WriteLine("No successful status code when retrieving API");
            return null;
        }
    }

    public async Task<ActionResult> Orders(){
        //All orders
        var orders = await GetOrders();

        //The different formats a date can be represented in
        string[] formats = { "dd-MM-yyyy HH:mm", "yyyy-MM-dd HH:mm" };

        //Dictionary for storing months and amount of orders to use in graph
        IDictionary<string, int> dictOrders = new Dictionary<string, int>();

        //Predefine the dictionary with every month 
        for (int i = 43; i >= 0; i--) {
            DateTime dateTime = DateTime.Now.AddMonths(-i);
            string shortDate = dateTime.Month.ToString("d2") + "/" + dateTime.Year.ToString();
            dictOrders.Add(shortDate,0);
        }

        //Adding the amount of orders on each month
        foreach (Order order in orders){
            DateTime dateTime;
            if (DateTime.TryParseExact(order.OrderDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime)){
                string shortDate = dateTime.Month.ToString("d2") + "/" + dateTime.Year.ToString();
                dictOrders[shortDate] = dictOrders[shortDate] + 1;
            }else{
                Console.WriteLine("Unable to parse DateTime");
            }
        }
        //Returning the dictionary to ordersView så the graph can be shown
        return View("OrdersView",dictOrders);
    }  
}
