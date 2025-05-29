using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using Web_DoChoi.Models;
using Web_DoChoi.Models.Momo;
using RestSharp;
using Newtonsoft.Json;

namespace Web_DoChoi.Services.Momo
{
    public class MomoService : IMomoService
    {
        private readonly IOptions<MomoOptionModel> _options;
        public MomoService(IOptions<MomoOptionModel> options)
        {
            _options = options;
        }
        public async Task<MomoCreatePaymentResponseModel> CreatePaymentMomo(OrderInfoModel model)
        {
            model.OrderInfor = "Khách hàng: " + model.FullName + ". Nội dung: " + model.OrderInfor;
            var rawData =
            $"partnerCode={_options.Value.PartnerCode}" +
            $"&accessKey={_options.Value.AccessKey}" +
            $"&requestId={model.OrderId}" +
            $"&amount={model.Amount}" +
            $"&orderId={model.OrderId}" +
            $"&orderInfo={model.OrderInfor}" +
            $"&returnUrl={_options.Value.ReturnUrl}" +
            $"&notifyUrl={_options.Value.NotifyUrl}" +
            $"&extraData=";

            var signature = ComputeHmacSha256(rawData, _options.Value.SecretKey);
            var client = new RestClient(_options.Value.MomoApiUrl);
            var request = new RestRequest() { Method = Method.Post };
            request.AddHeader("Content-Type", "application/json; charset=UTF-8");
            var requestData = new
            {
                accessKey = _options.Value.AccessKey,
                partnerCode = _options.Value.PartnerCode,
                requestType = _options.Value.RequestType,
                notifyUrl = _options.Value.NotifyUrl,
                returnUrl = _options.Value.ReturnUrl,
                orderId = model.OrderId,
                amount = model.Amount.ToString(),
                orderInfo = model.OrderInfor,
                requestId = model.OrderId,
                extraData = "",
                signature = signature
            };
            request.AddParameter("application/json", JsonConvert.SerializeObject(requestData), ParameterType.RequestBody);

            var response = await client.ExecuteAsync(request);
            var momoResponse = JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(response.Content);

            return momoResponse;
        }

        public MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection)
        {
            var amount = collection.First(s => s.Key == "Amount").Value;
            var orderInfor = collection.First(s => s.Key == "OrderInfor").Value;
            var orderId = collection.First(s => s.Key == "OrderId").Value;
            var fullName = collection.First(s => s.Key == "FullName").Value;
            return new MomoExecuteResponseModel()
            {
                Amount = amount,
                OrderId = orderId,
                OrderInfor = orderInfor,
                FullName = fullName
            };
        }


        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            byte[] hashBytes;

            using (var hmac = new HMACSHA256(keyBytes))
            {
                hashBytes = hmac.ComputeHash(messageBytes);
            }

            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return hashString;
        }
    }

}

