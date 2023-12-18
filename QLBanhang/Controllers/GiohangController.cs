using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLBanhang.Models;
using System.Net;
using System.Net.Mail;

namespace QLBanhang.Controllers
{
    public class GiohangController : Controller
    {
        private qlbanhangEntities db = new qlbanhangEntities();
        // GET: Giohang
        public ActionResult Index()
        {
            List<CartItem> giohang = Session["giohang"] as List<CartItem>;
            return View(giohang);
        }
        // khai báo phương thức sản phẩm vào giỏ hàng
        public RedirectToRouteResult AddToCart (string MaSP)
        {
            if (Session["giohang"] == null)
            {
                Session["giohang"] = new List<CartItem>();
            }
            List<CartItem> giohang = Session["giohang"] as List<CartItem>;
            // Kiểm tra sản phẩm khách đang chọn có trong giỏ hàng chưa
            if(giohang.FirstOrDefault(m => m.MaSP == MaSP)==null)// chưa có trong giỏ hàng
            {
                SanPham sp = db.SanPhams.Find(MaSP);
                CartItem newItem = new CartItem();
                newItem.MaSP = MaSP;
                newItem.TenSP = sp.TenSP;
                newItem.SoLuong = 1;
                newItem.DonGia = Convert.ToDouble(sp.Dongia);
                giohang.Add(newItem);
            }
            else //sản phẩm đã có trong giỏ hàng ==> tăng số lượng lên 1 
            {
                CartItem cartItem = giohang.FirstOrDefault(m => m.MaSP == MaSP);
                cartItem.SoLuong++;
            }
            Session["giohang"] = giohang;
            return RedirectToAction("Index");
        }
        public RedirectToRouteResult Update (string MaSp, int txtSoLuong)
        {
            //tìm cartItem muốn xoá
            List<CartItem> giohang = Session["giohang"] as List<CartItem>;
            CartItem item = giohang.FirstOrDefault(m => m.MaSP == MaSp);
            if (item != null)
            {
                item.SoLuong = txtSoLuong;
                Session["giohang"] = giohang;
            }
            return RedirectToAction("Index");
        }
        public RedirectToRouteResult DelCartItem(string MaSp)
        {
            //tìm cartItem muốn xoá
            List<CartItem> giohang = Session["giohang"] as List<CartItem>;
            CartItem item = giohang.FirstOrDefault(m => m.MaSP == MaSp);
            if (item != null)
            {
                giohang.Remove(item);
                Session["giohang"] = giohang;
            }
            return RedirectToAction("Index");
        }
        public ActionResult Order (string Email, string Phone)
        {
            List<CartItem> giohang = Session["giohang"] as List<CartItem>;
            string sMsg = "<html><body><table border='1'><caption>Thông tin đặt Hàng</caption><tr><th>STT</th><th>Tên Hàng</th><th>Số Lượng</th><th>Đơn Giá</th><th>Thành Tiền</th></tr>";
            int i = 0;
            double tongtien = 0;
            foreach (CartItem item in giohang)
            {
                i++;
                sMsg += "<tr>";
                sMsg += "<td>" + i.ToString() + "</td>";
                sMsg += "<td>" + item.TenSP + "</td>";
                sMsg += "<td>" + item.SoLuong.ToString() + "</td>";
                sMsg += "<td>" + item.DonGia.ToString() + "</td>";
                sMsg += "<td>" + String.Format("{0:#,###}", item.SoLuong*item.DonGia) + "</td>"; 
                sMsg += "</tr>"; 
                tongtien += item.SoLuong * item.DonGia;
            }
            sMsg +="<tr><th colpan='5'>Tổng Cộng: " + String.Format("{0:#,### vnđ}", tongtien) + "</th></tr></table>";
            MailMessage mail = new MailMessage("nguyenchihien03@gmail.com", Email, "Thông tin đơn hàng", sMsg);
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("nguyenchihien03@gmail.com", "tgvu sasb evja rqub");
            mail.IsBodyHtml = true;
            client.Send(mail);
            return RedirectToAction("Index", "Home");
            //gởi mail cho khách hàng
        }  
    }
}