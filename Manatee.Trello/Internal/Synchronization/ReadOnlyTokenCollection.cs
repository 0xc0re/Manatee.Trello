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

		public ReadOnlyTokenCollection(Func<string> getOwnerId, TrelloAuthorization auth)
			: base(getOwnerId, auth)
		{
		}

		internal override async Task PerformRefresh(bool force, CancellationToken ct)
		{
			var endpoint = EndpointFactory.Build(EntityRequestType.Member_Read_Tokens, new Dictionary<string, object> {{"_id", OwnerId}});
			var newData = await JsonRepository.Execute<List<IJsonToken>>(Auth, endpoint, ct, AdditionalParameters);

			Items.Clear();
			Items.AddRange(newData.Select(jn =>
				{
					var token = jn.GetFromCache<Token, IJsonToken>(Auth);
					token.Json = jn;
					return token;
				}));
		}
	}
}