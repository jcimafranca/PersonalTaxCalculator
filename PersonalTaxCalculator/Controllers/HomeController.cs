using Newtonsoft.Json;
using PersonalTaxCalculator.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace PersonalTaxCalculator.Controllers
{
    public class HomeController : Controller
    {
        [AcceptVerbs("Get")]
        public ActionResult Index()
        {
            List<TaxRateModel> taxRates = GetTaxSettings();
            if ((taxRates == null) || (!taxRates.Any()))
            {
                ViewBag.Message = ConfigurationManager.AppSettings["JsonNotFoundMsg"];
                return View();
            }
            else
            {
                return View(taxRates);
            }
        }

        [AcceptVerbs("Get")]
        public ActionResult IndexPathTesting(string pathFolder)
        {
            List<TaxRateModel> taxRates = GetTaxSettings(pathFolder);
            if ((taxRates == null) || (!taxRates.Any()))
            {
                ViewBag.Message = ConfigurationManager.AppSettings["JsonNotFoundMsg"];
                return View();
            }
            else
            {
                return View(taxRates);
            }
        }

        [AcceptVerbs("Post")]
        public ActionResult Index(string AnnualSalary)
        {
            List<TaxRateModel> taxRates = GetTaxSettings();
            if ((taxRates == null) || (!taxRates.Any()))
            {
                ViewBag.Message = ConfigurationManager.AppSettings["JsonNotFoundMsg"];
                ViewBag.Salary = "";
                return View();
            }
            else
            {
                bool IsParse = false;
                decimal decAnnualSalary = 0;

                if (decimal.TryParse(AnnualSalary, out decAnnualSalary))
                {
                    IsParse = true;
                }
                
                if (string.IsNullOrEmpty(AnnualSalary) || (IsParse==false && decAnnualSalary == 0))
                {
                    ViewBag.Message = "Please Enter a valid Annual Salary.";
                    ViewBag.Salary = "";
                }
                else
                {
                    taxRates = CalculateTax(taxRates, decAnnualSalary);
                    if (taxRates.Sum(x => x.TaxCollected).Equals(0) && decAnnualSalary != 0)
                    {
                        ViewBag.Message = "Sorry! something went wrong with the calculation.";
                        ViewBag.Salary = "";
                    }
                    else
                    {
                        ViewBag.Message = "";
                        ViewBag.Salary = "for " + decAnnualSalary.ToString("C");
                    }
                }

                return View(taxRates);
            }
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        private List<TaxRateModel> GetTaxSettings()
        {
            List<TaxRateModel> taxRates;

            try
            {
                string file = ConfigurationManager.AppSettings["TaxSettingsFile"];
                string folder = ConfigurationManager.AppSettings["TaxSettingsFolder"];                
                string pathFolder = Path.Combine(folder, file);

                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string fullPath = Path.Combine(baseDir, pathFolder);

                using (StreamReader sr = new StreamReader(fullPath))
                {
                    taxRates = JsonConvert.DeserializeObject<List<TaxRateModel>>(sr.ReadToEnd());
                }

                return taxRates.OrderBy(o => o.BandStart).ToList();
            }
            catch (Exception ex)
            {                
                return new List<TaxRateModel>();
            }
        }

        private List<TaxRateModel> GetTaxSettings(string pathFolder)
        {
            List<TaxRateModel> taxRates;

            try
            {                
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string fullPath = Path.Combine(baseDir, pathFolder);

                using (StreamReader sr = new StreamReader(fullPath))
                {
                    taxRates = JsonConvert.DeserializeObject<List<TaxRateModel>>(sr.ReadToEnd());
                }

                return taxRates.OrderBy(o => o.BandStart).ToList();
            }
            catch (Exception ex)
            {
                return new List<TaxRateModel>();
            }
        }

        private List<TaxRateModel> CalculateTax(List<TaxRateModel> currentTaxRates, decimal annualSalary)
        {
            decimal taxCollected = 0;
            decimal bandStart = 0;
            decimal bandFinish = 0;
            decimal decTaxRate = 0;

            try
            {
                for (int i = 0; i < currentTaxRates.Count; i++)
                {
                    decimal.TryParse(currentTaxRates[i].BandStart, out bandStart);
                    decimal.TryParse(currentTaxRates[i].BandFinish, out bandFinish);
                    decTaxRate = currentTaxRates[i].TaxRatePercent / 100;

                    if (!currentTaxRates[i].BandFinish.Trim().Contains("and over"))
                    {
                        if (annualSalary >= bandFinish)
                            taxCollected = Math.Round((bandFinish - bandStart) * decTaxRate, 2);

                        else if (annualSalary >= bandStart)
                            taxCollected = Math.Round((annualSalary - bandStart) * decTaxRate, 2);

                        else
                            taxCollected = 0;
                    }
                    else
                    {
                        if (annualSalary >= bandStart)
                            taxCollected = Math.Round((annualSalary - bandStart) * decTaxRate, 2);
                        else
                            taxCollected = 0;
                    }

                    currentTaxRates[i].TaxCollected = taxCollected;
                    currentTaxRates[i].TaxCollectedForDisplay = taxCollected.ToString("C");

                }

                return currentTaxRates;
            }
            catch (Exception ex)
            {                
                return currentTaxRates;
            }
        }
    }
}