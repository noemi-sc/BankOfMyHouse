namespace BankOfMyHouse.Domain.Investments;

public class Company
{
	public int Id { get; set; }
	public string Name { get; set; }

	//NAVIGATIONS
	public ICollection<CompanyStockPrice> StockPriceHistory { get; set; } = new List<CompanyStockPrice>();
}
