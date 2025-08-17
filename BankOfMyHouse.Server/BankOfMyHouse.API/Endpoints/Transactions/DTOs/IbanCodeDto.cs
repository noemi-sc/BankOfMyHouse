namespace BankOfMyHouse.API.Endpoints.Transactions.DTOs
{
	public sealed record IbanCodeDto
	{
		public IbanCodeDto(string value)
		{
			Value = value.Trim();
		}

		public string Value { get; init; }
	}
}
