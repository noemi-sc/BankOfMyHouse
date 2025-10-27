using BankOfMyHouse.Domain.Investments;

namespace BankOfMyHouse.Application.Services.Investments.Interfaces
{
	public interface IInvestmentService
	{
		Task<Investment> CreateInvestment(int userId, int bankAccountId, int companyId, decimal investmentAmount, CancellationToken ct);
		Task<List<Company>> GetCompanies(CancellationToken ct);
		Task<List<Investment>> GetInvestments(int userId, CancellationToken ct);
		Task<Investment> WithdrawInvestment(int userId, int companyId, CancellationToken ct);
		Task<Dictionary<int, List<CompanyStockPrice>>> GetHistoricalStockPrices(DateTime startDate, int? companyId, CancellationToken ct);
	}
}
