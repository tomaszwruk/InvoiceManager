using InvoiceManager.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace InvoiceManager.Models.Repositories
{
    public class InvoiceRepository
    {
        public List<Invoice> GetInvoices(string userId)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Invoices.Include(x => x.Client).
                    Where(x => x.UserId == userId).
                    ToList();
            }
        }

        public Invoice GetInvoice(int id, string userId)
        {
            using (var context = new ApplicationDbContext())
            {//dołączane informacje o pozycjach faktury
                return context.Invoices
                    .Include(x => x.InvoicePositions)
                    .Include(x => x.InvoicePositions.Select(y => y.Product))
                    .Include(x => x.MethodOfPayment)
                    .Include(x => x.User)
                    .Include(x => x.User.Address)
                    .Include(x => x.Client)
                    .Include(x => x.Client.Address)
                    .Single(x => x.Id == id && x.UserId == userId);
                    
            };
        }

        public List<MethodOfPayment> GetMethodsOfPayment()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.MethodOfPayments.ToList();
            }
        }

        public InvoicePosition GetInvoicePosition(int invoicePositionId, string userId)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.InvoicePositions
                    .Include(x => x.Invoice)
                    .Single(x => x.Id == invoicePositionId &&
                        x.Invoice.UserId == userId);
                    
            }
        }

        public void Add(Invoice invoice)
        {
            using (var context = new ApplicationDbContext())
            {
                invoice.CreatedDate = DateTime.Now;
                context.Invoices.Add(invoice);
                context.SaveChanges();
            }
        }

        public void Update(Invoice invoice)
        {
            using (var context = new ApplicationDbContext())
            {
                var invoiceToUpodate = context.Invoices
                    .Single(x => x.Id == invoice.Id && x.UserId == invoice.UserId);

                invoiceToUpodate.ClientId = invoice.ClientId;
                invoiceToUpodate.Comments = invoice.Comments;
                invoiceToUpodate.MethodOfPayment = invoice.MethodOfPayment;
                invoiceToUpodate.PaymentDate = invoice.PaymentDate;
                invoiceToUpodate.Title = invoice.Title;
                invoiceToUpodate.ClientId = invoice.ClientId;

                context.SaveChanges();
            }
        }

        public void AddPosition(InvoicePosition invoicePosition, string userId)
        {
            using (var context = new ApplicationDbContext())
            {
                //sprawdzamy czy user robił wcześniej tą fakturę i czy może ją teraz zmienić
                //jeśli nie ma t'akiego rekordu wtedy zostanie rzucony wyjątek
                var invoice = context.Invoices
                    .Single(x => x.Id == invoicePosition.InvoiceId &&
                        x.UserId == userId);

                context.InvoicePositions.Add(invoicePosition);
                context.SaveChanges();
                
            }
        }

        public void UpdatePosition(InvoicePosition invoicePosition, string userId)
        {
            using (var context = new ApplicationDbContext())
            {
                var positionToUpodate = context.InvoicePositions
                    .Include(x => x.Product)
                    .Include(x => x.Invoice)
                    .Single(x =>
                        x.Id == invoicePosition.Id &&
                        x.Invoice.UserId == userId);

                positionToUpodate.Lp = invoicePosition.Lp;
                positionToUpodate.ProductId = invoicePosition.ProductId;
                positionToUpodate.Quantity = invoicePosition.Quantity;
                positionToUpodate.Value = invoicePosition.Product.Value * invoicePosition.Quantity;

                context.SaveChanges();
            };
        }

        public decimal UpdateInvoiceValue(int invoiceId, string userId)
        {
            using (var context = new ApplicationDbContext())
            {
                //sprawdzamy czy user robił wcześniej tą fakturę i czy może ją teraz zmienić
                //jeśli nie ma t'akiego rekordu wtedy zostanie rzucony wyjątek
                var invoice = context.Invoices
                    .Include(x => x.InvoicePositions)
                    .Single(x => x.Id == invoiceId &&
                        x.UserId == userId);

                invoice.Value = invoice.InvoicePositions.Sum(x => x.Value);

                return invoice.Value;

            }
        }

        public void Delete(int id, string userId)
        {
            using (var context = new ApplicationDbContext())
            {
                var invoiceToDelete = context.Invoices
                    .Single(x => x.Id == id &&
                        x.UserId == userId);
                context.Invoices.Remove(invoiceToDelete);
                context.SaveChanges();
            }
        }

        internal void DeletePosition(int id, string userId)
        {
            using (var context = new ApplicationDbContext())
            {
                var positionToDelete = context.InvoicePositions
                    .Include(x => x.Invoice)
                    .Single(x => x.Id == id &&
                        x.Invoice.UserId == userId);

                context.InvoicePositions.Remove(positionToDelete);
                context.SaveChanges();
            }
        }
    }
}