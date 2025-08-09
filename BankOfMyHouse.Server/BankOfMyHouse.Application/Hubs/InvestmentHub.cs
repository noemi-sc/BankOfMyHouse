using Microsoft.AspNetCore.SignalR;

namespace BankOfMyHouse.Application.Hubs;

public class InvestmentHub : Hub
{
	public const string Url = "/personalInvestments";

	public async Task JoinGroup(string groupName)
	{
		await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
	}

	public async Task LeaveGroup(string groupName)
	{
		await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
	}
}
