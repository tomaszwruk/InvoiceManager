using InvoiceManager.Models.Domains;
using InvoiceManager.Models.Repositories;
using InvoiceManager.Models.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InvoiceManager.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private InvoiceRepository _invoiceRepository = new InvoiceRepository();
        private ClientRepository _clientRepository = new ClientRepository();
        private ProductRepository _productRepository = new ProductRepository();

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId() ;
            var invoices = _invoiceRepository.GetInvoices(userId);
            
            return View(invoices);

            //var invoices = new List<Invoice>
            //{
            //    new Invoice
            //    {
            //        Id = 1,
            //        Title = "Fa/01/2021",
            //        CreatedDate = DateTime.Now,
            //        PaymentDate = DateTime.Now,
            //        Value = 999,
            //        Client = new Client { Name = "Daneb test" }
            //    },

            //    new Invoice
            //    {
            //        Id = 2,
            //        Title = "Fa/02/2021",
            //        CreatedDate = DateTime.Now,
            //        PaymentDate = DateTime.Now,
            //        Value = 6565,
            //        Client = new Client { Name = "PKO SA" }
            //    },
            //};
            //return View(invoices);

        }

        public ActionResult InvoicePosition(int invoiceId, int invoicePositionId = 0)
        {
            var userId = User.Identity.GetUserId();

            var invoicePosition = invoicePositionId == 0 ?
                GetNewPosition(invoiceId, invoicePositionId) :
                _invoiceRepository.GetInvoicePosition(invoicePositionId, userId);
            var vm = PrepareInvoicePositionVm(invoicePosition);

            return View(vm);
            ////parametry muszą być takie same jak wywoływane w widoku _InvoicePosition
            //EditInvoicePositionViewModel vm = null;

            //if (invoicePositionId == 0)
            //{
            //    vm = new EditInvoicePositionViewModel
            //    {
            //        InvoicePosition = new InvoicePosition(),
            //        Heading = "Dodawanie nowej pozycji",
            //        Product = new List<Product>
            //        {
            //            new Product
            //            {
            //                Id = 1,
            //                Name = "produkt 11"
            //            }
            //        }
            //    };
            //}
            //else
            //{
            //    vm = new EditInvoicePositionViewModel
            //    {
            //        InvoicePosition = new InvoicePosition 
            //        { 
            //            Lp = 1, Value = 100, Quantity = 2, ProductId = 1
            //        },
            //        Heading = "Edycja pozycji",
            //        Product = new List<Product>
            //        {
            //            new Product
            //            {
            //                Id = 1,
            //                Name = "produkt 11"
            //            }
            //        }
            //    };
            //}
            
            //return View(vm);
        }

        private EditInvoicePositionViewModel PrepareInvoicePositionVm(InvoicePosition invoicePosition)
        {
            var _productRepository = new ProductRepository();

            return new EditInvoicePositionViewModel
            {
                InvoicePosition = invoicePosition,
                Heading = invoicePosition.Id == 0 ?
                 "Dodawanie nowej pozycji" :
                 "Pozycja",
                Products = _productRepository.GetProducts()
            };
        }

        private InvoicePosition GetNewPosition(int invoiceId, int invoicePositionId)
        {
            return new InvoicePosition
            {
                InvoiceId = invoiceId,
                Id = invoicePositionId
            };
        }

        [HttpPost]
        //po wypełnieu formularza w Invoice.cshtml wysyłamy go i kontroler musi zdefiniować akcję post 
        public ActionResult Invoice(Invoice invoice)
        {
            var userId = User.Identity.GetUserId();//dla bezpieczeństwa lepiej zawsze czytać na nowo usera z bazy
            invoice.UserId = userId;

            if (invoice.Id == 0)
            {
                _invoiceRepository.Add(invoice);
            }
            else
            {
                _invoiceRepository.Update(invoice);
            }

            return RedirectToAction("Index"); //zwracamy przekierowanie do listy faktur
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var userId = User.Identity.GetUserId();//dla bezpieczeństwa lepiej zawsze czytać na nowo usera z bazy
                _invoiceRepository.Delete(id, userId);
            }
            catch (Exception exception)
            {

                return Json(new { Success = false, Message = exception.Message });
                
            }

            return Json(new { Success = true });
        }

        [HttpPost]
        public ActionResult DeletePosition(int id, int invoiceId)
        {
            var invoiceValue = 0m; //m decimal

            try
            {
                var userId = User.Identity.GetUserId();//dla bezpieczeństwa lepiej zawsze czytać na nowo usera z bazy
                _invoiceRepository.DeletePosition(id, userId);
                invoiceValue = _invoiceRepository.UpdateInvoiceValue(invoiceId, userId);
            }
            catch (Exception exception)
            {

                return Json(new { Success = false, Message = exception.Message });
            }

            return Json(new { Success = true });
        }

        [HttpPost]
        public ActionResult InvoicePosition(InvoicePosition invoicePosition)
        {
            var userId = User.Identity.GetUserId();//dla bezpieczeństwa lepiej zawsze czytać na nowo usera z bazy
            var product = _productRepository.GetProduct(invoicePosition.ProductId);
            invoicePosition.Value = invoicePosition.Quantity * product.Value;

            if (invoicePosition.Id == 0)
            {
                _invoiceRepository.AddPosition(invoicePosition, userId);
            }
            else
            {
                _invoiceRepository.UpdatePosition(invoicePosition, userId);
            }

            _invoiceRepository.UpdateInvoiceValue(invoicePosition.InvoiceId, userId);

            return RedirectToAction("Invoice", 
                new { id = invoicePosition.InvoiceId }); //zwracamy przekierowanie
        }

        public ActionResult Invoice(int id = 0)
        {
            var userId = User.Identity.GetUserId();
            var invoice = id == 0 ?
                GetNewInvoice(userId) :
                _invoiceRepository.GetInvoice(id, userId);

            var vm = PrepareInvoiceVm(invoice, userId);
            return View(vm);

            //editinvoiceviewmodel vm = null;
            //if (id == 0)
            //{
            //    vm = new editinvoiceviewmodel
            //    {
            //        clients = new list<client> { new client { id = 1, name = "daneb test" } },
            //        methodofpayment = new list<methodofpayment> { new methodofpayment { id = 1, name = "przelew" } },
            //        heading = "edycja faktury",
            //        invoice = new invoice()
            //    };
            //}
            //else
            //{
            //    vm = new editinvoiceviewmodel
            //    {
            //        clients = new list<client> { new client { id = 1, name = "daneb test" } },
            //        methodofpayment = new list<methodofpayment> { new methodofpayment { id = 1, name = "przelew" } },
            //        heading = "edycja faktury",
            //        invoice = new invoice
            //        {
            //            clientid = 1,
            //            comments = "testowe opisy",
            //            createddate = datetime.now,
            //            paymentdate = datetime.now,
            //            methodofpaymentid = 1,
            //            id = 1,
            //            value = 100,
            //            title = "fa/10/2021",
            //            invoicepositions = new list<invoiceposition>
            //            {
            //                new invoiceposition
            //                {
            //                    id = 1,
            //                    lp = 1,
            //                    product = new product { name = "product"},
            //                    quantity = 2,
            //                    value = 32
            //                },
            //                new invoiceposition
            //                {
            //                    id = 2,
            //                    lp = 2,
            //                    product = new product { name = "product df3"},
            //                    quantity = 5,
            //                    value = 302
            //                }
            //            }
            //        }
            //    };

            //}            
            //return View(vm);//zwraca widok Invoice, można automatem dodawać przez PPM i dodaj widok
        }

        private EditInvoiceViewModel PrepareInvoiceVm(Invoice invoice, string userId)
        {
            return new EditInvoiceViewModel
            {
                Invoice = invoice,
                Heading = invoice.Id == 0 ? "Dodawanie nowej faktury" : "Faktura",
                Clients = _clientRepository.GetClients(userId),
                MethodOfPayment = _invoiceRepository.GetMethodsOfPayment()

            };
        }

        private Invoice GetNewInvoice(string userId)
        {
            return new Invoice
            {
                UserId = userId,
                CreatedDate = DateTime.Now,
                PaymentDate = DateTime.Now.AddDays(14)
            };
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}