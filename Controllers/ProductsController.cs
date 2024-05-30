using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Gendaccc.Models;
using Newtonsoft.Json;

namespace Gendaccc.Controllers
{
    public class ProductsController : Controller
    {
        private readonly HttpClient _httpClient;

        private const string ApiUrl = "https://gendacproficiencytest.azurewebsites.net/API/ProductsAPI/";

        public ProductsController()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // GET: Products
        public async Task<ActionResult> Index()
        {
            return await HandleApiCallAsync(
                async () => await _httpClient.GetAsync(ApiUrl),
                content => View(JsonConvert.DeserializeObject<List<Product>>(content)),
                new List<Product>(),
                "There was a problem retrieving the list of products."
            );
        }

        // GET: Products/Read/x
        public async Task<ActionResult> Read(int id)
        {
            return await HandleApiCallAsync(
                async () => await _httpClient.GetAsync($"{ApiUrl}{id}"),
                content => View(JsonConvert.DeserializeObject<Product>(content)),
                new Product(),
                $"There was a problem retrieving the product with ID {id}."
            );
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        public async Task<ActionResult> Create(Product product)
        {
            return await HandleApiCallAsync(
                async () =>
                {
                    var json = JsonConvert.SerializeObject(product);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    return await _httpClient.PostAsync(ApiUrl, data);
                },
                content => RedirectToAction("Index"),
                product,
                "There was a problem creating the product."
            );
        }

        // GET: Products/Edit/x
        public async Task<ActionResult> Edit(int id)
        {
            return await HandleApiCallAsync(
                async () => await _httpClient.GetAsync($"{ApiUrl}{id}"),
                content => View(JsonConvert.DeserializeObject<Product>(content)),
                new Product(),
                $"There was a problem retrieving the product with ID {id} for editing."
            );
        }

        // POST: Products/Edit/x
        [HttpPost]
        public async Task<ActionResult> Edit(int id, Product product)
        {
            return await HandleApiCallAsync(
                async () =>
                {
                    var json = JsonConvert.SerializeObject(product);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    return await _httpClient.PutAsync($"{ApiUrl}{id}", data);
                },
                content => RedirectToAction("Index"),
                product,
                "There was a problem updating the product."
            );
        }

        // GET: Products/Delete/x
        public async Task<ActionResult> Delete(int id)
        {
            return await HandleApiCallAsync(
                async () => await _httpClient.GetAsync($"{ApiUrl}{id}"),
                content => View(JsonConvert.DeserializeObject<Product>(content)),
                new Product(),
                $"There was a problem retrieving the product with ID {id} for deletion."
            );
        }

        // POST: Products/Delete/x
        [HttpPost]
        public async Task<ActionResult> Delete(int id, FormCollection collection)
        {
            return await HandleApiCallAsync(
                async () => await _httpClient.DeleteAsync($"{ApiUrl}{id}"),
                content => RedirectToAction("Index"),
                new Product(),
                "There was a problem deleting the product."
            );
        }

        // GET: Products/Search
        public async Task<ActionResult> Search(string searchString, int page = 1)
        {
            return await HandleApiCallAsync(
                async () => await _httpClient.GetAsync($"{ApiUrl}?page={page}&pageSize=50&orderBy=Name&ascending=true&filter={searchString ?? ""}"),
                content =>
                {
                    var products = JsonConvert.DeserializeObject<List<Product>>(content);
                    ViewBag.CurrentPage = page;
                    ViewBag.CurrentFilter = searchString;
                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("_ProductTable", products) as ActionResult;
                    }
                    else
                    {
                        return View(products) as ActionResult;
                    }
                },
                new List<Product>(),
                "There was a problem retrieving the list of products."
            );
        }

        private async Task<ActionResult> HandleApiCallAsync(Func<Task<HttpResponseMessage>> apiCall, Func<string, ActionResult> onSuccess, object modelOnError, string errorMessage)
        {
            try
            {
                var response = await apiCall();
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return onSuccess(content);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                ViewBag.ErrorMessage = errorMessage;
                return View(modelOnError);
            }
        }
    }
}
