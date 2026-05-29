using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class PedidosController : Controller
    {
        private ModelDb db = new ModelDb();

        public ActionResult Index()
        {
            return View(db.Pedidos.ToList());
        }

        public ActionResult Create()
        {
            ViewBag.id_cliente = new SelectList(db.Clientes, "ID", "Nombre");
            ViewBag.id_producto = new SelectList(db.Productos, "ID", "Descripcion");
            return View();
        }

        [HttpPost]
        public JsonResult CrearPedido(PedidoViewModel model)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    // 1. Guardar el Maestro (Pedido)
                    Pedidos nuevoPedido = new Pedidos
                    {
                        id_cliente = model.IdCliente,
                        total = model.Total
                    };
                    db.Pedidos.Add(nuevoPedido);
                    db.SaveChanges(); // Genera el ID del pedido

                    // 2. Guardar los Detalles
                    foreach (var item in model.Detalles)
                    {
                        Pedidos_detalle detalle = new Pedidos_detalle
                        {
                            id_pedido = nuevoPedido.ID, // ID recién generado
                            id_producto = item.IdProducto,
                            cant = item.Cantidad,
                            precio = item.Precio
                        };
                        db.Pedidos_detalle.Add(detalle);

                        // Opcional: Restar del Stock en la tabla Productos
                        var producto = db.Productos.Find(item.IdProducto);
                        if (producto != null)
                        {
                            producto.Stock -= item.Cantidad;
                        }
                    }

                    db.SaveChanges();
                    dbContextTransaction.Commit(); // Confirmamos los cambios en la BD

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback(); // Si algo falla, deshace todo
                    return Json(new { success = false, message = "Error: " + ex.Message });
                }
            }
        }

        public ActionResult Details(int id)
        {
            var pedido = db.Pedidos.Find(id);
            if (pedido == null) return HttpNotFound();

            return View(pedido);
        }

        public ActionResult Edit(int id)
        {
            var pedido = db.Pedidos.Find(id);
            if (pedido == null) return HttpNotFound();

            ViewBag.id_cliente = new SelectList(db.Clientes, "ID", "Nombre", pedido.id_cliente);
            ViewBag.id_producto = new SelectList(db.Productos, "ID", "Descripcion");

            return View(pedido);
        }

        [HttpPost]
        public JsonResult EditarPedido(int idPedido, PedidoViewModel model)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var pedidoExistente = db.Pedidos.Find(idPedido);
                    if (pedidoExistente == null) return Json(new { success = false, message = "Pedido no encontrado" });

                    // 1. Devolver el stock actual de los detalles viejos antes de borrarlos
                    var detallesViejos = db.Pedidos_detalle.Where(d => d.id_pedido == idPedido).ToList();
                    foreach (var detViejo in detallesViejos)
                    {
                        var prod = db.Productos.Find(detViejo.id_producto);
                        if (prod != null) prod.Stock += detViejo.cant; // Devolvemos stock

                        db.Pedidos_detalle.Remove(detViejo); // Eliminamos el detalle viejo
                    }

                    // 2. Actualizar datos maestros
                    pedidoExistente.id_cliente = model.IdCliente;
                    pedidoExistente.total = model.Total;

                    // 3. Insertar los nuevos detalles y restar stock actualizado
                    foreach (var item in model.Detalles)
                    {
                        Pedidos_detalle nuevoDetalle = new Pedidos_detalle
                        {
                            id_pedido = idPedido,
                            id_producto = item.IdProducto,
                            cant = item.Cantidad,
                            precio = item.Precio
                        };
                        db.Pedidos_detalle.Add(nuevoDetalle);

                        var prod = db.Productos.Find(item.IdProducto);
                        if (prod != null)
                        {
                            if (prod.Stock < item.Cantidad)
                                throw new Exception($"Stock insuficiente para el producto ID {item.IdProducto}");

                            prod.Stock -= item.Cantidad;
                        }
                    }

                    db.SaveChanges();
                    dbContextTransaction.Commit();
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    return Json(new { success = false, message = "Error: " + ex.Message });
                }
            }
        }

        public ActionResult Delete(int id)
        {
            var pedido = db.Pedidos.Find(id);
            if (pedido == null) return HttpNotFound();
            return View(pedido);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var pedido = db.Pedidos.Find(id);
                    var detalles = db.Pedidos_detalle.Where(d => d.id_pedido == id).ToList();

                    // Devolver stock antes de eliminar los registros
                    foreach (var det in detalles)
                    {
                        var prod = db.Productos.Find(det.id_producto);
                        if (prod != null) prod.Stock += det.cant;

                        db.Pedidos_detalle.Remove(det);
                    }

                    db.Pedidos.Remove(pedido);
                    db.SaveChanges();

                    dbContextTransaction.Commit();
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    dbContextTransaction.Rollback();
                    return RedirectToAction("Delete", new { id = id, error = true });
                }
            }
        }

        [HttpGet]
        public JsonResult ObtenerPrecioProducto(int idProducto)
        {
            var producto = db.Productos.Find(idProducto);
            if (producto != null)
            {
                return Json(new { precio = producto.Precio }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { precio = 0 }, JsonRequestBehavior.AllowGet);
        }
    }
}