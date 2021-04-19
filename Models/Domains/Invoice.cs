using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace InvoiceManager.Models.Domains
{
    public class Invoice
    {
        public Invoice()
        {
            InvoicePositions = new Collection<InvoicePosition>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Pole Tytuł jest wymagane")] //Title jest wymagane tak zeby nie było nulla, przy int nie trzeba pisać domyśnie będzie not null
                   //ten znacznik robimy zamiast konfiguracji tabeli w entity
        [Display(Name ="Tytuł")]
        public string Title { get; set; }

        [Display(Name = "Wartość")]
        public decimal Value { get; set; }

        [Required(ErrorMessage = "Pole Sposób płatności jest wymagane")]
        [Display(Name = "Sposób płatności")]
        public int MethodOfPaymentId { get; set; }

        [Display(Name = "Termin płatności")]
        public DateTime PaymentDate { get; set; }

        [Display(Name = "Utworzono")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Uwagi")]
        public string Comments { get; set; }

        [Display(Name = "Klient")]
        public int ClientId { get; set; }

        [Required]
        [ForeignKey("User")] //UserId to klucz obcy tabeli User - poniżej
        public string UserId { get; set; } //musi być string ponieważ w schemacie tworzenia jest tabelka i id występuje jako string id Usera


        public MethodOfPayment MethodOfPayment { get; set; }
        public Client Client { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<InvoicePosition> InvoicePositions { get; set; }
    }
}