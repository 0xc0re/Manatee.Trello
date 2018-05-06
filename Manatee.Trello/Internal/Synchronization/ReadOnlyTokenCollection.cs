using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Manatee.Trello.Internal.Caching;
using Manatee.Trello.Internal.DataAccess;
using Manatee.Trello.Json;

namespace Manatee.Trello.Internal.Synchronization
{
	internal class ReadOnlyTokenCollection : ReadOnlyCollection<IToken>
	{
		private readonly Dictionary<string, object> _additionalParameters;

		public ReadOnlyTokenCollection(Func<string> getOwnerId, TrelloAuthorization auth)
			: base(getOwnerId, auth)
		{
			_additionalParameters = new Dictionary<string, object>();
		}

		public override async Task Refresh(CancellationToken ct = default(CancellationToken))
		{
			IncorporateLimit(_additionalParameters);

			var endpoint = EndpointFactory.Build(EntityRequestType.Member_Read_Tokens, new Dictionary<string, object> {{"_id", OwnerId}});
			var newData = await JsonRepository.Execute<List<IJsonToken>>(Auth, endpoint, ct, _additionalParameters);

			Items.Clear();
			Items.AddRange(newData.Select(jn =>
				{
					var token = CachingObjectFactory.GetFromCache<Token, IJsonToken>(jn, Auth);
					token.Json = jn;
					return token;
				}));
		}
	}
}