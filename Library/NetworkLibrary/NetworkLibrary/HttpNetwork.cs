﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NetworkLibrary
{
    internal class HttpNetwork
    {
		// Http Post를 보내주고 그 결과를 반환해주는 메소드.
		// TODO :: 쓰레드 새로 만들어서 하기.
		public async Task<RESULT_T> HttpPostRequest<REQUEST_T, RESULT_T>(string postUri, byte[] postData)
		{
			using (var httpClient = new HttpClient())
			{
				var httpContent = new ByteArrayContent(postData);

				HttpResponseMessage responseMessage = null;

				try
				{
					responseMessage = await httpClient.PostAsync(postUri, httpContent);
				}
				catch (Exception e)
				{
					Console.WriteLine($"Http Post Request failed. Message({e.Message})");
				}

				var responseByte = await responseMessage.Content.ReadAsByteArrayAsync();

				return MessagePack.MessagePackSerializer.Deserialize<RESULT_T>(responseByte);
			}
		}
    }
}
