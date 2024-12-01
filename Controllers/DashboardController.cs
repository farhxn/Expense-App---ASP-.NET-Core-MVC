using AspNetCore.Reporting;
using Expense_App.HelperMethods;
using Expense_App.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Linq;
using System.Globalization;

namespace Expense_App.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly StockService _stockService;
        private readonly IEmailSender _emailSender;
        private IWebHostEnvironment webHostEnvironment;

        public DashboardController(ApplicationDbContext context, IWebHostEnvironment _webHostEnvironment, StockService stockService, IEmailSender emailSender)
        {
            _context = context;
            webHostEnvironment = _webHostEnvironment;
            _stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));
            this._emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            DateTime StartDate = DateTime.Today.AddDays(-6);
            DateTime EndDate = DateTime.Today;
            List<Transaction> SelectedTransaction = await _context.transactions
                .Include(x => x.Category)
                //.Where(y => y.Date >= StartDate && y.Date <= EndDate)
                .ToListAsync();

            //Cards
            int totalAmount = SelectedTransaction.Where(i => i.Category.Type == "Income").Sum(j => j.Amount);
            ViewBag.totalIncome = totalAmount.ToString("C0");

            int totalExpense = SelectedTransaction.Where(i => i.Category.Type == "Expense").Sum(j => j.Amount);
            ViewBag.totalExpense = totalExpense.ToString("C0");

            int totalBalance = totalAmount - totalExpense;
            CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture("en-US");
            cultureInfo.NumberFormat.CurrencyNegativePattern = 1;
            ViewBag.totalBalance = String.Format(cultureInfo, "{0:C0}", totalBalance);

            //Doughnut chart
            ViewBag.DoughnutChartData = SelectedTransaction.Where(i => i.Category.Type == "Expense")
                .GroupBy(x => x.CategoryId)
                .Select(k => new
                {
                    categoryTitleWithIcon = k.First().Category.TitleWithIcon,
                    amount = k.Sum(j => j.Amount),
                    formattedAmount = k.Sum(j => j.Amount).ToString("C0"),
                }).OrderByDescending(l => l.amount).ToList();

            //Income
            List<SplineChartData> IncomeSummary = SelectedTransaction.Where(i => i.Category.Type == "Income")
                .GroupBy(k => k.Date)
                .Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    income = k.Sum(l => l.Amount)
                }).ToList();

            //Expense
            List<SplineChartData> ExpenseSummary = SelectedTransaction.Where(i => i.Category.Type == "Expense")
                .GroupBy(k => k.Date)
                .Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    expense = k.Sum(l => l.Amount)
                }).ToList();

            //combine both 
            //DateTime StartDate = DateTime.Today.AddDays(-6);
            string[] lastTransactions = Enumerable.Range(0, 7)
                .Select(i => StartDate.AddDays(i).ToString("dd-MMM")).ToArray();

            ViewBag.SplineChartData = from day in lastTransactions
                                      join income in IncomeSummary on day equals income.day
                                      into dayIncomeJoined
                                      from income in dayIncomeJoined.DefaultIfEmpty()
                                      join expense in ExpenseSummary on day equals expense.day
                                      into dayExpenseJoined
                                      from expense in dayExpenseJoined.DefaultIfEmpty()
                                      select new
                                      {
                                          day = day,
                                          income = income == null ? 0 : income.income,
                                          expense = expense == null ? 0 : expense.expense,
                                      };
            ViewBag.RecentTransaction = await _context.transactions.Include(i => i.Category)
                .OrderByDescending(j => j.Date)
                .Take(5)
                .ToListAsync();

            return View();
        }

        public async Task<IActionResult> Reports()
        {
            var reciver = "farhanatif9990@gmail.com";
            var subject = "Test";
            var message = "hello world";
            await _emailSender.SendEmailAsync(reciver,subject,message);
            return View();
        }

        public async Task<IActionResult> Stocks(string symbol = "PSX")
        {
            var jsonData = await _stockService.GetStockData(symbol); 
            return View(jsonData); // Pass parsed data to view
        }


        public FileContentResult DownloadReport()
        {
            string format = "PDF", extension = "pdf", mimeType = "application/pdf",
                reportPath = $"{webHostEnvironment.WebRootPath}\\Reports\\ReportRPT.rdlc";

            var dataTable = Helper.ListToDataTable(_context.transactions.Include(x => x.Category).ToList());

            var localReport = new LocalReport(reportPath);
            localReport.AddDataSource("dsReport", dataTable);

            var res = localReport.Execute(RenderType.Pdf, 1, null, mimeType);
            return File(res.MainStream, mimeType);
        }

        //public FileContentResult DownloadReport()
        //{
        //    string format = "PDF", extension = "pdf", mimeType = "application/pdf",
        //        embbaddedPath = "Expense App.wwwroot.Reports.ReportRPT.rdlc",
        //        reportPath = $"{webHostEnvironment.WebRootPath}\\Reports\\ReportRPT.rdlc";

        //    var dataTable = Helper.ListToDataTable(_context.transactions.Include(x => x.Category).ToList());

        //    var localReport = new LocalReport(reportPath);  
        //    localReport.AddDataSource("dsReport",dataTable);

        //    var res = localReport.Execute(RenderType.Pdf,1,null,mimeType);
        //    return File(res.MainStream,mimeType);
        //}

    }
    public class SplineChartData
    {
        public string day;
        public int income;
        public int expense;
    }

}
