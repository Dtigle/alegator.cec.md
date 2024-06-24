using System.ComponentModel.DataAnnotations;

namespace CEC.SAISE.EDayModule.Infrastructure.Grids
{
	public abstract class JqGridRow
	{
		[ScaffoldColumn(false)]
		public string Id { get; set; }
	}
}