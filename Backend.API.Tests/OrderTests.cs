using Backend.API.Models;
using Xunit;

namespace Backend.API.Tests
{
    public class OrderTests
    {
        [Fact]
        public void TongTien_DonHang_TinhToanChinhXac()
        {
            // Arrange
            var order = new Order();
            var detail1 = new OrderDetail { SoLuong = 2, DonGia = 50000 };
            var detail2 = new OrderDetail { SoLuong = 1, DonGia = 150000 };
            
            // Act
            order.OrderDetails.Add(detail1);
            order.OrderDetails.Add(detail2);
            order.TongTien = order.OrderDetails.Sum(d => d.SoLuong * d.DonGia);
            
            // Assert
            Assert.Equal(250000, order.TongTien);
        }
    }
}
