using System.Text.Json.Serialization;

namespace Common.Shared
{
	public class ResponseDto<T>
	{
		public T? Data { get; set; }

		[JsonIgnore]
		public int StatusCode { get; set; }

		public List<string>? Errors { get; set; }

		public static ResponseDto<T> Success(int statusCode, T data)
			=> new() { StatusCode = statusCode, Data = data };

		public static ResponseDto<T> Success(int statusCode)
			=> new() { StatusCode = statusCode };

		public static ResponseDto<T> Fail(int statusCode, List<string> errors)
			=> new() { StatusCode = statusCode, Errors = errors };

		public static ResponseDto<T> Fail(int statusCode, string error)
			=> new() { StatusCode = statusCode, Errors = [error] };
	}
}
