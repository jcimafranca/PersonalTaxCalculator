using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PersonalTaxCalculator.Controllers;
using PersonalTaxCalculator.Models;

namespace PersonalTaxCalculator.Tests.Controllers
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestInvalidAnnualSalary()
        {
            //Arrange
            HomeController controller = new HomeController();

            //Act
            string annualSalary = "aaaaaa";
            ActionResult result = controller.Index(annualSalary);

            //Assert  
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;

            string message = null;
            foreach (var each in viewResult.ViewData.Values)
            {
                if (!string.IsNullOrEmpty(each.ToString()))
                {
                    message = each.ToString();
                }
            }

            Assert.AreEqual(message, "Please Enter a valid Annual Salary.");
        }

        [TestMethod]
        public void TestZeroAnnualSalary()
        {
            //Arrange
            HomeController controller = new HomeController();

            //Act
            string annualSalary = "0";            
            ActionResult result = controller.Index(annualSalary);

            //Assert  
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;           

            //Assert            
            Assert.IsInstanceOfType(viewResult.Model, typeof(IEnumerable<TaxRateModel>));
            IEnumerable<TaxRateModel> model = (IEnumerable<TaxRateModel>)viewResult.Model;

            decimal? taxCollected = 0;
            foreach (var each in model)
            {
                taxCollected += each.TaxCollected;
            }

            Assert.AreEqual(taxCollected.Value, 0);
        }

        [TestMethod]
        public void TestNullAnnualSalary()
        {
            //Arrange
            HomeController controller = new HomeController();

            //Act
            string annualSalary = null;
            ActionResult result = controller.Index(annualSalary);

            //Assert  
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;

            string message = null;
            foreach (var each in viewResult.ViewData.Values)
            {
                if (!string.IsNullOrEmpty(each.ToString()))
                {
                    message = each.ToString();
                }
            }

            Assert.AreEqual(message, "Please Enter a valid Annual Salary.");
        }

        [TestMethod]
        public void Test80KAnnualSalary()
        {
            //Arrange
            HomeController controller = new HomeController();

            //Act
            string annualSalary = "80000";
            ActionResult result = controller.Index(annualSalary);

            //Assert  
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;

            //Assert            
            Assert.IsInstanceOfType(viewResult.Model, typeof(IEnumerable<TaxRateModel>));
            IEnumerable<TaxRateModel> model = (IEnumerable<TaxRateModel>)viewResult.Model;

            decimal? taxCollected = 0;
            foreach (var each in model)
            {
                taxCollected += each.TaxCollected;
            }

            Assert.AreEqual(taxCollected.Value, 19230);
        }

        [TestMethod]
        public void Test60KAnnualSalary()
        {
            //Arrange
            HomeController controller = new HomeController();

            //Act
            string annualSalary = "60000";
            ActionResult result = controller.Index(annualSalary);

            //Assert  
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;

            //Assert            
            Assert.IsInstanceOfType(viewResult.Model, typeof(IEnumerable<TaxRateModel>));
            IEnumerable<TaxRateModel> model = (IEnumerable<TaxRateModel>)viewResult.Model;

            decimal? taxCollected = 0;
            foreach (var each in model)
            {
                taxCollected += each.TaxCollected;
            }

            Assert.AreEqual(taxCollected.Value, 12530);
        }

        [TestMethod]
        public void Test100KAnnualSalary()
        {
            //Arrange
            HomeController controller = new HomeController();

            //Act
            string annualSalary = "100,000.00";
            ActionResult result = controller.Index(annualSalary);

            //Assert  
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;

            //Assert            
            Assert.IsInstanceOfType(viewResult.Model, typeof(IEnumerable<TaxRateModel>));
            IEnumerable<TaxRateModel> model = (IEnumerable<TaxRateModel>)viewResult.Model;

            decimal? taxCollected = 0;
            foreach (var each in model)
            {
                taxCollected += each.TaxCollected;
            }

            Assert.AreEqual(taxCollected.Value, 26330);
        }

        [TestMethod]
        public void TestSettingsNotFound()
        {
            //Arrange
            HomeController controller = new HomeController();

            //Act
            string file = "IncorrectTaxSettings.json";
            string folder = "JSON";
            string pathFolder = Path.Combine(folder, file);
            ActionResult result = controller.IndexPathTesting(pathFolder);

            //Assert  
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;

            string message = null;
            foreach (var each in viewResult.ViewData.Values)
            {
                if (!string.IsNullOrEmpty(each.ToString()))
                {
                    message = each.ToString();
                }
            }

            Assert.AreEqual(message, ConfigurationManager.AppSettings["JsonNotFoundMsg"]);
        }

        [TestMethod]
        public void TestSettingsFound()
        {
            //Arrange
            HomeController controller = new HomeController();

            //Act
            string file = "TaxSettings.json";
            string folder = "JSON";
            string pathFolder = Path.Combine(folder, file);
            ActionResult result = controller.IndexPathTesting(pathFolder);

            //Assert  
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;

            string message = null;
            foreach (var each in viewResult.ViewData.Values)
            {
                if (!string.IsNullOrEmpty(each.ToString()))
                {
                    message = each.ToString();
                }
            }

            Assert.AreEqual(message, null);
        }

        [TestMethod]
        public void TestAnotherSettingsReturnsFiveModel()
        {
            //Arrange
            HomeController controller = new HomeController();

            //Act
            string file = "TaxSettings2018.json";
            string folder = "JSON";
            string pathFolder = Path.Combine(folder, file);
            ActionResult result = controller.IndexPathTesting(pathFolder);

            //Assert  
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;

            //Assert            
            Assert.IsInstanceOfType(viewResult.Model, typeof(IEnumerable<TaxRateModel>));
            IEnumerable<TaxRateModel> model = (IEnumerable<TaxRateModel>)viewResult.Model;

            int Count = 0;
            foreach (var each in model)
                Count++;

            Assert.AreEqual(Count, 5);
        }
    }
}
