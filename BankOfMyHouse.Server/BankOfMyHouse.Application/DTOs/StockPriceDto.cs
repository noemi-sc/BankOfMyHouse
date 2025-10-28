namespace BankOfMyHouse.Application.DTOs;

public class StockPriceDto
{
	public decimal StockPrice { get; set; }
	public DateTimeOffset TimeOfPriceChange { get; set; }
	public int CompanyId { get; set; }
}
