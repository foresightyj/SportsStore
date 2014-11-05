using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;
using SportsStore.WebUI.HtmlHelpers;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void Can_Paginate()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductId = 1, Name = "P1"},
                new Product {ProductId = 2, Name = "P2"},
                new Product {ProductId = 3, Name = "P3"},
                new Product {ProductId = 4, Name = "P4"},
                new Product {ProductId = 5, Name = "P5"}
            }.AsQueryable());

            var controller = new ProductController(mock.Object) { PageSize = 3 };

            var result = (ProductsListViewModel)controller.List(null, 2).Model;

            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");
        }

        [TestMethod]
        public void Can_Filter_Products()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductId = 1, Name = "P1", Category="Cat1"},
                new Product {ProductId = 2, Name = "P2", Category="Cat2"},
                new Product {ProductId = 3, Name = "P3", Category="Cat1"},
                new Product {ProductId = 4, Name = "P4", Category="Cat2"},
                new Product {ProductId = 5, Name = "P5", Category="Cat3"}
            }.AsQueryable());

            var controller = new ProductController(mock.Object) { PageSize = 3 };

            Product[] result = ((ProductsListViewModel)controller.List("Cat2", 1).Model).Products.ToArray();

            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "Cat2");
        }


        [TestMethod]
        public void Can_Create_Categories()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductId = 1, Name = "P1", Category = "Apples"},
                new Product {ProductId = 2, Name = "P2", Category = "Apples"},
                new Product {ProductId = 3, Name = "P3", Category = "Plums"},
                new Product {ProductId = 4, Name = "P4", Category = "Oranges"}
            }.AsQueryable());

            var target = new NavController(mock.Object);
            string[] results = ((IEnumerable<string>)target.Menu(null).Model).ToArray();

            Assert.AreEqual(results[0], "Apples");
            Assert.AreEqual(results[1], "Oranges");
            Assert.AreEqual(results[2], "Plums");

        }

        [TestMethod]
        public void Indicates_Selected_Category()
        {
            // Arrange
            // - create the mock repository
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                 new Product {ProductId = 1, Name = "P1", Category = "Apples"},
                 new Product {ProductId = 4, Name = "P2", Category = "Oranges"},
                 }.AsQueryable());
            // Arrange - create the controller
            var target = new NavController(mock.Object);
            // Arrange - define the category to selected
            const string categoryToSelect = "Apples";
            // Action
            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;
            // Assert
            Assert.AreEqual(categoryToSelect, result);
        }


        [TestMethod]
        public void Generate_Category_Specific_Product_Count()
        {
            // Arrange
            // - create the mock repository
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                 new Product {ProductId = 1, Name = "P1", Category = "Cat1"},
                 new Product {ProductId = 2, Name = "P2", Category = "Cat2"},
                 new Product {ProductId = 3, Name = "P3", Category = "Cat1"},
                 new Product {ProductId = 4, Name = "P4", Category = "Cat2"},
                 new Product {ProductId = 5, Name = "P5", Category = "Cat3"}
                 }.AsQueryable());

            var target = new ProductController(mock.Object);
            target.PageSize = 3;

            Assert.AreEqual(5, ((ProductsListViewModel)target.List(null).Model).PagingInfo.TotalItems);
            Assert.AreEqual(2, ((ProductsListViewModel)target.List("Cat1").Model).PagingInfo.TotalItems);
            Assert.AreEqual(2, ((ProductsListViewModel)target.List("Cat2").Model).PagingInfo.TotalItems);
            Assert.AreEqual(1, ((ProductsListViewModel)target.List("Cat3").Model).PagingInfo.TotalItems);

        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductId = 1, Name = "P1"},
                new Product {ProductId = 2, Name = "P2"},
                new Product {ProductId = 3, Name = "P3"},
                new Product {ProductId = 4, Name = "P4"},
                new Product {ProductId = 5, Name = "P5"}
            }.AsQueryable());

            var controller = new ProductController(mock.Object) { PageSize = 3 };

            var result = (ProductsListViewModel)controller.List(null, 2).Model;

            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }

        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            HtmlHelper myHelper = null;

            var pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            Func<int, string> pageUrlDelegate = i => "Page" + i;

            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            Assert.AreEqual(result.ToString(), @"<a href=""Page1"">1</a>"
                + @"<a class=""selected"" href=""Page2"">2</a>"
                + @"<a href=""Page3"">3</a>");

        }
    }

    [TestClass]
    public class CartTests
    {
        [TestMethod]
        public void Can_Add_New_Lines()
        {
            var p1 = new Product { ProductId = 1, Name = "P1" };
            var p2 = new Product { ProductId = 2, Name = "P2" };

            var target = new Cart();

            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            CartLine[] results = target.Lines.ToArray();

            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Product, p1);
            Assert.AreEqual(results[1].Product, p2);
        }

        [TestMethod]
        public void Can_Add_Quantity_For_Existing_Lines()
        {
            var p1 = new Product { ProductId = 1, Name = "P1" };
            var p2 = new Product { ProductId = 2, Name = "P2" };

            var target = new Cart();

            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 10);
            CartLine[] results = target.Lines.OrderBy(c => c.Product.ProductId).ToArray();

            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Quantity, 11);
            Assert.AreEqual(results[1].Quantity, 1);
        }

        [TestMethod]
        public void Can_Remove_Line()
        {
            // Arrange - create some test products
            var p1 = new Product { ProductId = 1, Name = "P1" };
            var p2 = new Product { ProductId = 2, Name = "P2" };
            var p3 = new Product { ProductId = 3, Name = "P3" };
            // Arrange - create a new cart
            var target = new Cart();
            // Arrange - add some products to the cart
            target.AddItem(p1, 1);
            target.AddItem(p2, 3);
            target.AddItem(p3, 5);
            target.AddItem(p2, 1);
            // Act
            target.RemoveLine(p2);
            // Assert
            Assert.AreEqual(target.Lines.Count(c => c.Product == p2), 0);
            Assert.AreEqual(target.Lines.Count(), 2);
        }

        [TestMethod]
        public void Calculate_Cart_Total()
        {
            var p1 = new Product { ProductId = 1, Name = "P1", Price = 100M };
            var p2 = new Product { ProductId = 2, Name = "P2", Price = 50M };

            var target = new Cart();

            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 3);
            decimal result = target.ComputeTotalValue();

            Assert.AreEqual(result, 450M);
        }

        [TestMethod]
        public void Can_Clear_Contents()
        {
            // Arrange - create some test products
            var p1 = new Product { ProductId = 1, Name = "P1", Price = 100M };
            var p2 = new Product { ProductId = 2, Name = "P2", Price = 50M };
            // Arrange - create a new cart
            var target = new Cart();
            // Arrange - add some items
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            // Act - reset the cart
            target.Clear();
            // Assert
            Assert.AreEqual(target.Lines.Count(), 0);
        }
    }
}
