using System;
using System.Collections.Generic;
using Manatee.Json;
using Manatee.Json.Serialization;

namespace Manatee.Trello.Json.Entities
{
	internal class ManateeAction : IJsonAction, IJsonSerializable
	{
		public string Id { get; set; }
		public IJsonMember MemberCreator { get; set; }
		public IJsonActionData Data { get; set; }
		public ActionType? Type { get; set; }
		public DateTime? Date { get; set; }
		public string Text { get; set; }
		public List<IJsonReaction> Reactions { get; set; }

		public virtual void FromJson(JsonValue json, JsonSerializer serializer)
		{
			if (json.Type != JsonValueType.Object) return;
			var obj = json.Object;
			Id = obj.TryGetString("id");
			MemberCreator = obj.Deserialize<IJsonMember>(serializer, "memberCreator") ?? obj.Deserialize<IJsonMember>(serializer, "idMemberCreator");
			Data = obj.Deserialize<IJsonActionData>(serializer, "data");
			Type = obj.Deserialize<ActionType?>(serializer, "type");
			Date = obj.Deserialize<DateTime?>(serializer, "date");
			Reactions = obj.Deserialize<List<IJsonReaction>>(serializer, "reactions");
		}
		public virtual JsonValue ToJson(JsonSerializer serializer)
		{
			return new JsonObject {{"text", Text}};
		}
	}

	internal class ManateeReaction : IJsonReaction, IJsonSerializable
	{
		public string Id { get; set; }
		public IJsonMember Member { get; set; }
		public string IdModel { get; set; }
		public IJsonEmoji Emoji { get; set; }

		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			if (json.Type != JsonValueType.Object) return;
			var obj = json.Object;
			Id = obj.TryGetString("id");
			Member = obj.Deserialize<IJsonMember>(serializer, "member") ?? obj.Deserialize<IJsonMember>(serializer, "idMember");
			IdModel = obj.TryGetString("idModel");
			Emoji = obj.Deserialize<IJsonEmoji>(serializer, "emoji");
		}

		public JsonValue ToJson(JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}

	internal class ManateeEmoji : IJsonEmoji, IJsonSerializable
	{
		public string Unified { get; set; }
		public string Native { get; set; }
		public string Name { get; set; }
		public object SkinVariation { get; set; }
		public string ShortName { get; set; }

		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			if (json.Type != JsonValueType.Object) return;
			var obj = json.Object;
			Unified = obj.TryGetString("unified");
			Native = obj.TryGetString("native");
			Name = obj.TryGetString("name");
			//SkinVariation = ???;
			ShortName = obj.TryGetString("shortName");
		}

		public JsonValue ToJson(JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}
