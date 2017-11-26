using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Newtonsoft.Json;

namespace NetworkLibrary
{
    internal class HttpNetwork
    {
		private static Logger logger = LogManager.GetCurrentClassLogger();

		// Http Post를 보내주고 그 결과를 반환해주는 메소드.
		// TODO :: 쓰레드 새로 만들어서 하기.
		public async Task<RESULT_T> HttpPostRequest<REQUEST_T, RESULT_T>(string postUri, REQUEST_T postData) where RESULT_T : new()
		{
			logger.Debug($"Function Entry. Post Uri({postUri})");

			var resultData = new RESULT_T();

			using (var httpClient = new HttpClient())
			{
				var contentJson = JsonConvert.SerializeObject(postData);

				logger.Debug($"Req Json : {contentJson}");

				var httpContent = new StringContent(contentJson);

				HttpResponseMessage responseMessage = null;

				try
				{
					responseMessage = await httpClient.PostAsync(postUri, httpContent).ConfigureAwait(false);
				}
				catch (Exception e)
				{
					logger.Error($"Http Post Request failed. Exception Message : {e.Message}");
					return resultData;
				}

				if (responseMessage.IsSuccessStatusCode == false)
				{
					logger.Error($"Http Post Request failed. StatusCode : {responseMessage.StatusCode}");
					return resultData;
				}

				var responseString = await responseMessage.Content.ReadAsStringAsync();
				var responseJson = JsonConvert.DeserializeObject<RESULT_T>(responseString);

				return responseJson;
			}
		}
    }
}
