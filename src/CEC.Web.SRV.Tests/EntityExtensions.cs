using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amdaris.Domain;

namespace CEC.Web.SRV.Tests
{
	public static class EntityExtensions
	{
		public static TEntity WithId<TEntity, TId>(this TEntity entity, TId entityId) where TEntity : EntityWithTypedId<TId>
		{
			entity.SetId(entityId);
			return entity;
		}
	}
}
