namespace CEC.SAISE.BLL
{
	public interface ILogoImageBll
	{
		byte[] Get(long partyId);
		string NoLogoPath { get; set; }
	}
}