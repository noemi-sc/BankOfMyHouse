using BankOfMyHouse.Domain.Investments;

namespace BankOfMyHouse.Application.Services.Investments.Interfaces
{
	public interface IInvestmentService
	{
		Task<Investment> CreateInvestment(int userId, int bankAccountId, int companyId, decimal investmentAmount, CancellationToken ct);
		Task<Investment> WithdrawInvestment(int userId, int companyId, CancellationToken ct);
	}
}
