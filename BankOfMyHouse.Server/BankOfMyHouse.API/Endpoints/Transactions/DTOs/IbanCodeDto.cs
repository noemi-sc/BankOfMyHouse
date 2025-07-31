namespace BankOfMyHouse.API.Endpoints.Transactions.DTOs
{
	public sealed record IbanCodeDto
	{
		public IbanCodeDto(string value)
		{
			Value = value;
		}

		public string Value { get; init; }
	}
}
