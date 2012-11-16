using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContactManager.Models;

namespace ContactManager.Controllers
{
    public class ContactController : Controller
    {
        //
        // GET: /Home/
        // Muestra la lista de  Contactos
        public ActionResult Index()
        {
            // Best practice: 'using' pattern
            using (ContactManagerDBEntities ctx = new ContactManagerDBEntities())
            {
                // Linq to Entities
                var contacts = ctx.Contacts.ToList();
                return View(contacts);
            }

        }

        //
        // GET: /Home/Create
        // Muestra el formulario vacio
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Home/Create
        // Recupera los datos introducidos por el usuario y los guarda en la bd
        [HttpPost] // Esta anotacion indica que esta accion solo estara disponible en peticiones POST
        public ActionResult Create([Bind(Exclude = "Id")]Contact newContact)
        {
            // El componente Model Binder automaticamente recupera los valores del formulario
            // y crea un objeto Contact, basandose en el nombre de las propiedades

            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                using (var ctx = new ContactManagerDBEntities())
                {
                    // Añadimos el nuevo objeto al contexto actual y enviamos los cambios a la bd
                    ctx.AddToContacts(newContact);
                    ctx.SaveChanges();
                }

                // Pattern PRG (Post/Redirect/Get): Si todo sale bien redirigimos al usuario a la lista
                // inicial, en lugar de  mostrar un mensaje en la vista actual, evitando posibles duplicidades
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }

        }

        //
        // GET: /Home/Edit/5
        // Muestra el formulario con la informacion del contacto
        public ActionResult Edit(int id)
        {
            // Recuperamos el contacto
            using (var ctx = new ContactManagerDBEntities())
            {
                // Linq to Entities, FirstOrDefault devuelve una instancia
                var editContact = ctx.Contacts.FirstOrDefault(c => c.Id == id);

                // Generamos la vista con el modelo
                return View(editContact);
            }

        }

        //
        // POST: /Home/Edit/5
        // Recupera  los datos introducidos en el formulario y los guarda en la base de datos
        [HttpPost]
        public ActionResult Edit(Contact editedContact)
        {
            // Si hay errores en el modelo volvemos a presentar el formulario
            if (!ModelState.IsValid)
            {
                return View();
            }
            try
            {

                using (var ctx = new ContactManagerDBEntities())
                {
                    // Recuperamos el Contacto Original, para que EF pueda gestionar 
                    // automaticamente la modificacion
                    var originalContact = ctx.Contacts.SingleOrDefault(c => c.Id == editedContact.Id);

                    // Aplicamos los cambios
                    ctx.Contacts.ApplyCurrentValues(editedContact);
                    ctx.SaveChanges();

                    // Post/Redirect/Get 
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Home/Delete/5

        public ActionResult Delete(int id)
        {
            using (var ctx = new ContactManagerDBEntities())
            {
                var delContact = ctx.Contacts.FirstOrDefault(c => c.Id == id);
                // Otra alternativa para pasar el modelo a la vista
                this.ViewData.Model = delContact;
                return View();
            }

        }

        //
        // POST: /Home/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, Contact delContact)
        {
            try
            {
                using (var ctx = new ContactManagerDBEntities())
                {
                    var contact = ctx.Contacts.SingleOrDefault(c => c.Id == id);
                    ctx.DeleteObject(contact);
                    ctx.SaveChanges();

                    return RedirectToAction("Index");
                }                
            }
            catch
            {
                return View();
            }
        }
    }
}
