using System;
using System.Threading;
using System.Threading.Tasks;

namespace Manatee.Trello
{
	public class ReadOnlyWebhookCollection : ReadOnlyCollection<IWebhook>
	{
		public ReadOnlyWebhookCollection(Func<string> getOwnerId, TrelloAuthorization auth)
			: base(getOwnerId, auth)
		{
		}

		internal override Task PerformRefresh(bool force, CancellationToken ct)
		{
			throw new NotImplementedException();
		}
	}
}