using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace Wingineers.Controllers;

public class SalesPersonController : Controller
{
    private readonly HttpClient _httpClient;
    public List<SalesPerson> salesPeople;

    public SalesPersonController(){
        _httpClient = new HttpClient();
    }
    
    public async Task<List<SalesPerson>> GetSalesPeople(){
        string baseUrl = "https://azurecandidatetestapi.azurewebsites.net/api/v1";
        string endpointPath = "/SalesPersons"; 
        string apiUrl = $"{baseUrl}{endpointPath}";

        string apiKey = "test1234";
        _httpClient.DefaultRequestHeaders.Add("ApiKey", apiKey);

        HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

        if (response.IsSuccessStatusCode){
            string responseContent = await response.Content.ReadAsStringAsync();
            // Process the response content and pass data to the view
            salesPeople =  JsonConvert.DeserializeObject<List<SalesPerson>>(responseContent);

            OrdersController ordersController = new();
            var orders = await ordersController.GetOrders();

            //Adding orders to each salesperson
            foreach (var salesPerson in salesPeople){
                salesPerson.Orders = orders.Where( x => x.SalesPersonId == salesPerson.Id).ToList();
            }
            return salesPeople;
        }
        else{
            return null;
        }
    }

    public async Task<ActionResult> SalesPeople(){
        var salesPeople = await GetSalesPeople();
        return View("SalesPersonView",salesPeople);
    } 

    public async Task<ActionResult> GetDetails(int salespersonid){
            IEnumerable<SalesPerson> salesPeople = await GetSalesPeople();
            //Get the salesperson with the same id
            var person = salesPeople.FirstOrDefault(x => x.Id == salespersonid);
            return View("SalesPersonDetailView", person);
    }
}
