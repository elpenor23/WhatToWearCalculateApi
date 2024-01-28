using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace WhatToWearCalculateApi.Utilities
{
    public class AzureAccess
    {
        public static async Task<string> GetSecretAsync(string key)
        {
            var secret = string.Empty;
            try
            {
                // TODO: Probably move this to an environment variable config
                //don't care so much now though
                var kvUri = "https://whattowearsecrets.vault.azure.net";
                var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
                var keyVaultSecret = await client.GetSecretAsync(key);
                secret = keyVaultSecret.Value.Value;
            }
            catch (Exception ex)
            {
                ex.Data.Add("Secret Vault Access Failed for key:", key);
                throw;
            }

            return secret;
        }
    }
}