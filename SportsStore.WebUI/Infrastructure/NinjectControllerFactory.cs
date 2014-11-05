using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.Domain.Concrete;
using Moq;

namespace SportsStore.WebUI.Infrastructure
{
    public class NinjectControllerFactory: DefaultControllerFactory
    {
        private readonly IKernel _ninjectKernel;

        public NinjectControllerFactory()
        {
            _ninjectKernel = new StandardKernel();
            AddBindings();
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return controllerType == null
                ? null
                : (IController) _ninjectKernel.Get(controllerType);
        }

        public void AddBindings()
        {
            //public bindings here
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new List<Product>
            {
                new Product {Name = "FootBall", Price = 25},
                new Product {Name = "Surf Board", Price = 179},
                new Product {Name = "Running Shoes", Price = 95}
            }.AsQueryable());

            //_ninjectKernel.Bind<IProductRepository>().ToConstant(mock.Object);
            _ninjectKernel.Bind<IProductRepository>().To<EFProductRepository>();

        }
    }
}