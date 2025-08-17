namespace BankOfMyHouse.Domain.BankAccounts
{
	public sealed record Currency
	{
		public Currency(string name, string code, string symbol)
		{
			Name = name;
			Code = code;
			Symbol = symbol;
		}

		public int Id { get; init; }
		public string Name { get; init; }
		public string Code { get; init; }
		public string Symbol { get; init; }
	}
}