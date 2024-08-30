using System;

namespace AvaloniaApplication1.Models;

public record Transaction
{
    public int Id { get; set; }
    public string Category { get; set; } = "Unknown";
    public string Merchant { get; set; } = "Unknown";
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}
