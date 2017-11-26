using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;

// TODO :: MsgPack 사용.
namespace LoginServer
{
	class HttpSender
	{
		// Http 형식의 api를 호출해주는 함수.
		// 다른 서버, 클라이언트와의 통신을 위해 사용.
		public static async Task<Result_t> RequestHttp<Request_t, Result_t>(string address, int port, string reqApi, Request_t reqPacket) where Result_t : new()
		{
			var resultData = new Result_t();

			string reqAddress = "http://" + address + ":" + port.ToString() + "/" + reqApi;
			var requestJson = JsonConvert.SerializeObject(reqPacket);

			var content = new ByteArrayContent(Encoding.UTF8.GetBytes(requestJson));
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var network = new HttpClient();
			HttpResponseMessage response = null;
			string responseString = "";

			try
			{
				response = await network.PostAsync(reqAddress, content).ConfigureAwait(false);

				if (response.IsSuccessStatusCode == false)
				{
					return resultData;
				}

				responseString = await response.Content.ReadAsStringAsync();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				if (response == null)
				{
					response = new HttpResponseMessage();
				}
				response.StatusCode = HttpStatusCode.InternalServerError;
				response.ReasonPhrase = string.Format("RestHttpClient.SendRequest failed: {0}", e);
			}

			var responseJson = JsonConvert.DeserializeObject<Result_t>(responseString);

			return responseJson;
		}
	}
}