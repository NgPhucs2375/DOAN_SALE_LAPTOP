using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using WEB_SALE_LAPTOP.Models;

namespace WEB_SALE_LAPTOP.Controllers
{
    public class PaymentController : Controller
    {
        private QL_LAPTOP db = new QL_LAPTOP();

        // 1. GỬI YÊU CẦU THANH TOÁN (PHIÊN BẢN SIÊU ỔN ĐỊNH)
        public ActionResult PaymentMomo()
        {
            // Kích hoạt TLS 1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Lấy giỏ hàng
            var cart = Session["CartSession"] as List<CartItem>;
            if (cart == null || cart.Count == 0) return RedirectToAction("Index", "Cart");

            // Tính tiền
            decimal totalAmount = cart.Sum(x => x.ThanhTien);
            decimal discount = (decimal)(Session["SoTienGiam"] ?? 0m);
            decimal finalAmount = totalAmount - discount;

            // [QUAN TRỌNG]: Ép số tiền về 10.000đ để test (Luôn thành công)
            // Sau khi test xong, bạn có thể bỏ dòng này để dùng số tiền thật
            string amount = "10000";

            // Cấu hình MoMo (Key Công cộng chuẩn nhất hiện nay)
            string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
            string partnerCode = "MOMO5RGX20191128";
            string accessKey = "M8brj9K6E1Rv0859";
            string secretKey = "nqQiVSgDMy809JoPF6OzP5OdBUB550Y4";

            string orderInfo = "Thanh toan laptop";
            string redirectUrl = "https://localhost:44311/Payment/MomoReturn";
            string ipnUrl = "https://localhost:44311/Payment/MomoIPN";
            string requestType = "captureWallet";

            string orderId = DateTime.Now.Ticks.ToString();
            string requestId = orderId;
            string extraData = "";

            // TẠO CHỮ KÝ (SIGNATURE)
            string rawHash = "accessKey=" + accessKey +
                "&amount=" + amount +
                "&extraData=" + extraData +
                "&ipnUrl=" + ipnUrl +
                "&orderId=" + orderId +
                "&orderInfo=" + orderInfo +
                "&partnerCode=" + partnerCode +
                "&redirectUrl=" + redirectUrl +
                "&requestId=" + requestId +
                "&requestType=" + requestType;

            string signature = ComputeHmacSha256(rawHash, secretKey);

            // TẠO JSON REQUEST
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "partnerName", "Test" },
                { "storeId", "MomoTestStore" },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderId },
                { "orderInfo", orderInfo },
                { "redirectUrl", redirectUrl },
                { "ipnUrl", ipnUrl },
                { "lang", "vi" },
                { "extraData", extraData },
                { "requestType", requestType },
                { "signature", signature }
            };

            // GỬI REQUEST
            string responseFromMomo = SendPaymentRequest(endpoint, message.ToString());
            JObject jmessage = JObject.Parse(responseFromMomo);

            // KIỂM TRA KẾT QUẢ
            var payUrl = jmessage.GetValue("payUrl");
            if (payUrl != null)
            {
                return Redirect(payUrl.ToString());
            }
            else
            {
                // Nếu lỗi, hiển thị chi tiết để biết đường sửa
                string errorMsg = jmessage.GetValue("message")?.ToString() ?? "Lỗi không xác định.";
                ViewBag.Message = "Lỗi từ MoMo: " + errorMsg;
                return View("PaymentFail");
            }
        }

        // 2. XỬ LÝ KẾT QUẢ TRẢ VỀ
        public ActionResult MomoReturn()
        {
            string errorCode = Request.QueryString["errorCode"];
            string orderId = Request.QueryString["orderId"];

            if (errorCode == "0")
            {
                // LƯU ĐƠN HÀNG
                SaveOrderToDatabase(orderId);
                ViewBag.Message = "Giao dịch thành công! Mã đơn: " + orderId;
                return View("PaymentSuccess");
            }
            else
            {
                ViewBag.Message = "Giao dịch thất bại (Mã lỗi: " + errorCode + ")";
                return View("PaymentFail");
            }
        }

        // === CÁC HÀM HỖ TRỢ ===
        private void SaveOrderToDatabase(string orderId)
        {
            var cart = Session["CartSession"] as List<CartItem>;
            var user = Session["UserCustomer"] as KHACHHANG;
            decimal discount = (decimal)(Session["SoTienGiam"] ?? 0m);
            string maVoucher = Session["MaVoucher"] as string;

            if (cart != null)
            {
                decimal totalAmount = cart.Sum(x => x.ThanhTien);
                HOADON hoadon = new HOADON
                {
                    NGAYLAP = DateTime.Now,
                    MAKH = (user != null) ? (int?)user.MAKH : null,
                    TONGTIEN_HANG = totalAmount,
                    SOTIEN_GIAM_VOUCHER = discount,
                    TONG_THANHTOAN = totalAmount - discount,
                    TRANGTHAI = "Đã thanh toán (MoMo)",
                    MAVOUCHER = maVoucher,
                    DIACHI_GIAO = (user != null) ? user.DIACHI : "Khách vãng lai",
                    SDT_GIAO = (user != null) ? user.SODT : ""
                };

                db.HOADONs.Add(hoadon);
                db.SaveChanges();

                foreach (var item in cart)
                {
                    CT_HOADON ct = new CT_HOADON
                    {
                        MAHD = hoadon.MAHD,
                        MALAPTOP = item.MaLaptop,
                        SOLUONG = item.SoLuong,
                        DONGIA = item.DonGia,
                        THANHTIEN = item.ThanhTien
                    };
                    db.CT_HOADON.Add(ct);

                    var sp = db.LAPTOPs.Find(item.MaLaptop);
                    if (sp != null) sp.SOLUONG_TON -= item.SoLuong;
                }

                if (!string.IsNullOrEmpty(maVoucher))
                {
                    var v = db.VOUCHERs.FirstOrDefault(x => x.MAVOUCHER == maVoucher);
                    if (v != null) v.DA_DUNG++;
                }

                db.SaveChanges();
                Session.Remove("CartSession");
                Session.Remove("MaVoucher");
                Session.Remove("SoTienGiam");
            }
        }

        private string ComputeHmacSha256(string message, string secretKey)
        {
            var encoding = new System.Text.UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(secretKey);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                string hex = BitConverter.ToString(hashmessage);
                return hex.Replace("-", "").ToLower();
            }
        }

        private string SendPaymentRequest(string endpoint, string postJsonString)
        {
            try
            {
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(endpoint);
                var postData = Encoding.UTF8.GetBytes(postJsonString);

                httpWReq.ProtocolVersion = HttpVersion.Version11;
                httpWReq.Method = "POST";
                httpWReq.ContentType = "application/json";
                httpWReq.ContentLength = postData.Length;
                httpWReq.Timeout = 15000;

                using (var stream = httpWReq.GetRequestStream())
                {
                    stream.Write(postData, 0, postData.Length);
                }

                var response = (HttpWebResponse)httpWReq.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (WebException e)
            {
                return "{\"errorCode\": \"99\", \"localMessage\": \"Lỗi kết nối: " + e.Message + "\"}";
            }
        }
    }
}