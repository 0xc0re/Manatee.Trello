﻿/***************************************************************************************

	Copyright 2014 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		Webhook.cs
	Namespace:		Manatee.Trello
	Class Name:		Webhook
	Purpose:		Represents a webhook.

***************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Trello.Contracts;
using Manatee.Trello.Internal;
using Manatee.Trello.Internal.Synchronization;
using Manatee.Trello.Internal.Validation;
using Manatee.Trello.Json;

namespace Manatee.Trello
{
	public abstract class Webhook
	{
		internal Webhook() {}

		public static void ProcessNotification(string content)
		{
			var notification = TrelloConfiguration.Deserializer.Deserialize<IJsonWebhookNotification>(content);
			var action = new Action(notification.Action);

			foreach (var obj in TrelloConfiguration.Cache.OfType<ICanWebhook>())
			{
				obj.ApplyAction(action);
			}
		}
	}

	/// <summary>
	/// Represents a webhook.
	/// </summary>
	/// <typeparam name="T">The type of object to which the webhook is attached.</typeparam>
	public class Webhook<T> : Webhook
		where T : class, ICanWebhook
	{
		private readonly Field<string> _callBackUrl;
		private readonly Field<string> _description;
		private readonly Field<bool?> _isActive;
		private readonly Field<T> _target;
		private readonly WebhookContext<T> _context;

		/// <summary>
		/// Gets or sets a callback URL for the webhook.
		/// </summary>
		public string CallBackUrl
		{
			get { return _callBackUrl.Value; }
			set { _callBackUrl.Value = value; }
		}
		/// <summary>
		/// Gets or sets a description for the webhook.
		/// </summary>
		public string Description
		{
			get { return _description.Value; }
			set { _description.Value = value; }
		}
		/// <summary>
		/// Gets the webhook's ID>
		/// </summary>
		public string Id { get; private set; }
		/// <summary>
		/// Gets or sets whether the webhook is active.
		/// </summary>
		public bool? IsActive
		{
			get { return _isActive.Value; }
			set { _isActive.Value = value; }
		}
		/// <summary>
		/// Gets or sets the webhook's target.
		/// </summary>
		public T Target
		{
			get { return _target.Value; }
			set { _target.Value = value; }
		}

		/// <summary>
		/// Raised when data on the webhook is updated.
		/// </summary>
		public event Action<Webhook<T>, IEnumerable<string>> Updated;

		/// <summary>
		/// Creates a new instance of the <see cref="Webhook{T}"/> object and registers a webhook with Trello.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="description"></param>
		/// <param name="callBackUrl"></param>
		public Webhook(T target, string callBackUrl, string description = null)
		{
			_context = new WebhookContext<T>();
			Id = _context.Create(target, description, callBackUrl);

			_callBackUrl = new Field<string>(_context, () => CallBackUrl);
			_callBackUrl.AddRule(UriRule.Instance);
			_description = new Field<string>(_context, () => Description);
			_isActive = new Field<bool?>(_context, () => IsActive);
			_target = new Field<T>(_context, () => Target);
			_target.AddRule(NotNullRule<T>.Instance);

			TrelloConfiguration.Cache.Add(this);
		}
		/// <summary>
		/// Creates a new instance of the <see cref="Webhook{T}"/> object for a webhook which has already been registered with Trello.
		/// </summary>
		/// <param name="id"></param>
		public Webhook(string id)
		{
			Id = id;
			_context = new WebhookContext<T>(Id);
			_context.Synchronized += Synchronized;

			_callBackUrl = new Field<string>(_context, () => CallBackUrl);
			_description = new Field<string>(_context, () => Description);
			_isActive = new Field<bool?>(_context, () => IsActive);
			_isActive.AddRule(NullableHasValueRule<bool>.Instance);
			_target = new Field<T>(_context, () => Target);
			_target.AddRule(NotNullRule<T>.Instance);

			TrelloConfiguration.Cache.Add(this);
		}
		internal Webhook(IJsonWebhook json)
			: this(json.Id)
		{
			_context.Merge(json);
		}

		/// <summary>
		/// Deletes the webhook.
		/// </summary>
		/// <remarks>
		/// This permanently deletes the card from Trello's server, however, this object will
		/// remain in memory and all properties will remain accessible.
		/// </remarks>
		public void Delete()
		{
			_context.Delete();
			TrelloConfiguration.Cache.Remove(this);
		}

		private void Synchronized(IEnumerable<string> properties)
		{
			Id = _context.Data.Id;
			var handler = Updated;
			if (handler != null)
				handler(this, properties);
		}
	}
}