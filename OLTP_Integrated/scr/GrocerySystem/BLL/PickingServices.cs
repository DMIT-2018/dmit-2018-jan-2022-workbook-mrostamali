#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using GrocerySystem.Models;
using GrocerySystem.DAL;
using GrocerySystem.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Data.SqlTypes;
#endregion


namespace GrocerySystem.BLL
{
    public class PickingServices
    {
        #region Constructor and DI variable setup
        private readonly GrocerylistContext _context;

        internal PickingServices(GrocerylistContext context)
        {
            _context = context;
        }
        #endregion

        #region Query
        public QueryOrderList Picking_GetPickingOrder(int orderid,
                                                      int pickerid)
        {
            QueryOrderList info = _context.Orders
                                 .Where(x => x.OrderID == orderid)
                                 .Select(x => new QueryOrderList
                                 {
                                     PickerName = _context.Pickers
                                                    .Where(y => y.PickerID == pickerid)
                                                    .Select(x => x.FirstName + " " + x.LastName)
                                                    .FirstOrDefault(),
                                     CustomerName = x.Customer.FirstName + " " + x.Customer.LastName,
                                     CustomerPhone = x.Customer.Phone,
                                     OrderedItems = x.OrderLists
                                                    .Select(o => new QueryOrderListItem
                                                    {
                                                        OrderListId = o.OrderListID,
                                                        ProductId = o.ProductID,
                                                        Description = o.Product.Description,
                                                        OrderQty = o.QtyOrdered,
                                                        OrderComment = o.CustomerComment
                                                    })
                                                    .ToList()
                                 }).FirstOrDefault();
            return info;

        }
        #endregion

        #region Command
        public void Picking_RecordPicking(int orderid, int pickerid, List<PickedItem> orderitems)
        {
            #region TODO: Student Code Here
            // TODO: Student Code Here
            //Money = Decimal
            //Float = Double
            //1) Create local variables
            Order orderExists = null;
            Picker pickerExists = null;
            OrderList orderitemExists = null;
            Product productExists = null;
            decimal gst = 0.0m;
            decimal subtotal = 0.0m;
            List<Exception> errorlist = new List<Exception>();

            //Parameter Exists
            if(orderid <= 0)
            {
                throw new ArgumentNullException($"Order number of {orderid} is invalid");
            }
            if(pickerid <= 0)
            {
                throw new ArgumentNullException($"Order number of {pickerid} is invalid");
            }
            //List has at least one item picked
            if(orderitems == null)
            {
                throw new ArgumentNullException($"There are no items on order picking list submitted");
            }
            else
            {
                if(!orderitems.Any())
                {
                    throw new ArgumentNullException($"There are no items on order picking list submitted");
                }
            }

            //Rules:
            //Order exists
            orderExists = _context.Orders
                          .Where(x => x.OrderID == orderid)
                          .FirstOrDefault();
            if (orderExists == null)
            {
                throw new ArgumentNullException($"Order {orderid} does not exist.");
            }
            else
            {
                if (orderExists.Status.Equals("R"))
                {
                    throw new ArgumentNullException($"Order{orderid} has already been picked");
                }
            }

            //Picker exists
            pickerExists = _context.Pickers
                          .Where(x => x.PickerID == pickerid)
                          .FirstOrDefault();
            if (pickerExists == null)
            {
                errorlist.Add(new Exception($"Picker {pickerid} does not exist."));
            }
            //Picker assigned to store where order is placed
            if (orderExists != null && pickerExists != null)
            {
                if (orderExists.StoreID != pickerExists.StoreID)
                {
                    errorlist.Add(new Exception($"This Picker {pickerid} is not valid for order store."));
                }
            }
            else
            {
                errorlist.Add(new Exception($"Picker not valid for order store"));
            }

            //Each Order item exists
            foreach (var item in orderitems)
            {
                orderitemExists = _context.OrderLists
                          .Where(x => x.OrderListID == item.OrderListId)
                          .FirstOrDefault();
                productExists = _context.Products
                           .Where(x => x.ProductID == item.ProductId)
                           .FirstOrDefault();
                if (orderitemExists == null)
                {
                    errorlist.Add(new Exception($"Order item {productExists.Description} no longer on order."));
                }
                else
                {
                    //Every item picked quantity is positive (greater or equal to zero)
                    if (item.QtyPicked < 0)
                    {
                        errorlist.Add(new Exception($"Order item {productExists.Description} " +
                            $"Picked Quantity ({item.QtyPicked}) is invalid. Must be 0 or greater."));
                    }
                    //Any item that is picked that has a picked quanity not equal to the ordered qty must have a picked concern  
                    if((item.QtyPicked !=(decimal)orderitemExists.QtyOrdered)
                        && string.IsNullOrWhiteSpace(item.Pickedissue))
                    {
                        errorlist.Add(new Exception($"Order item {productExists.Description} order and " +
                            $"pick quantity ({item.QtyPicked}) mismatch. " +
                            $"Missing picker isssue. Picked issue is required in this case."));
                    }
                    //Update OrderList items quantity picked, product price, product discount, Pick issue
                    orderitemExists.QtyPicked = (double)item.QtyPicked;
                    orderitemExists.Price = productExists.Price;
                    orderitemExists.Discount = productExists.Discount;
                    orderitemExists.PickIssue = item.Pickedissue;

                    subtotal += (decimal)item.QtyPicked * (productExists.Price - productExists.Discount);
                    if(productExists.Taxable)
                    {
                        gst += (decimal)item.QtyPicked * (productExists.Price - productExists.Discount) * 0.05m;
                    }

                    EntityEntry<OrderList> updating = _context.Entry(orderitemExists);
                    updating.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
            }
            //Update Order picker, last date updated (today), subtotal, gst, status (R)
            if (orderExists != null)
            {
                orderExists.Status = "R";
                orderExists.SubTotal = subtotal;
                orderExists.GST = gst;
                orderExists.PickerID = pickerid;
                orderExists.LastStatusUpdate = DateTime.Now;
                EntityEntry<Order> updatingO = _context.Entry(orderExists);
                updatingO.State = Microsoft.EntityFrameworkCore.EntityState.Modified; 
            }
            if (errorlist.Any())
            {
                throw new AggregateException("Unable to add the order. Check concerns:", errorlist);
            }
            else
            {
                _context.SaveChanges();
            }
            #endregion
        }
        #endregion


    }
}
