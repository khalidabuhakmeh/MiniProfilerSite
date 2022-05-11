using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniProfilerSite.Models;

namespace MiniProfilerSite.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly Database _database;

    public IndexModel(ILogger<IndexModel> logger, Database database)
    {
        _logger = logger;
        _database = database;
    }

    public async Task OnGet()
    {
        Products = await _database
            .Products
            .OrderByDescending(p => p.Price)
            .Take(10)
            .ToListAsync();
    }

    public List<Product> Products { get; set; } = new();
}