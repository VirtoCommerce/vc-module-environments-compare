using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.EnvironmentsCompare.Core;
using VirtoCommerce.EnvironmentsCompare.Core.Models;
using VirtoCommerce.EnvironmentsCompare.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.EnvironmentsCompare.Data.Services;

public class EnvironmentsCompareClient(IHttpClientFactory httpClientFactory) : IEnvironmentsCompareClient
{
    public async Task<IList<ComparableEnvironmentSettings>> GetSettingsAsync(IList<ComparableEnvironment> comparableEnvironments)
    {
        using var httpClient = httpClientFactory.CreateClient();

        var environmentTasks = comparableEnvironments.Select(async environment =>
        {
            var result = AbstractTypeFactory<ComparableEnvironmentSettings>.TryCreateInstance();
            result.EnvironmentName = environment.Name;

            try
            {
                using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{environment.Url}/{ModuleConstants.Api.SettingsCompareRoute}");
                httpRequest.Headers.TryAddWithoutValidation(ModuleConstants.Api.ApiKeyHeaderName, environment.ApiKey);

                using var httpResponse = await httpClient.SendAsync(httpRequest);
                httpResponse.EnsureSuccessStatusCode();

                var responseString = await httpResponse.Content.ReadAsStringAsync();
                var environmentSettings = JsonConvert.DeserializeObject<IList<ComparableSettingScope>>(responseString);

                result.Settings = environmentSettings;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }

            return result;
        }).ToArray();

        await Task.WhenAll(environmentTasks);

        return environmentTasks
            .Select(x => x.Result)
            .ToArray();
    }
}
