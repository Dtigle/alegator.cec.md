using System.IO;
using System.Linq;
using CEC.SAISE.Domain;

namespace CEC.SAISE.BLL.Impl
{
	public class LogoImageBll : ILogoImageBll
	{
		private readonly ISaiseRepository _repository;

		public string NoLogoPath { get; set; }

		public LogoImageBll(ISaiseRepository repository)
		{
			_repository = repository;
		}

		public byte[] Get(long partyId)
		{
			var party = _repository.Get<ElectionCompetitor>(partyId);
			byte[] buffer;
			if (party != null && party.ColorLogo != null)
			{
				buffer = party.ColorLogo.ToArray();
			}
			else
			{
				using (var fs = new FileStream(NoLogoPath, FileMode.Open, FileAccess.Read))
				{
					buffer = new byte[fs.Length];
					fs.Read(buffer, 0, buffer.Length);
				}
			}
			return buffer;
		}
	}
}