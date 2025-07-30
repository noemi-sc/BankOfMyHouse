namespace BankOfMyHouse.Domain.Investments
{
	public class Investment
	{
		public int Id { get; set; }
		public decimal SharesAmount { get; set; }

		//NAVIGATIONS
		public Company Company { get; set; }
	}
}
