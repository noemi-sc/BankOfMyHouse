namespace BankOfMyHouse.Domain.BankAccounts
{
	public sealed record Currency
	{
		public Currency(string name, string code)
		{
			Name = name;
			Code = code;
		}

		public string Name { get; init; }
		public string Code { get; init; }
	}
}