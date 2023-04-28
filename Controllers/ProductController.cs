using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using TestMVC.Models;

namespace TestMVC.Controllers
{

    public class ProductController : Controller
    {
        //Ver listado de productos
        public async Task<ActionResult> VerProducto()
        {
            var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync("https://localhost:7120/product");
            var productList = JsonConvert.DeserializeObject<List<Producto>>(json);
            return View(productList);
        }

        // GET: Product/Createroduct
        public ActionResult CreateProduct()
        {
            return View();
        }

        // POST: Product/CreateProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateProduct([Bind(Include = "IdProd,NombreProd,PrecioProd,EstadoProd")] Producto producto)
        {
            var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync("https://localhost:7120/product");

            var productList = JsonConvert.DeserializeObject<List<Producto>>(json);

            if (ModelState.IsValid)
            {
                var productoExistente = productList.FirstOrDefault(p => p.IdProd == producto.IdProd);

                if (productoExistente == null)
                {
                    // Serializar el objeto producto en formato JSON y enviarlo mediante un método POST
                    var response = await httpClient.PostAsync("https://localhost:7120/product", new StringContent(JsonConvert.SerializeObject(producto), Encoding.UTF8, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        productList.Add(producto);
                        return RedirectToAction("VerProducto");
                    }
                    if(response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        ModelState.AddModelError("", "Es el mismo ID");
                    }
                }
                else
                {
                    ModelState.AddModelError("IdProd", "Ya existe un producto con este Id.");
                }
            }
            ModelState.AddModelError(nameof(Producto.IdProd),"Este es un mensaje de prueba");
            
            return View(producto);
        }

        // GET: Producto/Edit/5
        public async Task<ActionResult> EditProduct(int? id)
        {
            var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync("https://localhost:7120/product");
            var prod = JsonConvert.DeserializeObject<List<Producto>>(json);
            var producto = prod.FirstOrDefault(p => p.IdProd == id);

            if (producto == null)
            {
                return  HttpNotFound();
            }

            return View(producto);
        }

        // POST: Producto/EditProduct/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditProduct([Bind(Include = "IdProd,NombreProd,PrecioProd,EstadoProd")] Producto producto)
        {
            var httpClient = new HttpClient();

            if (ModelState.IsValid)
            {
                var jsonActualizado = JsonConvert.SerializeObject(producto);
                var content = new StringContent(jsonActualizado, Encoding.UTF8, "application/json");
                var respuesta = await httpClient.PutAsync("https://localhost:7120/product/EditProduct/" + producto.IdProd, content);

                if (respuesta.IsSuccessStatusCode)
                {
                    // Redirige a la página si se realiza correctamente
                    return RedirectToAction("VerProducto");
                }
                else
                {
                    ModelState.AddModelError("", "Ha ocurrido un error al actualizar el producto.");
                }
            }
            return View(producto);
        }

        // GET: Producto/BorrarProducto/5
        public async Task<ActionResult> BorrarProducto(int? id)
        {
            var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync("https://localhost:7120/product");

            var prod = JsonConvert.DeserializeObject<List<Producto>>(json);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var producto = prod.FirstOrDefault(p => p.IdProd == id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // POST: Producto/BorrarProducto/5
        [HttpPost, ActionName("BorrarProducto")]
        [ValidateAntiForgeryToken]
        public async  Task<ActionResult> BorrarProducto(int id)
        {
            var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync("https://localhost:7120/product");
            var prod = JsonConvert.DeserializeObject<List<Producto>>(json);
            var producto = prod.FirstOrDefault(p => p.IdProd == id);

            if (producto != null)
            {
                // Si se encuentra el producto, realiza una solicitud DELETE a la API para eliminarlo
                var respuesta = await httpClient.DeleteAsync("https://localhost:7120/product/BorrarProducto/" + id);
                if (respuesta.IsSuccessStatusCode)
                {
                    return RedirectToAction("VerProducto");
                }
                else
                {
                    ViewBag.ErrorMessage = "Hubo un problema al eliminar el producto.";
                    return View(producto);
                }
            }
            else
            {
                ViewBag.ErrorMessage = "No se encontró el producto.";
                return View(producto);
            }
        }




    }
}