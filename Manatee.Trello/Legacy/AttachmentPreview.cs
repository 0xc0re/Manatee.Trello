﻿/***************************************************************************************

	Copyright 2013 Little Crab Solutions

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		AttachmentPreview.cs
	Namespace:		Manatee.Trello
	Class Name:		AttachmentPreview
	Purpose:		Represents a thumbnail preview of a card attachment on Trello.com.

***************************************************************************************/
using Manatee.Trello.Json;

namespace Manatee.Trello
{
	///<summary>
	/// Represents a thumbnail preview of a card attachment.
	///</summary>
	public class AttachmentPreview
	{
		private readonly IJsonAttachmentPreview _jsonAttachmentPreview;

		///<summary>
		/// Indicates the ID of the attachment preview.
		///</summary>
		public string Id { get { return _jsonAttachmentPreview.Id; } }
		///<summary>
		/// Indicates the height in pixels of the attachment preview.
		///</summary>
		public int? Height { get { return _jsonAttachmentPreview.Height; } }
		///<summary>
		/// Indicates the attachment storage location.
		///</summary>
		public string Url { get { return _jsonAttachmentPreview.Url; } }
		///<summary>
		/// Indicates the width in pixels of the attachment preview.
		///</summary>
		public int? Width { get { return _jsonAttachmentPreview.Width; } }

		internal AttachmentPreview(IJsonAttachmentPreview jsonAttachmentPreview)
		{
			_jsonAttachmentPreview = jsonAttachmentPreview;
		}
	}
}